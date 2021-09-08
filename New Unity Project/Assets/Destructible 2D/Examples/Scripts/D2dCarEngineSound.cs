using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component implements very basic physics for a 2D wheel in a top-down perspective.</summary>
	[RequireComponent(typeof(AudioSource))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dCarEngineSound")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Car Engine Sound")]
	public class D2dCarEngineSound : MonoBehaviour
	{
		[Tooltip("The wheel whose speed we will use for pitch.")]
		public D2dWheel Wheel;

		[Tooltip("The sound pitch when not moving.")]
		public float IdlePitch = 1.0f;

		[Tooltip("The sound pitch shift for each given speed value.")]
		public float SpeedPitch = 0.05f;

		[System.NonSerialized]
		private AudioSource cachedAudioSource;

		protected virtual void OnEnable()
		{
			if (cachedAudioSource == null) cachedAudioSource = GetComponent<AudioSource>();
		}

		protected virtual void Update()
		{
			if (Wheel != null)
			{
				cachedAudioSource.pitch = IdlePitch + SpeedPitch * Mathf.Abs(Wheel.Speed);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dCarEngineSound))]
	public class D2dCarEngineSound_Editor : D2dEditor<D2dCarEngineSound>
	{
		protected override void OnInspector()
		{
			Draw("Wheel");
			Draw("IdlePitch");
			Draw("SpeedPitch");
		}
	}
}
#endif