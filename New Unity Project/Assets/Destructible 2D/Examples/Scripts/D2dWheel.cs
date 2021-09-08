using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component implements very basic physics for a 2D wheel in a top-down perspective.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dWheel")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Wheel")]
	public class D2dWheel : MonoBehaviour
	{
		[Tooltip("The current rotational speed of the wheel")]
		public float Speed;

		[Tooltip("How quickly the wheel matches the ground speed")]
		public float GripDampening = 1.0f;

		[Tooltip("How quickly the wheels slow down")]
		public float Friction = 0.1f;
		
		// Has oldPosition been set?
		[SerializeField]
		private bool oldPositionSet;

		// Stores the old position
		[SerializeField]
		private Vector2 oldPosition;

		// The rigidbody this wheel is attached to
		[System.NonSerialized]
		private Rigidbody2D cachedRigidbody2D;

		[System.NonSerialized]
		private float remainingAngularVelocity;

		public void AddTorque(float amount)
		{
			Speed += amount;
		}

		protected virtual void FixedUpdate()
		{
			if (cachedRigidbody2D == null) cachedRigidbody2D = GetComponentInParent<Rigidbody2D>();

			if (cachedRigidbody2D != null)
			{
				if (oldPositionSet == false)
				{
					oldPositionSet = true;
					oldPosition    = transform.position;
				}

				var newPosition   = (Vector2)transform.position;
				var deltaPosition = newPosition - oldPosition;
				var deltaSpeed    = deltaPosition.magnitude / Time.fixedDeltaTime;
				var expectedSpeed = deltaSpeed * Vector2.Dot(transform.up, cachedRigidbody2D.transform.up);

				oldPosition = newPosition;

				// Match ground speed
				Speed = Dampen(Speed, expectedSpeed, GripDampening, Time.fixedDeltaTime);

				// Apply speed difference
				var deltaWheel         = (Vector2)transform.up * Speed * Time.fixedDeltaTime;
				var oldAngularVelocity = cachedRigidbody2D.angularVelocity;

				cachedRigidbody2D.AddForceAtPosition(deltaWheel - deltaPosition, transform.position, ForceMode2D.Impulse);

				remainingAngularVelocity += cachedRigidbody2D.angularVelocity - oldAngularVelocity;

				cachedRigidbody2D.angularVelocity = oldAngularVelocity;

				// Slow wheel down
				Speed = Dampen(Speed, 0.0f, Friction, Time.fixedDeltaTime);
			}
		}

		protected virtual void Update()
		{
			if (cachedRigidbody2D != null)
			{
				cachedRigidbody2D.angularVelocity += remainingAngularVelocity;

				remainingAngularVelocity = 0;
			}
		}

		private static float Dampen(float current, float target, float dampening, float elapsed, float minStep = 0.0f)
		{
			var factor   = D2dHelper.DampenFactor(dampening, elapsed);
			var maxDelta = Mathf.Abs(target - current) * factor + minStep * elapsed;
			
			return Mathf.MoveTowards(current, target, maxDelta);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dWheel))]
	public class D2dWheel_Editor : D2dEditor<D2dWheel>
	{
		protected override void OnInspector()
		{
			Draw("Speed");
			Draw("GripDampening");
			Draw("Friction");
		}
	}
}
#endif