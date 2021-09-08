using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component can be used to slice between two points and spawn particles between the slice points.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dSlicer")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Slicer")]
	public class D2dSlicer : MonoBehaviour
	{
		[Tooltip("The slice will start from this local point.")]
		public Vector3 LocalStart;

		[Tooltip("The slice will end at this local point.")]
		public Vector3 LocalEnd = Vector3.up;

		[Tooltip("If you want to continuously slice then set the slice interval here in seconds.\n-1 = Manual\n0 = Start")]
		public float Interval;

		[Tooltip("The paint type used when stamping.")]
		public D2dDestructible.PaintType Paint;

		[Tooltip("The texture used for the stamping.")]
		public Texture2D Shape;

		[Tooltip("The thickness of the slice in world space.")]
		public float Thickness = 0.25f;

		[Tooltip("The color tint of the slice.")]
		public Color Color = Color.white;

		[Tooltip("The layers that will be sliced.")]
		public LayerMask Layers = -1;

		[Tooltip("If you want particles to be spawned along the slice line, specify the particle system here.")]
		public ParticleSystem ParticleSystem;

		[Tooltip("The amount of particles that will be spawned per world unit length of the slice line.")]
		public float ParticlesPerUnit = 10.0f;

		[Tooltip("If youw ant the particles to spawn at random points along the line, enable this.")]
		public bool ParticlesRandom = true;

		[Tooltip("The amount of outward force that will be added to sliced objects.")]
		public float Force = 10.0f;

		[Tooltip("The type of force that will be added.")]
		public ForceMode2D ForceMode;

		private float cooldown;

		public void Slice()
		{
			var positionA = transform.TransformPoint(LocalStart);
			var positionB = transform.TransformPoint(LocalEnd  );

			D2dSlice.All(Paint, positionA, positionB, Thickness, Shape, Color, Layers);

			// The slice won't happen until next frame, so delay the force application
			if (Force != 0.0f)
			{
				StartCoroutine(DelayedForce(positionA, positionB));
			}

			if (ParticleSystem != null && ParticlesPerUnit > 0.0f)
			{
				var particleCount = Mathf.CeilToInt(Vector3.Distance(positionA, positionB) * ParticlesPerUnit);

				if (particleCount > 0.0f)
				{
					var emitParams = new ParticleSystem.EmitParams();
					var positionD  = positionB - positionA;

					if (ParticlesRandom == true)
					{
						for (var i = 0; i < particleCount; i++)
						{
							emitParams.position = positionA + positionD * Random.value;
							emitParams.velocity = Random.insideUnitSphere;

							ParticleSystem.Emit(emitParams, 1);
						}
					}
					else
					{
						var step = positionD / particleCount;

						emitParams.position = positionA + step * 0.5f;

						for (var i = 0; i < particleCount; i++)
						{
							emitParams.velocity = Random.insideUnitSphere;

							ParticleSystem.Emit(emitParams, 1);

							emitParams.position += step;
						}
					}
				}
			}
		}

		/// <summary>This will transform the current slicer between the input positions.</summary>
		public void SetTransform(Vector2 positionA, Vector2 positionB)
		{
			var scale = Vector2.Distance(positionB, positionA);
			var angle = D2dHelper.Atan2(positionB - positionA) * Mathf.Rad2Deg;

			// Transform the indicator so it lines up with the slice
			transform.position   = positionA;
			transform.rotation   = Quaternion.Euler(0.0f, 0.0f, -angle);
			transform.localScale = new Vector3(Thickness, scale, scale);
		}

		protected virtual void Start()
		{
			if (Interval == 0)
			{
				Slice();
			}
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;

			if (Interval > 0.0f)
			{
				if (cooldown <= 0.0f)
				{
					cooldown = Interval;

					Slice();
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			var pointA = transform.TransformPoint(LocalStart);
			var pointB = transform.TransformPoint(LocalEnd  );

			Gizmos.DrawWireSphere(pointA, HandleUtility.GetHandleSize(pointA) * 0.25f);
			Gizmos.DrawWireSphere(pointB, HandleUtility.GetHandleSize(pointB) * 0.25f);

			Gizmos.DrawLine(pointA, pointB);
		}
#endif

		private IEnumerator DelayedForce(Vector3 oldPosition, Vector3 newPosition)
		{
			yield return new WaitForEndOfFrame();

			D2dSlice.ForceAll(oldPosition, newPosition, Thickness, Force, ForceMode, Layers);
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dSlicer))]
	public class D2dSlicer_Editor : D2dEditor<D2dSlicer>
	{
		protected override void OnInspector()
		{
			Draw("LocalStart");
			Draw("LocalEnd");
			Draw("Interval");
			Draw("Thickness");
			BeginError(Any(t => t.Layers == 0));
				Draw("Layers");
			EndError();

			Separator();

			Draw("Paint");
			BeginError(Any(t => t.Shape == null));
				Draw("Shape");
			EndError();
			Draw("Color");

			Separator();

			Draw("ParticleSystem");
			BeginError(Any(t => t.ParticlesPerUnit <= 0.0f));
				Draw("ParticlesPerUnit");
			EndError();
			Draw("ParticlesRandom");

			Separator();

			Draw("Force");
			Draw("ForceMode");
		}
	}
}
#endif