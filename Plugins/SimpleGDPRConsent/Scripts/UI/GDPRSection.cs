using UnityEngine;
using UnityEngine.UI;

namespace SimpleGDPRConsent
{
	public class GDPRSection : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		private Text title;

		[SerializeField]
		private Text buttonLabel;

		[SerializeField]
		private Text description;

		[SerializeField]
		private Button button;

		[SerializeField]
		private SlidingToggle toggleHolder;

		[SerializeField]
		private GameObject toggle;
#pragma warning restore 0649

		private GDPRConsentDialog.Section section;

		private void Awake()
		{
			button.onClick.AddListener( OnButtonClicked );
		}

		public void Initialize( GDPRConsentDialog.Section section )
		{
			this.section = section;

			if( !string.IsNullOrEmpty( section.description ) )
			{
				description.text = section.description;
				description.gameObject.SetActive( true );
			}
			else
				description.gameObject.SetActive( false );

			if( !string.IsNullOrEmpty( section.title ) )
			{
				title.text = section.title;
				title.gameObject.SetActive( true );
			}
			else
				title.gameObject.SetActive( false );

			if( !string.IsNullOrEmpty( section.identifier ) )
			{
				toggle.gameObject.SetActive( true );

				SimpleGDPR.ConsentState consentState = GDPRConsentCanvas.GetConsentState( section.identifier );
				if( consentState == SimpleGDPR.ConsentState.Unknown )
					toggleHolder.Value = section.initialConsentValue;
				else
					toggleHolder.Value = consentState != SimpleGDPR.ConsentState.No;
			}
			else
				toggle.gameObject.SetActive( false );

			if( section.onButtonClicked != null )
			{
				buttonLabel.text = !string.IsNullOrEmpty( section.buttonLabel ) ? section.buttonLabel : "Configure";
				button.gameObject.SetActive( true );
			}
			else
				button.gameObject.SetActive( false );

			toggleHolder.gameObject.SetActive( toggle.gameObject.activeSelf || title.gameObject.activeSelf );
		}

		private void OnButtonClicked()
		{
			if( section.onButtonClicked != null )
				section.onButtonClicked();
		}

		public void SaveConsent()
		{
			if( !string.IsNullOrEmpty( section.identifier ) )
				GDPRConsentCanvas.SetConsentState( section.identifier, toggleHolder.Value ? SimpleGDPR.ConsentState.Yes : SimpleGDPR.ConsentState.No );
		}
	}
}