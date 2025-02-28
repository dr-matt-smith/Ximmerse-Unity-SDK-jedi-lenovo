﻿//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.InputSystem {

	/// <summary>
	/// Wrapper for working with input of tracking device in X-Device SDK.
	/// Like UnityEngine.VR.InputTracking,it tells us VR Input tracking data.
	/// </summary>
	public class TrackerInput:MonoBehaviour {

		#region Static

		public static string ToString(TrackerInput input,int[] nodes) {
			System.Text.StringBuilder sb=new System.Text.StringBuilder(string.Format(
				"TrackingInput@{0} Timestamp:{1}\n",input.m_Handle,input.m_State.frameCount));
			if(XDevicePlugin.GetInt(input.m_Handle,XDevicePlugin.kField_ConnectionState,0)!=(int)DeviceConnectionState.Connected) {
				//
				sb.Append("Connection State=\""+((DeviceConnectionState)XDevicePlugin.GetInt(input.m_Handle,XDevicePlugin.kField_ConnectionState,0)));
				sb.Append("\"\nError Code=\""+XDevicePlugin.GetInt(input.m_Handle,XDevicePlugin.kField_ErrorCode,0)+"\"");
			}else {
				Vector3 pos=new Vector3();
				int node;for(int i=0,imax=nodes.Length;i<imax;++i) {node=nodes[i];
					if(input.Exists(node)) {
						pos=input.GetPosition(node);
						sb.AppendLine(string.Format("Node@{0} is {1}",
							node,pos.ToString("F3")));
					}else {
						sb.AppendLine(string.Format("Node@{0} is not existed.",node));
					}
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Find a TrackingInput instance identified by deviceName
		/// </summary>
		/// <param name="deviceName">Which device in X-Device SDK.</param>
		/// <returns>A TrackingInput instance.</returns>
		public static TrackerInput Find(string deviceName) {
			List<TrackerInput> list=FindAll(deviceName);
			if(list.Count==0) {
				return null;
			}else {
				return list[0];
			}
		}

		/// <summary>
		/// Find all TrackingInput instances identified by deviceName
		/// </summary>
		/// <param name="deviceName">Which device in X-Device SDK.</param>
		/// <returns>A list of TrackingInput instances.</returns>
		public static List<TrackerInput> FindAll(string deviceName) {
			List<TrackerInput> list=new List<TrackerInput>();
			list.AddRange(FindObjectsOfType<TrackerInput>());
			if(!string.IsNullOrEmpty(deviceName)) {
				list.RemoveAll((x)=>(x.deviceName!=deviceName));
			}
			return list;
		}

		#endregion Static

		#region Fields
	
		[Header("Common")]

		/// <summary>
		/// Device name tells X-Device SDK which tracking device we used .
		/// </summary>
		public string deviceName="XHawk-0";

		/// <summary>
		/// The scale of position data.
		/// </summary>
		public Vector3 sensitivity=Vector3.one;

		/// <summary>
		/// Provides a root transform for all anchors in tracking space.
		/// </summary>
		public Transform trackingSpace;

		/// <summary>
		/// The anchor is used for the tracking device which needs a “projection” point.It must be child of trackingSpace.
		/// </summary>
		public Transform anchor;

		public System.Action onRecenter;

		protected int m_Handle=-1;
		protected XDevicePlugin.TrackerState m_State=
			new XDevicePlugin.TrackerState();
		
		protected int m_PrevFrameCount=-1;
		protected int m_PrevTimestamp=-1;
		protected int m_KeepFrames;

		protected bool m_UseAnchorProjection=true;
		protected Matrix4x4 m_AnchorMatrix=Matrix4x4.identity;

		#endregion Fields

		#region Unity Messages

		protected virtual void Start() {
			XDevicePlugin.Init();
			if(m_Handle==-1) {
				m_Handle=XDevicePlugin.GetInputDeviceHandle(deviceName);
			}
		}

		protected virtual void Update() {
			UpdateState();
		}

		protected virtual void OnDestroy() {		
			XDevicePlugin.Exit();
		}

		#endregion Unity Messages

		#region Methods

		public virtual void UpdateStateForcely() {
			m_PrevFrameCount=-1;
			UpdateState();
		}

		/// <summary>
		/// Update the tracking data from native plugin(called just once per frame).
		/// </summary>
		protected virtual void UpdateState() {
			if(m_Handle>=0) {if(Time.frameCount!=m_PrevFrameCount){
				m_PrevFrameCount=Time.frameCount;
				m_PrevTimestamp=m_State.timestamp;
				//
				if(anchor!=null&&m_UseAnchorProjection) {
					m_AnchorMatrix=anchor.localToWorldMatrix;
					if(trackingSpace!=null) {
						m_AnchorMatrix=trackingSpace.worldToLocalMatrix*m_AnchorMatrix;
					}
				}
				//
				XDevicePlugin.GetInputState(m_Handle,ref m_State);
				//
			}}
		}

		/// <summary>
		/// The total number of frames that device have passed.
		/// </summary>
		public virtual int GetFrameCount() {
			UpdateState();
			//
			return m_State.frameCount;
		}

		/// <summary>
		/// Returns if the node exists at the current frame.
		/// </summary>
		public virtual bool Exists(int node) {
			UpdateState();
			//
			return (XDevicePlugin.GetNodePosition(m_Handle,0,node,null)&TrackingResult.PositionTracked)!=0;
		}

		/// <summary>
		/// The current position of the requested node.
		/// </summary>
		/// <param name="node">Node index.</param>
		/// <returns>Position of node local to its tracking space.</returns>
		public virtual Vector3 GetPosition(int node) {
			return GetPosition(node,0);
		}
	
		public virtual Vector3 GetPosition(int node,int history) {
			UpdateState();
			//
			Vector3 position=new Vector3();
			if((XDevicePlugin.GetNodePosition(m_Handle,history,node,ref position)&TrackingResult.PositionTracked)!=0) {
				position.Scale(sensitivity);
				if(m_UseAnchorProjection&&anchor!=null) {
					position=m_AnchorMatrix.MultiplyPoint3x4(position);
				}
				return position;
			}else {
				//Log.d("TrackerInput","No result when calling GetPosition("+node+","+history+") at "+Time.time+" ms.");
			}
			return Vector3.zero;
		}

		/// <summary>
		/// The current rotation of the requested node.
		/// </summary>
		/// <param name="node">Node index.</param>
		/// <returns>Rotation of node local to its tracking space.</returns>
		public virtual Quaternion GetRotation(int node) {
			UpdateState();
			//
			return Quaternion.identity;
		}

		/// <summary>
		///  Transforms position from local space to tracking space.
		/// </summary>
		public virtual Vector3 TransformPoint(Vector3 position) {
			UpdateState();
			//
			if(m_UseAnchorProjection&&anchor!=null) {
				return m_AnchorMatrix.MultiplyPoint3x4(position);
			} else {
				return position;
			}
		}

		public virtual void Recenter() {
			XDevicePlugin.SendMessage(m_Handle,XDevicePlugin.kMessage_RecenterSensor,0,0);
			if(onRecenter!=null) {
				onRecenter.Invoke();
			}
		}
		
		/// <summary>
		/// Returns the controller's current connection state.
		/// </summary>
		public virtual DeviceConnectionState connectionState{
			get{
				UpdateState();
				return (DeviceConnectionState)XDevicePlugin.GetInt(m_Handle,
					XDevicePlugin.kField_ConnectionState,
					0
				);
			}
		}
		
		public virtual string ToString(int[] nodes) {
			UpdateState();
			//
			return ToString(this,nodes);
		}

		#endregion Methods

	}

}