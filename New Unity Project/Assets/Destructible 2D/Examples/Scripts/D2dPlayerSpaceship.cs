using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current Rigidbody2D into a spaceship that can be controlled with the <b>Horizontal</b> and <b>Vertical</b> and <b>Jump</b> input axes.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dPlayerSpaceship")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Player Spaceship")]
	public class D2dPlayerSpaceship : MonoBehaviour
	{
		/// <summary>Minimum time between each shot in seconds.</summary>
		public float ShootDelay = 0.1f;

		/// <summary>The left gun.</summary>
		public D2dGun LeftGun;

		/// <summary>The right gun.</summary>
		public D2dGun RightGun;

		/// <summary>The left thruster.</summary>
		public D2dThruster LeftThruster;
		
		/// <summary>The right thruster.</summary>
		public D2dThruster RightThruster;
		
		// Cached rigidbody of this spaceship
		[System.NonSerialized]
		private Rigidbody2D body;

		[SerializeField]
		private int burstRemaining;
		
		// Seconds until next shot is available
		[SerializeField]
		private float cooldown;

		[SerializeField]
		private D2dInputManager inputManager = new D2dInputManager();

		protected virtual void Update()
		{
			// Update input
			inputManager.Update(KeyCode.None);

			cooldown -= Time.deltaTime;

			// Tap to begin burst of 5 shots?
			foreach (var finger in inputManager.Fingers)
			{
				if (finger.StartedOverGui == false && finger.Tap == true)
				{
					burstRemaining = 5;

					// Skip other fingers
					break;
				}
			}

			// Can we shoot again in this burst?
			if (burstRemaining > 0 && cooldown <= 0.0f)
			{
				cooldown        = ShootDelay;
				burstRemaining -= 1;

				// Shoot left gun?
				if (LeftGun != null && LeftGun.CanShoot == true)
				{
					LeftGun.Shoot();
				}
				// Shoot right gun?
				else if (RightGun != null && RightGun.CanShoot == true)
				{
					RightGun.Shoot();
				}
			}

			// Set thrusters based on finger drag
			var delta = inputManager.GetAveragePullScaled(true);

			delta.x = Mathf.Clamp(delta.x / 100.0f, -1.0f, 1.0f);
			delta.y = Mathf.Clamp(delta.y / 100.0f, -1.0f, 1.0f);
			
			if (LeftThruster != null)
			{
				LeftThruster.Throttle = delta.y + delta.x * 0.5f;
			}

			if (RightThruster != null)
			{
				RightThruster.Throttle = delta.y - delta.x * 0.5f;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dPlayerSpaceship))]
	public class D2dPlayerSpaceship_Editor : D2dEditor<D2dPlayerSpaceship>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.ShootDelay < 0.0f));
				Draw("ShootDelay", "Minimum time between each shot in seconds.");
			EndError();
			Draw("LeftGun", "The left gun.");
			Draw("RightGun", "The right gun.");
			Draw("LeftThruster", "The left thruster.");
			Draw("RightThruster", "The right thruster.");
		}
	}
}
#endif