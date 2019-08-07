using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleGDPRConsent
{
	public class SlidingToggle : MonoBehaviour, IPointerClickHandler
	{
#pragma warning disable 0649
		[SerializeField]
		private RectTransform handle;

		[SerializeField]
		private Image background;

		[SerializeField]
		private Sprite backgroundOn;

		[SerializeField]
		private Sprite backgroundOff;
#pragma warning restore 0649

		private bool m_value = true;
		public bool Value
		{
			get { return m_value; }
			set
			{
				if( m_value != value )
				{
					m_value = value;
					UpdateHandle();
				}
			}
		}

		public void OnPointerClick( PointerEventData eventData )
		{
			Value = !Value;
		}

		private void UpdateHandle()
		{
			if( Value )
			{
				handle.anchorMin = new Vector2( 1f, 0f );
				handle.anchorMax = new Vector2( 1f, 1f );
				handle.pivot = new Vector2( 1f, 0.5f );
				handle.anchoredPosition = new Vector2( -1f, 0f );

				background.sprite = backgroundOn;
			}
			else
			{
				handle.anchorMin = new Vector2( 0f, 0f );
				handle.anchorMax = new Vector2( 0f, 1f );
				handle.pivot = new Vector2( 0f, 0.5f );
				handle.anchoredPosition = new Vector2( 1f, 0f );

				background.sprite = backgroundOff;
			}
		}
	}
}