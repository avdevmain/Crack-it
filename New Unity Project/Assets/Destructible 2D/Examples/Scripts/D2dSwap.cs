using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Destructible2D.Examples
{
	/// <summary>This component allows you to swap the current <b>SpriteRenderer</b> sprite with the specified sprite when you manually call the <b>Swap</b> method. This can be done using <b>D2dRequirements</b> to show a different damage state.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dSwap")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Swap")]
	public class D2dSwap : MonoBehaviour
	{
		[Tooltip("The visual sprite you want to swap in.")]
		public Sprite VisualSprite;

		[System.NonSerialized]
		private SpriteRenderer cachedSpriteRenderer;

		/// <summary>This will instantly trigger the swap.</summary>
		[ContextMenu("Swap")]
		public void Swap()
		{
			if (cachedSpriteRenderer == null) cachedSpriteRenderer = GetComponent<SpriteRenderer>();

			cachedSpriteRenderer.sprite = VisualSprite;
		}
	}
}

#if UNITY_EDITOR
namespace Destructible2D.Examples
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dSwap))]
	public class D2dSwap_Editor : D2dEditor<D2dSwap>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.VisualSprite == null));
				Draw("VisualSprite", "The visual sprite you want to swap in.");
			EndError();
		}
	}
}
#endif