using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component puts the gallery gun sprite at the bottom of the screen and moves it based on the mouse position.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dGalleryGun")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Gallery Gun")]
	public class D2dGalleryGun : MonoBehaviour
	{
		/// <summary>How much the finger/mouse position relates to the gun position.</summary>
		public float MoveScale = 0.25f;

		/// <summary>How quickly the gun moves to its target position.</summary>
		public float MoveSpeed = 5.0f;

		/// <summary>The prefab spawned at the muzzle of the gun when shooting.</summary>
		public GameObject MuzzlePrefab;

		/// <summary>The prefab spawned at the mouse position when shooting.</summary>
		public GameObject BulletPrefab;

		[SerializeField]
		private D2dInputManager inputManager = new D2dInputManager();

		protected virtual void Update()
		{
			// Update input
			inputManager.Update(KeyCode.None);

			// Make sure the camera exists
			var camera = D2dHelper.GetCamera(null);

			if (camera != null)
			{
				// Loop through all non-gui fingers
				foreach (var finger in inputManager.Fingers)
				{
					if (finger.StartedOverGui == false)
					{
						var localPosition = transform.localPosition;
						var targetX       = (finger.PositionA.x - Screen.width  / 2) * MoveScale;
						var targetY       = (finger.PositionA.y - Screen.height / 2) * MoveScale;
						var factor        = D2dHelper.DampenFactor(MoveSpeed, Time.deltaTime);

						localPosition.x = Mathf.Lerp(localPosition.x, targetX, factor);
						localPosition.y = Mathf.Lerp(localPosition.y, targetY, factor);

						transform.localPosition = localPosition;

						// Fire?
						if (finger.Up == true)
						{
							if (MuzzlePrefab != null)
							{
								Instantiate(MuzzlePrefab, transform.position, Quaternion.identity);
							}

							if (BulletPrefab != null)
							{
								var position = camera.ScreenToWorldPoint(finger.PositionA);

								Instantiate(BulletPrefab, position, Quaternion.identity);
							}
						}

						// Skip other fingers
						break;
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dGalleryGun))]
	public class D2dGalleryGun_Editor : D2dEditor<D2dGalleryGun>
	{
		protected override void OnInspector()
		{
			Draw("MoveScale", "How much the finger/mouse position relates to the gun position.");
			Draw("MoveSpeed", "How quickly the gun moves to its target position.");
			Draw("MuzzlePrefab", "The prefab spawned at the muzzle of the gun when shooting.");
			Draw("BulletPrefab", "The prefab spawned at the mouse position when shooting.");
		}
	}
}
#endif