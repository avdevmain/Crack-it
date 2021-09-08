using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component will automatically spawn the specified prefab when the attached destructible object splits.</summary>
	[RequireComponent(typeof(D2dDestructible))]
	[RequireComponent(typeof(D2dSplitter))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dSplitSpawner")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Split Spawner")]
	public class D2dSplitSpawner : MonoBehaviour
	{
		[Tooltip("The prefab spawned on split")]
		public GameObject Prefab;

		[System.NonSerialized]
		private D2dDestructible cachedDestructible;

		protected virtual void OnEnable()
		{
			if (cachedDestructible == null) cachedDestructible = GetComponent<D2dDestructible>();

			cachedDestructible.OnSplitEnd += HandleSplitEnd;
		}

		protected virtual void OnDisable()
		{
			cachedDestructible.OnSplitEnd -= HandleSplitEnd;
		}

		private void HandleSplitEnd(List<D2dDestructible> splitDestructibles, D2dDestructible.SplitMode mode)
		{
			if (Prefab != null)
			{
				var clone = Instantiate(Prefab, transform.position, Quaternion.identity);

				clone.SetActive(true);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dSplitSpawner))]
	public class D2dSplitSpawner_Editor : D2dEditor<D2dSplitSpawner>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Prefab == null));
				Draw("Prefab");
			EndError();
		}
	}
}
#endif