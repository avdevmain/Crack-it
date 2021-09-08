using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component automatically adds shake to the <b>D2dCameraShake</b> component.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dShake")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Shake")]
	public class D2dShake : MonoBehaviour
	{
		[Tooltip("The amount of shake this applies to the D2dCameraShake component")]
		public float Shake;

		protected virtual void Awake()
		{
			for (var i = D2dCameraShake.Instances.Count - 1; i >= 0; i--)
			{
				D2dCameraShake.Instances[i].Shake += Shake;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dShake))]
	public class D2dShake_Editor : D2dEditor<D2dShake>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Shake <= 0.0f));
				Draw("Shake");
			EndError();
		}
	}
}
#endif