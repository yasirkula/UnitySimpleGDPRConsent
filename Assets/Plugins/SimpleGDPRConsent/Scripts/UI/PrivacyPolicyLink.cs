using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleGDPRConsent
{
	public class PrivacyPolicyLink : MonoBehaviour, IPointerClickHandler
	{
#pragma warning disable 0649
		[SerializeField]
		private Text text;
#pragma warning restore 0649

		private string url;

		public void Initialize( string text, string url )
		{
			if( !string.IsNullOrEmpty( text ) )
				this.text.text = text;

			this.url = url;
		}

		public void OnPointerClick( PointerEventData eventData )
		{
			SimpleGDPR.OpenURL( url );
		}
	}
}