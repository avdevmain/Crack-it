using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component reads the current color of any D2dDestructible under the mouse when holding the specified button.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dClickToRead")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Click To Read")]
	public class D2dClickToRead : MonoBehaviour
	{
		/// <summary>The key you must hold down to read.</summary>
		public KeyCode Requires = KeyCode.Mouse0;

		/// <summary>The z position the prefab should spawn at.</summary>
		public float Intercept;

		private D2dInputManager inputManager = new D2dInputManager();

		protected virtual void Update()
		{
			// Touching the screen?
			inputManager.Update(Requires);

			if (inputManager.Fingers.Count > 0)
			{
				// Main camera exists?
				var mainCamera = Camera.main;

				if (mainCamera != null)
				{
					// World position of the mouse
					var position = D2dHelper.ScreenToWorldPosition(inputManager.Fingers[0].PositionA, Intercept, mainCamera);

					// Read the destructible and alpha at this position
					var destructible = default(D2dDestructible);
					var alpha        = default(Color32);

					if (D2dDestructible.TrySampleAlphaAll(position, ref destructible, ref alpha) == true)
					{
						Debug.Log("Read " + destructible + " with alpha: " + alpha);
					}
					else
					{
						Debug.Log("Read nothing.");
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
	[CustomEditor(typeof(D2dClickToRead))]
	public class D2dClickToRead_Editor : D2dEditor<D2dClickToRead>
	{
		protected override void OnInspector()
		{
			Draw("Requires", "The key you must hold down to read.");
			Draw("Intercept", "The z position the prefab should spawn at.");
		}
	}
}
#endif