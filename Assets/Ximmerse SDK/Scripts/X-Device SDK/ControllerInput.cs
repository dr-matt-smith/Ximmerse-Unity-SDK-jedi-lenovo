﻿//=============================================================================
//
// Copyright 2016 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

using UnityEngine;

namespace Ximmerse.InputSystem {

	#region Enums

	[System.Flags]
	public enum ControllerType {
		/// <summary>
		/// Null controller.
		/// </summary>
		None                      = 0,

		/// <summary>
		/// Left motion controller.
		/// </summary>
		LeftController            = 0x00000001,

		/// <summary>
		/// Right motion controller.
		/// </summary>
		RightController           = 0x00000002,

		/// <summary>
		/// Motion controller.
		/// </summary>
		Controller                = LeftController|RightController,

		/// <summary>
		/// Head mount display.
		/// </summary>
		Hmd                       = 0x00000004,

		/// <summary>
		/// Xbox 360 or Xbox One gamepad on PC. Generic gamepad on Android.
		/// </summary>
		Gamepad                   = 0x00000008,

		/// <summary>
		/// Represents the logical OR of all controllers.
		/// </summary>
		All                       = ~None,
	}

	// Reference : https://msdn.microsoft.com/en-us/library/windows/apps/microsoft.directx_sdk.reference.xinput_gamepad

	public enum ControllerRawAxis:int {
		LeftTrigger,
		RightTrigger,
		LeftThumbX,
		LeftThumbY,
		RightThumbX,
		RightThumbY,
		Max,
	}

	public enum ControllerRawButton {
		DpadUp        = 0x0001,
		DpadDown      = 0x0002,
		DpadLeft      = 0x0004,
		DpadRight     = 0x0008,
		Start         = 0x0010,
		Back          = 0x0020,
		LeftThumb     = 0x0040,
		RightThumb    = 0x0080,
		LeftShoulder  = 0x0100,
		RightShoulder = 0x0200,
		Guide         = 0x0400,
		A             = 0x1000,
		B             = 0x2000,
		X             = 0x4000,
		Y             = 0x8000,
		// Emulation
		LeftThumbMove   = 0x10000,
		RightThumbMove  = 0x20000,
		LeftTrigger     = 0x40000,
		RightTrigger    = 0x80000,
		LeftThumbUp     = 0x100000,
		LeftThumbDown   = 0x200000,
		LeftThumbLeft   = 0x400000,
		LeftThumbRight  = 0x800000,
		RightThumbUp    = 0x1000000,
		RightThumbDown  = 0x2000000,
		RightThumbLeft  = 0x4000000,
		RightThumbRight = 0x8000000,
		//
		None          = 0x0,
		Any           = ~None,
	}

	public enum ControllerAxis:int {
		PrimaryTrigger   = ControllerRawAxis.LeftTrigger,
		SecondaryTrigger = ControllerRawAxis.RightTrigger,
		PrimaryThumbX    = ControllerRawAxis.LeftThumbX,
		PrimaryThumbY    = ControllerRawAxis.LeftThumbY,
		SecondaryThumbX  = ControllerRawAxis.RightThumbX,
		SecondaryThumbY  = ControllerRawAxis.RightThumbY,
		Max              = ControllerRawAxis.Max,
	}

	public enum ControllerButton {
		DpadUp              = ControllerRawButton.DpadUp,
		DpadDown            = ControllerRawButton.DpadDown,
		DpadLeft            = ControllerRawButton.DpadLeft,
		DpadRight           = ControllerRawButton.DpadRight,
		Start               = ControllerRawButton.Start,
		Back                = ControllerRawButton.Back,
		PrimaryThumb        = ControllerRawButton.LeftThumb,
		SecondaryThumb      = ControllerRawButton.RightThumb,
		PrimaryShoulder     = ControllerRawButton.LeftShoulder,
		SecondaryShoulder   = ControllerRawButton.RightShoulder,
		Guide               = ControllerRawButton.Guide,
		A                   = ControllerRawButton.A,
		B                   = ControllerRawButton.B,
		X                   = ControllerRawButton.X,
		Y                   = ControllerRawButton.Y,
		PrimaryThumbMove    = ControllerRawButton.LeftThumbMove,
		SecondaryThumbMove  = ControllerRawButton.RightThumbMove,
		PrimaryTrigger      = ControllerRawButton.LeftTrigger,
		SecondaryTrigger    = ControllerRawButton.RightTrigger,
		PrimaryThumbUp      = ControllerRawButton.LeftThumbUp,
		PrimaryThumbDown    = ControllerRawButton.LeftThumbDown,
		PrimaryThumbLeft    = ControllerRawButton.LeftThumbLeft,
		PrimaryThumbRight   = ControllerRawButton.LeftThumbRight,
		SecondaryThumbUp    = ControllerRawButton.RightThumbUp,
		SecondaryThumbDown  = ControllerRawButton.RightThumbDown,
		SecondaryThumbLeft  = ControllerRawButton.RightThumbLeft,
		SecondaryThumbRight = ControllerRawButton.RightThumbRight,
		None                = ControllerRawButton.None,
		Any                 = ControllerRawButton.Any,
	}

	#endregion Enums

	/// <summary>
	/// Wrapper for working with input of controller device in X-Device SDK.
	/// </summary>
	public class ControllerInput {

		#region Fields

		public ControllerType type=ControllerType.None;
		public int handle;
		public string name;
		public int priority;
		protected int m_PrevFrameCount=-1;
		protected XDevicePlugin.ControllerState m_State,m_PrevState;

		#endregion Fields

		#region Constructors
		
		public ControllerInput():this(-1,"Untitled Controller") {
		}
		
		public ControllerInput(int handle):this(handle,"Untitled Controller") {
		}
		
		public ControllerInput(string name):this(XDevicePlugin.GetInputDeviceHandle(name),name) {
		}
		
		public ControllerInput(int handle,string name) {
			this.handle=handle;
			this.name=name;
			//
			m_State    =new XDevicePlugin.ControllerState();
			m_PrevState=new XDevicePlugin.ControllerState();
		}

		#endregion Constructors

		#region Methods
		
		public virtual void UpdateState() {
			if(Time.frameCount!=m_PrevFrameCount){
				m_PrevFrameCount=Time.frameCount;
				m_PrevState=m_State;
				//
				XDevicePlugin.GetInputState(this.handle,ref this.m_State);
			}
		}

        /// <summary>
        /// Returns the value of the virtual axis identified by axisIndex.
        /// </summary>
        /// <param name="axisIndex">Passing in 0 for trigger value. Passing in 1 for X Axis value. Passing in 2 for Y Axis value.</param>
        /// <example> 
        /// This example shows how to use the <see cref="GetAxis"/> method.
        /// <code>
        /// using UnityEngine;
        /// using Ximmerse;
        /// using Ximmerse.InputSystem;
        /// 
        /// class TestClass : MonoBehaviour
        /// {
        ///     void Update() 
        ///     {
        ///        ControllerInput leftController = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
        ///        // Trigger value
        ///        float triggerValue = leftController.GetAxis(0);  
        ///        // Touch pad x axis value
        ///        float xAxis = leftController.GetAxis(1);
        ///        // Touch pad y axix value
        ///        float yAxis = leftController.GetAxis(2);
        ///     }
        /// }
        /// </code>
        /// </example>
        public virtual float GetAxis(int axisIndex){
			UpdateState();
			if(axisIndex>=0&&axisIndex<(int)ControllerRawAxis.Max) {
				return m_State.axes[axisIndex];
			}else {
				return 0;
			}
		}

        /// <summary>
        /// Returns true while the virtual button identified by buttonMask is held down.
        /// </summary>
        public virtual bool GetButton(uint buttonMask){
			UpdateState();
			return /*((m_PrevState.buttons&buttonMask)!=0)&&*/((m_State.buttons&buttonMask)!=0);
		}

		/// <summary>
		/// Returns true during the frame the user pressed down the virtual button identified by buttonMask.
		/// </summary>
		public virtual bool GetButtonDown(uint buttonMask){
			UpdateState();
			return ((m_PrevState.buttons&buttonMask)==0)&&((m_State.buttons&buttonMask)!=0);
		}

		/// <summary>
		/// Returns true the first frame the user releases the virtual button identified by buttonMask.
		/// </summary>
		public virtual bool GetButtonUp(uint buttonMask){
			UpdateState();
			return ((m_PrevState.buttons&buttonMask)!=0)&&((m_State.buttons&buttonMask)==0);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual TrackingResult trackingResult{
			get{
				UpdateState();
				return (TrackingResult)XDevicePlugin.GetInt(handle,
					XDevicePlugin.kField_TrackingResult,
					0
				);
			}
		}

		/// <summary>
		/// Returns the current position of this controller local to its tracking space.
		/// </summary>
		public virtual Vector3 GetPosition() {
			UpdateState();
			return new Vector3(
				 m_State.position[0],
				 m_State.position[1],
				-m_State.position[2]
			);
		}

		/// <summary>
		/// Returns the current rotation of this controller local to its tracking space.
		/// </summary>
		public virtual Quaternion GetRotation() {
			UpdateState();
			return new Quaternion(
				-m_State.rotation[0],
				-m_State.rotation[1],
				 m_State.rotation[2],
				 m_State.rotation[3]
			);
		}

		/// <summary>
		/// Returns the current accelerometer of this controller local to its tracking space.
		/// </summary>
		public virtual Vector3 GetAccelerometer() {
			UpdateState();
			return new Vector3(
				 m_State.accelerometer[0],
				 m_State.accelerometer[1],
				-m_State.accelerometer[2]
			);
		}

		/// <summary>
		/// Returns the current gyroscope of this controller local to its tracking space.
		/// </summary>
		public virtual Vector3 GetGyroscope() {
			UpdateState();
			return new Vector3(
				-m_State.gyroscope[0],
				-m_State.gyroscope[1],
				 m_State.gyroscope[2]
			);
		}

		public virtual XDevicePlugin.ControllerState GetState() {
			UpdateState();
			return m_State;
		}

		public virtual XDevicePlugin.ControllerState GetPrevState() {
			UpdateState();
			return m_PrevState;
		}
		
		/// <summary>
		/// Returns the controller's current connection state.
		/// </summary>
		public virtual DeviceConnectionState connectionState{
			get{
				UpdateState();
				return (DeviceConnectionState)XDevicePlugin.GetInt(handle,
					XDevicePlugin.kField_ConnectionState,
					0
				);
			}
		}
		
		/// <summary>
		/// Returns the controller's error code when an error has occurred.
		/// </summary>
		public virtual int errorCode{
			get{
				UpdateState();
				return XDevicePlugin.GetInt(handle,
					XDevicePlugin.kField_ErrorCode,
					0
				);
			}
		}
		
		/// <summary>
		/// Returns the controller's current battery level.If returns -1,its battery level is unknown.
		/// </summary>
		public virtual int batteryLevel{
			get{
				UpdateState();
				return XDevicePlugin.GetInt(handle,
					XDevicePlugin.kField_BatteryLevel,
					0
				);
			}
		}

		/// <summary>
		/// Returns the total number of frames that have passed.
		/// </summary>
		public virtual int frameCount {
			get {
				UpdateState();
				return m_State.timestamp;
			}
		}

		/// <summary>
		/// If this value is true,this controller don't need to recenter for aligning with HMD.
		/// </summary>
		public virtual bool isAbsRotation {
			get{
				return XDevicePlugin.GetBool(handle,
					XDevicePlugin.kField_IsAbsRotation,
					false
				);
			}
		}

		protected DelayedCall m_VibrationDC;

		[System.Obsolete("Method StartVibration(int,float) has been deprecated. Use StartHaptics(int,int,float) instead.",false)]
		public virtual void StartVibration(int strength,float duration=0.0f) {
			StartHaptics(strength,0,duration);
		}

		[System.Obsolete("Method StopVibration() has been deprecated. Use StopHaptics() instead.",false)]
		public virtual void StopVibration() {
			StopHaptics();
		}

		public virtual void StartHaptics(string type,float duration=0.0f) {
			HapticsLibrary.HapticsDesc desc=HapticsLibrary.GetDesc(type);
			if(desc!=null) {
				StartHaptics(desc.intValue0,desc.intValue1,duration);
			}else {
				StartHaptics(0,0,duration);
			}
		}

		public virtual void StartHaptics(int strength,int frequency,float duration=0.0f) {
			DelayedCall.Free(ref m_VibrationDC);
			//
			strength=Mathf.Clamp(strength,0,100);//[0,100],0 -> Default.
			XDevicePlugin.SendMessage(handle,XDevicePlugin.kMessage_TriggerVibration,
				(int)((strength<=0?20:strength)|((frequency<<16)&0xFFFF0000)),
				(int)(duration*1000)
			);
			if(duration>0.0f) {
				m_VibrationDC=Juggler.Main.DelayCall(StopHaptics,duration);
			}
		}

		public virtual void StopHaptics() {
			DelayedCall.Free(ref m_VibrationDC);
			//
			XDevicePlugin.SendMessage(handle,XDevicePlugin.kMessage_TriggerVibration,-1,0);
		}

		public virtual void Recenter() {
			//
			XDevicePlugin.SendMessage(handle,XDevicePlugin.kMessage_RecenterSensor,0,0);
		}

        /// <summary>
        /// Returns the value of the virtual axis identified by axisIndex.
        /// </summary>
        /// <param name="axisIndex">Passing in a ControllerRawAxis to get axis value.</param>
        /// <example> 
        /// This example shows how to use the <see cref="GetAxis"/> method.
        /// <code>
        /// using UnityEngine;
        /// using Ximmerse;
        /// using Ximmerse.InputSystem;
        /// 
        /// class TestClass : MonoBehaviour
        /// {
        ///     void Update() 
        ///     {
        ///        ControllerInput leftController = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
        ///        // Trigger value
        ///        float triggerValue = leftController.GetAxis(ControllerRawAxis.LeftTrigger);  
        ///        // Touch pad x axis value
        ///        float xAxis = leftController.GetAxis(ControllerRawAxis.LeftThumbX);
        ///        // Touch pad y axix value
        ///        float yAxis = leftController.GetAxis(ControllerRawAxis.LeftThumbY);
        ///     }
        /// }
        /// </code>
        /// </example>
		public virtual float GetAxis(ControllerRawAxis axisIndex){return GetAxis((int)axisIndex);}


        /// <summary>
        /// Returns a bool to indicate if the button is held down or not.
        /// </summary>
        /// <param name="buttonMask">Passing in a ControllerRawButton to get button value</param>
        /// <example> 
        /// This example shows how to use the <see cref="GetButton"/> method.
        /// <code>
        /// using UnityEngine;
        /// using Ximmerse;
        /// using Ximmerse.InputSystem;
        /// 
        /// class TestClass : MonoBehaviour
        /// {
        ///     void Update() 
        ///     {
        ///        ControllerInput leftController = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
        ///        bool isHeldDown = leftController.GetButton(ControllerRawButton.Start);
        ///        Debug.Log("isHeldDown = " +  isHeldDown);
        ///     }
        /// }
        /// </code>
        /// </example>
		public virtual bool GetButton(ControllerRawButton buttonMask){return GetButton((uint)buttonMask);}


        /// <summary>
        /// Returns a bool to indicate if there is a button down event. 
        /// </summary>
        /// <param name="buttonMask">Passing in a ControllerRawButton to get button value</param>
        /// <example> 
        /// This example shows how to use the <see cref="GetButtonDown"/> method.
        /// <code>
        /// using UnityEngine;
        /// using Ximmerse;
        /// using Ximmerse.InputSystem;
        /// 
        /// class TestClass : MonoBehaviour
        /// {
        ///     void Update() 
        ///     {
        ///        ControllerInput leftController = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
        ///        bool isButtonDown = leftController.GetButtonDown(ControllerRawButton.Start);
        ///        Debug.Log("isButtonDown = " +  isButtonDown);
        ///     }
        /// }
        /// </code>
        /// </example>
		public virtual bool GetButtonDown(ControllerRawButton buttonMask){return GetButtonDown((uint)buttonMask);}

        /// <summary>
        /// Returns a bool to indicate if there is a button up event. 
        /// </summary>
        /// <param name="buttonMask">Passing in a ControllerRawButton to get button value</param>
        /// <example> 
        /// This example shows how to use the <see cref="GetButtonUp"/> method.
        /// <code>
        /// using UnityEngine;
        /// using Ximmerse;
        /// using Ximmerse.InputSystem;
        /// 
        /// class TestClass : MonoBehaviour
        /// {
        ///     void Update() 
        ///     {
        ///        ControllerInput leftController = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
        ///        bool isButtonUp = leftController.GetButtonUp(ControllerRawButton.Start);
        ///        Debug.Log("isButtonUp = " +  isButtonUp);
        ///     }
        /// }
        /// </code>
        /// </example>
		public virtual bool GetButtonUp(ControllerRawButton buttonMask){return GetButtonUp((uint)buttonMask);}

		public virtual float GetAxis(ControllerAxis axisIndex){return GetAxis((int)axisIndex);}

		public virtual bool GetButton(ControllerButton buttonMask){return GetButton((uint)buttonMask);}

		public virtual bool GetButtonDown(ControllerButton buttonMask){return GetButtonDown((uint)buttonMask);}

		public virtual bool GetButtonUp(ControllerButton buttonMask){return GetButtonUp((uint)buttonMask);}
		
		#endregion Methods

	}
}
