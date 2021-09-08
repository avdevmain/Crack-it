using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to fracture a destructible sprite under the mouse.</summary>
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Fracture")]
	public class D2dClickToFracture : MonoBehaviour
	{
		public enum HitType
		{
			All,
			First
		}

		[Tooltip("The key you must hold down to perform a fracture.")]
		public KeyCode Requires = KeyCode.Mouse0;

		[Tooltip("The destructible sprite layers we can click.")]
		public LayerMask Layers = -1;

		[Tooltip("How many destructibles should be hit?")]
		public HitType Hit;

		[Tooltip("The camera used to calculate the ray.\n\nNone = MainCamera.")]
		public Camera Camera;

		[Tooltip("The prefab that gets spawned under the mouse when clicking.")]
		public GameObject Prefab;

		[Tooltip("Only fracture GameObjects that have the D2dFracturer component?")]
		public bool RequireFracturer = true;

		[Tooltip("This lets you set how many fracture points there can be based on the amount of solid pixels.")]
		public float PointsPerSolidPixel = 0.001f;

		[Tooltip("This lets you limit how many points the fracture can use.")]
		public int MaxPoints = 10;

		[Tooltip("Automatically multiply the points by the D2dDestructible.AlphaSharpness value to account for optimizations?")]
		public bool FactorInSharpness = true;

		[Tooltip("Fracturing can cause pixel islands to appear, should a split be triggered on each fractured part to check for these?")]
		public bool SplitAfterFracture;

		[Tooltip("This allows you to set the Feather value used when splitting.")]
		public int SplitFeather = 3;

		[Tooltip("This allows you to set the HealThreshold value used when splitting.")]
		public int SplitHealThreshold = -1;

		private static RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1024];

		protected virtual void Update()
		{
			if (Input.GetKeyDown(Requires) == true)
			{
				var camera = D2dHelper.GetCamera(Camera, gameObject);

				if (camera != null)
				{
					var ray   = camera.ScreenPointToRay(Input.mousePosition);
					var count = Physics2D.GetRayIntersectionNonAlloc(ray, raycastHit2Ds, float.PositiveInfinity, Layers);

					if (count > 0)
					{
						for (var i = 0; i < count; i++)
						{
							var raycastHit2D = raycastHit2Ds[i];
							
							if (TryFracture(raycastHit2D) == true)
							{
								// Spawn prefab?
								if (Prefab != null)
								{
									var clone = Instantiate(Prefab, raycastHit2D.point, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

									clone.SetActive(true);
								}

								if (Hit == HitType.First)
								{
									break;
								}
							}
						}
					}
				}
			}
		}

		private bool TryFracture(RaycastHit2D hit)
		{
			if (RequireFracturer == true)
			{
				var fracturer = hit.transform.GetComponentInParent<D2dFracturer>();

				if (fracturer != null && fracturer.enabled == true && fracturer.TryFracture() == true)
				{
					return true;
				}
			}
			else
			{
				var destructible = hit.transform.GetComponentInParent<D2dDestructible>();

				if (destructible != null)
				{
					var points = D2dFracturer.CalculatePointCount(destructible, PointsPerSolidPixel, FactorInSharpness, MaxPoints);

					if (D2dFracturer.TryFracture(destructible, points, SplitAfterFracture, SplitFeather, SplitHealThreshold) == true)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dClickToFracture))]
	public class D2dClickToFracture_Editor : D2dEditor<D2dClickToFracture>
	{
		protected override void OnInspector()
		{
			Draw("Requires");
			BeginError(Any(t => t.Layers == 0));
				Draw("Layers");
			EndError();
			Draw("Hit");
			Draw("Camera");
			Draw("Prefab");

			Separator();

			Draw("RequireFracturer");

			if (Any(t => t.RequireFracturer == false))
			{
				Draw("PointsPerSolidPixel");
				Draw("MaxPoints");
				Draw("FactorInSharpness");
				Draw("SplitAfterFracture");

				if (Any(t => t.SplitAfterFracture == true))
				{
					BeginIndent();
						Draw("SplitFeather");
						Draw("SplitHealThreshold");
					EndIndent();
				}
			}
		}
	}
}
#endif