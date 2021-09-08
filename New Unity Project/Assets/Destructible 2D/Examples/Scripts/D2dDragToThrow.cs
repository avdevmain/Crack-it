using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Destructible2D.Examples
{
	/// <summary>This component spawns and throws a prefab when you click and drag across the screen.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dDragToThrow")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Drag To Throw")]
	public class D2dDragToThrow : MonoBehaviour
	{
		class Link : D2dInputManager.Link
		{
			public Vector3 Start;

			public GameObject Visual;

			public override void Clear()
			{
				Destroy(Visual);
			}
		}

		/// <summary>The key you must hold down to activate this component on desktop platforms.
		/// None = Any mouse button.</summary>
		public KeyCode Requires = KeyCode.Mouse0;

		/// <summary>The Z position in world space this component will use. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		/// <summary>The prefab used to show what the slice will look like.</summary>
		public GameObject IndicatorPrefab;

		/// <summary>The scale of the throw indicator.</summary>
		public float Scale = 1.0f;

		/// <summary>The minimum distance the throw is calculated using in world space.
		/// 0 = Unlimited.</summary>
		public float DistanceMin;

		/// <summary>The maximum distance the throw is calculated using in world space.
		/// 0 = Unlimited.</summary>
		public float DistanceMax;

		/// <summary>The prefab that gets thrown.</summary>
		public GameObject ProjectilePrefab;

		/// <summary>How fast the projectile will be launched.</summary>
		public float ProjectileSpeed = 10.0f;

		/// <summary>How much spread is added to the project when fired.</summary>
		public float ProjectileSpread;

		/// <summary>The projectile will be rotated by this angle in degrees.</summary>
		public float ProjectileAngle;

		[SerializeField]
		private D2dInputManager inputManager = new D2dInputManager();

		[SerializeField]
		private List<Link> links = new List<Link>();

		private float GetAngleAndClampCurrentPos(Vector3 startPos, ref Vector3 currentPos)
		{
			if (startPos != currentPos)
			{
				var distance = Vector3.Distance(currentPos, startPos);

				if (DistanceMin > 0.0f && distance < DistanceMin)
				{
					distance = DistanceMin;
				}

				if (DistanceMax > 0.0f && distance > DistanceMax)
				{
					distance = DistanceMax;
				}

				currentPos = startPos + (currentPos - startPos).normalized * distance;
			}

			return D2dHelper.Atan2(currentPos - startPos) * Mathf.Rad2Deg;
		}

		protected virtual void Update()
		{
			// Update input
			inputManager.Update(Requires);

			// Make sure the camera exists
			var camera = D2dHelper.GetCamera(null);

			if (camera != null)
			{
				// Loop through all non-gui fingers
				foreach (var finger in inputManager.Fingers)
				{
					if (finger.StartedOverGui == false)
					{
						// Grab extra finger data and position
						var link     = D2dInputManager.Link.FindOrCreate(ref links, finger);
						var position = D2dHelper.ScreenToWorldPosition(finger.PositionA, Intercept, camera);

						// Create indiactor?
						if (finger.Down == true)
						{
							link.Start = position;

							if (IndicatorPrefab != null)
							{
								link.Visual = Instantiate(IndicatorPrefab);

								link.Visual.SetActive(true);
							}
						}

						// Update indicator?
						if (finger.Set == true && link.Visual != null)
						{
							var angle = GetAngleAndClampCurrentPos(link.Start, ref position);
							var scale = Vector3.Distance(position, link.Start) * Scale;

							link.Visual.transform.position   = link.Start;
							link.Visual.transform.rotation   = Quaternion.Euler(0.0f, 0.0f, -angle);
							link.Visual.transform.localScale = new Vector3(scale, scale, scale);
						}

						// Slice scene then clear link?
						if (finger.Up == true)
						{
							var angle = GetAngleAndClampCurrentPos(link.Start, ref position) + ProjectileAngle + Random.Range(-ProjectileSpread, ProjectileSpread);

							// Spawn
							var projectile = Instantiate(ProjectilePrefab, link.Start, Quaternion.Euler(0.0f, 0.0f, -angle));

							projectile.SetActive(true);

							// Apply velocity?
							var rigidbody2D = projectile.GetComponent<Rigidbody2D>();

							if (rigidbody2D != null)
							{
								rigidbody2D.velocity = (position - link.Start) * ProjectileSpeed;
							}

							link.Clear();
						}
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
	[CustomEditor(typeof(D2dDragToThrow))]
	public class D2dDragToThrow_Editor : D2dEditor<D2dDragToThrow>
	{
		protected override void OnInspector()
		{
			Draw("Requires", "The key you must hold down to activate this component on desktop platforms.\n\nNone = Any mouse button.");
			Draw("Intercept", "The Z position in world space this component will use. For normal 2D scenes this should be 0.");
			BeginError(Any(t => t.IndicatorPrefab == null));
				Draw("IndicatorPrefab", "The prefab used to show what the slice will look like.");
			EndError();
			Draw("Scale", "The scale of the throw indicator.");
			Draw("DistanceMin", "The minimum distance the throw is calculated using in world space.\n\n0 = Unlimited.");
			Draw("DistanceMax", "The maximum distance the throw is calculated using in world space.\n\n0 = Unlimited.");

			Separator();

			BeginError(Any(t => t.ProjectilePrefab == null));
				Draw("ProjectilePrefab", "The prefab that gets thrown.");
			EndError();
			Draw("ProjectileSpeed", "How fast the projectile will be launched.");
			Draw("ProjectileSpread", "How much spread is added to the project when fired.");
			Draw("ProjectileAngle", "The projectile will be rotated by this angle in degrees.");
		}
	}
}
#endif