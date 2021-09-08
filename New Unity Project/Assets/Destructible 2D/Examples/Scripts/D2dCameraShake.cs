using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component adds camera shake to the current GameObject.
	/// NOTE: This component takes over the current Transform.localPosition, so you must nest it under another GameObject if you want to move it normally.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dCameraShake")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Camera Shake")]
	public class D2dCameraShake : MonoBehaviour
	{
		/// <summary>All active and enabled D2dCameraShake instances in the scene.</summary>
		public static List<D2dCameraShake> Instances = new List<D2dCameraShake>();

		[Tooltip("The current shake strength. This gets reduced automatically")]
		public float Shake;

		[Tooltip("The speed at which the Shake value gets reduced")]
		public float ShakeDampening = 10.0f;

		[Tooltip("The amount this camera shakes relative to the Shake value")]
		public float ShakeScale = 1.0f;

		[Tooltip("The freqncy of the camera shake")]
		public float ShakeSpeed = 10.0f;

		// Used to seed/offset the perlin lookup
		[SerializeField]
		private float offsetX;

		[SerializeField]
		private float offsetY;

		protected virtual void Awake()
		{
			offsetX = Random.Range(-1000.0f, 1000.0f);
			offsetY = Random.Range(-1000.0f, 1000.0f);
		}

		protected virtual void OnEnable()
		{
			Instances.Add(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
		}

		protected virtual void LateUpdate()
		{
			var factor = D2dHelper.DampenFactor(ShakeDampening, Time.deltaTime, 0.1f);

			Shake = Mathf.Lerp(Shake, 0.0f, factor);

			var shakeStrength = Shake * ShakeScale;
			var shakeTime     = Time.time * ShakeSpeed;
			var localPosition = transform.localPosition;

			localPosition.x = Mathf.PerlinNoise(offsetX, shakeTime) * shakeStrength;
			localPosition.y = Mathf.PerlinNoise(offsetY, shakeTime) * shakeStrength;

			transform.localPosition = localPosition;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dCameraShake))]
	public class D2dCameraShake_Editor : D2dEditor<D2dCameraShake>
	{
		protected override void OnInspector()
		{
			Draw("Shake");
			Draw("ShakeDampening");
			Draw("ShakeScale");
			Draw("ShakeSpeed");
		}
	}
}
#endif