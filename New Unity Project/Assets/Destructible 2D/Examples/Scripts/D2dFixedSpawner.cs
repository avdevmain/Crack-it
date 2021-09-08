using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component spawns the specified prefab, and respawns it if it's been destroyed.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dFixedSpawner")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Fixed Spawner")]
	public class D2dFixedSpawner : MonoBehaviour
	{
		[Tooltip("The prefab that will be spawned.")]
		public GameObject Prefab;

		[Tooltip("The delay in seconds between the spawned object being deleted, and a new clone being spawned.")]
		public float RespawnDelay = 1.0f;

		[SerializeField]
		private GameObject clone;

		[SerializeField]
		private float cooldown;

		protected virtual void Update()
		{
			if (clone == null)
			{
				cooldown -= Time.deltaTime;

				if (cooldown <= 0.0f)
				{
					if (Prefab != null)
					{
						cooldown = RespawnDelay;
						clone    = Instantiate(Prefab, transform.position, transform.rotation);

						clone.SetActive(true);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dFixedSpawner))]
	public class D2dFixedSpawner_Editor : D2dEditor<D2dFixedSpawner>
	{
		protected override void OnInspector()
		{
			Draw("Prefab");
			Draw("RespawnDelay");
		}
	}
}
#endif