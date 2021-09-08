using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component implements a basic 2D gun that spawns a prefab where you click on the screen.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dGun")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Gun")]
	public class D2dGun : MonoBehaviour
	{
		[Tooltip("Minimum time between each shot in seconds")]
		public float ShootDelay = 0.1f;

		[Tooltip("The bullet prefab spawned when shooting")]
		public GameObject BulletPrefab;

		[Tooltip("The muzzle prefab spawned on the gun when shooting")]
		public GameObject MuzzleFlashPrefab;

		// Seconds until next shot is available
		[SerializeField]
		private float cooldown;

		public bool CanShoot
		{
			get
			{
				return cooldown <= 0.0f;
			}
		}

		public void Shoot()
		{
			if (cooldown <= 0.0f)
			{
				cooldown = ShootDelay;

				if (BulletPrefab != null)
				{
					Instantiate(BulletPrefab, transform.position, transform.rotation);
				}

				if (MuzzleFlashPrefab != null)
				{
					Instantiate(MuzzleFlashPrefab, transform.position, transform.rotation);
				}
			}
		}

		protected virtual void Update()
		{
			cooldown -= Time.deltaTime;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dGun))]
	public class D2dGun_Editor : D2dEditor<D2dGun>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.ShootDelay < 0.0f));
				Draw("ShootDelay");
			EndError();
			Draw("BulletPrefab");
			Draw("MuzzleFlashPrefab");
		}
	}
}
#endif