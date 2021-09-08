using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current GameObject into a thruster that can apply forces to its parent <b>Rigidbody2D</b>.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dThruster")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Thruster")]
	public class D2dThruster : MonoBehaviour
	{
		[Tooltip("The current thottle amount")]
		public float Throttle;

		[Tooltip("The scale of this thruster when throttle is 1")]
		public Vector3 MaxScale = Vector3.one;
		
		[Tooltip("How quickly the throttle scales to the desired value")]
		public float Dampening = 10.0f;

		[Tooltip("The amount of force applied to the rigidbody2D when throttle is 1")]
		public float MaxForce = 1.0f;

		[Tooltip("The amount the thruster effect can flicker")]
		public float Flicker = 0.1f;

		// The rigidbody this thruster is attached to
		[System.NonSerialized]
		private Rigidbody2D body;

		// The current interpolated throttle value
		[SerializeField]
		private float currentThrottle;

		protected virtual void FixedUpdate()
		{
			if (body == null) body = GetComponentInParent<Rigidbody2D>();

			if (body != null)
			{
				body.AddForceAtPosition(transform.up * MaxForce * -Throttle, transform.position, ForceMode2D.Force);
			}
		}

		protected virtual void Update()
		{
			var factor = D2dHelper.DampenFactor(Dampening, Time.deltaTime);

			currentThrottle = Mathf.Lerp(currentThrottle, Throttle, factor);
			
			transform.localScale = MaxScale * Random.Range(1.0f - Flicker, 1.0f + Flicker) * currentThrottle;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dThruster))]
	public class D2dThruster_Editor : D2dEditor<D2dThruster>
	{
		protected override void OnInspector()
		{
			Draw("Throttle");
			Draw("MaxScale");
			BeginError(Any(t => t.Dampening < 0.0f));
				Draw("Dampening");
			EndError();
			Draw("MaxForce");
			Draw("Flicker");
		}
	}
}
#endif