using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component turns the current Rigidbody2D into a snake that can be controlled with the <b>Horizontal</b> and <b>Vertical</b> input axes.</summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dPlayerSnake")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Player Snake")]
	public class D2dPlayerSnake : MonoBehaviour
	{
		[Tooltip("This allows you to set the GameObject that contains the D2dRepeatStamp component that will be used to dig in front of the snake.")]
		public GameObject DigRoot;

		[Tooltip("This allows you to set the distance from snake head the DigRoot will be placed relative to the desired movement direction.")]
		public float DigDistance = 0.25f;

		[Tooltip("This allows you to set the amount of rays that will be fired around the snake head to handle movement.")]
		public int RayCount = 8;

		[Tooltip("This allows you to set the distance fro mthe snake head the rays will fire in world space.")]
		public float RayRange = 1.0f;

		[Tooltip("This allows you to set the Rigidbody2D.AddForce acceleration applied per raycast hit.")]
		public float HitMovement = 1.0f;

		[Tooltip("This allows you to set the Rigidbody2D.drag applied per raycast hit.")]
		public float HitDrag = 1.0f;

		[Tooltip("This allows you to set the prefab that will be used when spawning tail segments.")]
		public Transform TailPrefab;

		[Tooltip("This allows you to set how many tail segments will be initially spawned with the snake.")]
		public int TailInitial = 10;

		[Tooltip("This allows you to set the maximum distance between each snake tail segment in world space.")]
		public float TailSpacing = 0.5f;

		[Tooltip("This stores a list of all tail segments that have been spawned.")]
		public List<Transform> TailSegments;

		[System.NonSerialized]
		private Rigidbody2D cachedBody;

		[SerializeField]
		private D2dInputManager inputManager = new D2dInputManager();

		private static RaycastHit2D[] hits = new RaycastHit2D[64];

		[ContextMenu("Add Tail Segment")]
		public void AddTailSegment()
		{
			if (TailPrefab != null)
			{
				var segment = Instantiate(TailPrefab, transform.position, transform.rotation);

				if (TailSegments == null)
				{
					TailSegments = new List<Transform>();
				}

				TailSegments.Add(segment);

				segment.gameObject.SetActive(true);
			}
		}

		protected virtual void OnEnable()
		{
			cachedBody = GetComponent<Rigidbody2D>();
		}

		protected virtual void Start()
		{
			for (var i = 0; i < TailInitial; i++)
			{
				AddTailSegment();
			}
		}

		protected virtual void Update()
		{
			// Update input
			inputManager.Update(KeyCode.None);

			if (TailSegments != null)
			{
				var prev = transform;

				for (var i = 0; i < TailSegments.Count; i++)
				{
					var cur  = TailSegments[i];
					var dist = Vector3.Distance(prev.position, cur.position);

					if (dist > TailSpacing)
					{
						cur.position = Vector3.MoveTowards(cur.position, prev.position, dist - TailSpacing);
					}

					prev = cur;
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			var hitCount = 0;

			for (var i = 0; i < RayCount; i++)
			{
				var dir   = GetRayDirection(i);
				var count = Physics2D.RaycastNonAlloc(transform.position, dir, hits, RayRange);

				for (var j = 0; j < count; j++)
				{
					var hit = hits[j];

					if (hit.transform != transform)
					{
						hitCount += 1; break;
					}
				}
			}

			cachedBody.drag = HitDrag * hitCount;

			var delta = inputManager.GetAveragePullScaled(true);

			delta.x = Mathf.Clamp(delta.x / 100.0f, -1.0f, 1.0f);
			delta.y = Mathf.Clamp(delta.y / 100.0f, -1.0f, 1.0f);

			cachedBody.AddForce(delta * HitMovement * hitCount);

			if (DigRoot != null)
			{
				DigRoot.SetActive(delta != Vector2.zero);

				DigRoot.transform.localPosition = delta * DigDistance;
			}
		}
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			for (var i = 0; i < RayCount; i++)
			{
				var dst = transform.position + GetRayDirection(i) * RayRange;

				Gizmos.DrawLine(transform.position, dst);
			}
		}
#endif
		private Vector3 GetRayDirection(int index)
		{
			var a = (360.0f / RayCount) * Mathf.Deg2Rad * index;

			return new Vector3(Mathf.Sin(a), Mathf.Cos(a));
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dPlayerSnake))]
	public class D2dPlayerSnake_Editor : D2dEditor<D2dPlayerSnake>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.DigRoot == null));
				Draw("DigRoot");
			EndError();
			Draw("DigDistance");

			Separator();

			Draw("RayCount");
			Draw("RayRange");
			Draw("HitMovement");
			Draw("HitDrag");

			Separator();

			BeginError(Any(t => t.TailPrefab == null));
				Draw("TailPrefab");
			EndError();
			Draw("TailInitial");
			Draw("TailSegments");
			BeginError(Any(t => t.TailSpacing <= 0.0f));
				Draw("TailSpacing");
			EndError();
		}
	}
}
#endif