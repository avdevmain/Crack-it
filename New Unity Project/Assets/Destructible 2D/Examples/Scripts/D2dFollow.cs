using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component causes the current GameObject to follow the target Transform.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dFollow")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Follow")]
	public class D2dFollow : MonoBehaviour
	{
		[Tooltip("The target object you want this GameObject to follow")]
		public Transform Target;

		public void UpdatePosition()
		{
			if (Target != null)
			{
				var position = transform.position;

				position.x = Target.position.x;
				position.y = Target.position.y;

				transform.position = position;
			}
		}

		protected virtual void Update()
		{
			UpdatePosition();
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dFollow))]
	public class D2dFollow_Editor : D2dEditor<D2dFollow>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Target == null));
				Draw("Target");
			EndError();
		}
	}
}
#endif