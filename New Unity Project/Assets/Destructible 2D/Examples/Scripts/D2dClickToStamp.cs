using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to stamp all destructible sprites under the mouse.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dClickToStamp")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Stamp")]
	public class D2dClickToStamp : MonoBehaviour
	{
		public enum HitType
		{
			All,
			First
		}

		class Link : D2dInputManager.Link
		{
			public GameObject Visual;

			public Vector3 Scale;

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

		/// <summary>The destructible sprite layers we want to stamp.</summary>
		public LayerMask Layers = -1;

		/// <summary>The prefab used to show what the stamp will look like.</summary>
		public GameObject IndicatorPrefab;

		/// <summary>This allows you to change the painting type.</summary>
		public D2dDestructible.PaintType Paint;

		/// <summary>The shape of the stamp.</summary>
		public Texture2D Shape;

		/// <summary>The stamp shape will be multiplied by this.
		/// nWhite = No Change.</summary>
		public Color Color = Color.white;

		/// <summary>The size of the stamp in world space.</summary>
		public Vector2 Size = Vector2.one;

		/// <summary>The angle of the stamp in degrees.</summary>
		public float Angle;

		/// <summary>How many destructibles should be hit?</summary>
		public HitType Hit;

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
						if (finger.Down == true && IndicatorPrefab != null)
						{
							link.Visual = Instantiate(IndicatorPrefab);
							link.Scale  = link.Visual.transform.localScale;

							link.Visual.SetActive(true);
						}

						// Update indicator?
						if (finger.Set == true && link.Visual != null)
						{
							link.Visual.transform.position = position;

							link.Visual.transform.localScale = Vector3.Scale(link.Scale, new Vector3(Size.x, Size.y, 1.0f));
						}

						// Clear indicator then stamp?
						if (finger.Up == true)
						{
							// Stamp everything at this point?
							if (Hit == HitType.All)
							{
								D2dStamp.All(Paint, position, Size, Angle, Shape, Color, Layers);
							}

							// Stamp the first thing at this point?
							if (Hit == HitType.First)
							{
								var destructible = default(D2dDestructible);

								if (D2dDestructible.TrySampleThrough(position, ref destructible) == true)
								{
									destructible.Paint(Paint, D2dStamp.CalculateMatrix(position, Size, Angle), Shape, Color);
								}
							}

							// Destroy indicator
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
	[CustomEditor(typeof(D2dClickToStamp))]
	public class D2dClickToStamp_Editor : D2dEditor<D2dClickToStamp>
	{
		protected override void OnInspector()
		{
			Draw("Requires", "The key you must hold down to activate this component on desktop platforms.\n\nNone = Any mouse button.");
			Draw("Intercept", "The Z position in world space this component will use. For normal 2D scenes this should be 0.");
			BeginError(Any(t => t.Layers == 0));
				Draw("Layers", "The destructible sprite layers we want to stamp.");
			EndError();
			BeginError(Any(t => t.IndicatorPrefab == null));
				Draw("IndicatorPrefab", "The prefab used to show what the stamp will look like.");
			EndError();

			Separator();

			Draw("Paint", "This allows you to change the painting type.");
			BeginError(Any(t => t.Shape == null));
				Draw("Shape", "The shape of the stamp.");
			EndError();
			Draw("Color", "The stamp shape will be multiplied by this.\n\nWhite = No Change.");
			BeginError(Any(t => t.Size.x == 0.0f || t.Size.y == 0.0f));
				Draw("Size", "The size of the stamp in world space.");
			EndError();
			Draw("Angle", "The angle of the stamp in degrees.");

			Separator();

			Draw("Hit", "How many destructibles should be hit?");
		}
	}
}
#endif