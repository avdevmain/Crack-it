using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component will automatically update the event system if you switch to using the new <b>InputSystem</b>.</summary>
	[ExecuteInEditMode]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Upgrade EventSystem")]
	public class D2dUpgradeEventSystem : MonoBehaviour
	{
#if UNITY_EDITOR && ENABLE_INPUT_SYSTEM
		protected virtual void Awake()
		{
			var module = FindObjectOfType<UnityEngine.EventSystems.StandaloneInputModule>();

			if (module != null)
			{
				Debug.Log("Replacing old StandaloneInputModule with new InputSystemUIInputModule.", module.gameObject);

				module.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

				DestroyImmediate(module);
			}
		}
#endif
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dUpgradeEventSystem))]
	public class D2dUpgradeEventSystem_Editor : D2dEditor<D2dUpgradeEventSystem>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component will automatically update the event system if you switch to using the new InputSystem.", MessageType.Info);
		}
	}
}
#endif