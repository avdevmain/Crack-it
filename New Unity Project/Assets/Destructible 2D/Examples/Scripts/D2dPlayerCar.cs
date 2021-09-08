using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current Rigidbody2D into a car that can be controlled with the <b>Horizontal</b> and <b>Vertical</b> input axes.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dPlayerCar")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Player Car")]
	public class D2dPlayerCar : MonoBehaviour
	{
		/// <summary>The wheels used to steer this car.</summary>
		public D2dWheel[] SteerWheels;

		/// <summary>The maximum +- angle of turning.</summary>
		public float SteerAngleMax = 20.0f;

		/// <summary>How quickly the steering wheels turn to their target angle.</summary>
		public float SteerAngleDampening = 5.0f;

		/// <summary>The wheels used to move this car.</summary>
		public D2dWheel[] DriveWheels;

		/// <summary>The maximum torque that can be applied to each drive wheel.</summary>
		public float DriveTorque = 1.0f;

		/// <summary>How quickly the drive wheels get to their target torque.</summary>
		public float DriveDampening = 5.0f;

		// Current steering angle
		[SerializeField]
		private float currentSteer;

		[SerializeField]
		private float currentDrive;

		[SerializeField]
		private D2dInputManager inputManager = new D2dInputManager();

		protected virtual void Update()
		{
			// Update input
			inputManager.Update(KeyCode.None);

			// Calculate control values from fingers/mouse
			var delta       = inputManager.GetAveragePullScaled(true);
			var targetSteer = Mathf.Clamp(delta.x / 100.0f, -1.0f, 1.0f) * SteerAngleMax;
			var targetDrive = Mathf.Clamp(delta.y / 100.0f, -1.0f, 1.0f) * DriveTorque;

			// Smooth to target values
			var steerFactor = D2dHelper.DampenFactor(SteerAngleDampening, Time.deltaTime);
			var driveFactor = D2dHelper.DampenFactor(     DriveDampening, Time.deltaTime);

			currentSteer = Mathf.Lerp(currentSteer, targetSteer, steerFactor);
			currentDrive = Mathf.Lerp(currentDrive, targetDrive, driveFactor);

			// Apply steering
			for (var i = 0; i < SteerWheels.Length; i++)
			{
				SteerWheels[i].transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -currentSteer);
			}
		}

		protected virtual void FixedUpdate()
		{
			// Apply drive
			for (var i = 0; i < DriveWheels.Length; i++)
			{
				DriveWheels[i].AddTorque(currentDrive * Time.fixedDeltaTime);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dPlayerCar))]
	public class D2dPlayerCar_Editor : D2dEditor<D2dPlayerCar>
	{
		protected override void OnInspector()
		{
			Draw("SteerWheels", "The wheels used to steer this car.");
			Draw("SteerAngleMax", "The maximum +- angle of turning.");
			Draw("SteerAngleDampening", "How quickly the steering wheels turn to their target angle.");

			Separator();

			Draw("DriveWheels", "The wheels used to move this car.");
			Draw("DriveTorque", "The maximum torque that can be applied to each drive wheel.");
			Draw("DriveDampening", "How quickly the drive wheels get to their target torque.");
		}
	}
}
#endif