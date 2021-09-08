using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to slice all destructible sprites between the mouse down and mouse up points.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dDragToSlice")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Drag To Slice")]
	public class D2dDragToSlice : MonoBehaviour
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
		public KeyCode Requires;

		/// <summary>The Z position in world space this component will use. For normal 2D scenes this should be 0.</summary>
		public float Intercept;

		/// <summary>The destructible sprite layers we want to slice.</summary>
		public LayerMask Layers = -1;

		/// <summary>The prefab used to show what the slice will look like.</summary>
		public GameObject IndicatorPrefab;

		/// <summary>This allows you to change the painting type.</summary>
		public D2dDestructible.PaintType Paint;

		/// <summary>The shape of the slice.</summary>
		public Texture2D Shape;

		/// <summary>The stamp shape will be multiplied by this.
		/// White = No Change.</summary>
		public Color Color = Color.white;

		/// <summary>The thickness of the slice line in world space.</summary>
		public float Thickness = 1.0f;

		[SerializeField]
		private D2dInputManager inputManager = new D2dInputManager();

		[SerializeField]
		private List<Link> links = new List<Link>();

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
							var scale = Vector3.Distance(position, link.Start);
							var angle = D2dHelper.Atan2(position - link.Start) * Mathf.Rad2Deg;

							link.Visual.transform.position   = link.Start;
							link.Visual.transform.rotation   = Quaternion.Euler(0.0f, 0.0f, -angle);
							link.Visual.transform.localScale = new Vector3(Thickness, scale, scale);
						}

						// Slice scene then clear link?
						if (finger.Up == true)
						{
							D2dSlice.All(Paint, link.Start, position, Thickness, Shape, Color, Layers);

							link.Clear();
						}
					}
				}
			}
		}

		protected virtual void OnDestroy()
		{
			D2dInputManager.Link.ClearAll(links);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dDragToSlice))]
	public class D2dDragToSlice_Editor : D2dEditor<D2dDragToSlice>
	{
		protected override void OnInspector()
		{
			Draw("Requires", "The key you must hold down to activate this component on desktop platforms.\n\nNone = Any mouse button.");
			Draw("Intercept", "The Z position in world space this component will use. For normal 2D scenes this should be 0.");
			BeginError(Any(t => t.Layers == 0));
				Draw("Layers", "The destructible sprite layers we want to slice.");
			EndError();
			BeginError(Any(t => t.IndicatorPrefab == null));
				Draw("IndicatorPrefab", "The prefab used to show what the slice will look like.");
			EndError();

			Separator();

			Draw("Paint", "This allows you to change the painting type.");
			BeginError(Any(t => t.Shape == null));
				Draw("Shape", "The shape of the slice.");
			EndError();
			Draw("Color", "The stamp shape will be multiplied by this.\n\nWhite = No Change.");
			BeginError(Any(t => t.Thickness == 0.0f));
				Draw("Thickness", "The thickness of the slice line in world space.");
			EndError();
		}
	}
}
#endif