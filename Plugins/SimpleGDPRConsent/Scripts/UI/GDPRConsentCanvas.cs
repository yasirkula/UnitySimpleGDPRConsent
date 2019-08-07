using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleGDPRConsent
{
	public class GDPRConsentCanvas : MonoBehaviour
	{
		private static GDPRConsentCanvas m_instance = null;
		public static GDPRConsentCanvas Instance
		{
			get
			{
				if( m_instance == null )
					m_instance = Instantiate( Resources.Load<GameObject>( "GDPRConsentCanvas" ).GetComponent<GDPRConsentCanvas>() );

				return m_instance;
			}
		}

#pragma warning disable 0649
		[Header( "Terms of Service View" )]
		[SerializeField]
		private PrivacyPolicyLink _termsOfService;

		[SerializeField]
		private PrivacyPolicyLink _privacyPolicy;

		[SerializeField]
		private Button acceptButton;

		[Header( "Consent View" )]
		[SerializeField]
		private GDPRSection sectionPrefab;

		[SerializeField]
		private RectTransform horizontalLinePrefab;

		[SerializeField]
		private PrivacyPolicyLink privacyPolicyPrefab;

		[SerializeField]
		private RectTransform sectionsParent;

		[SerializeField]
		private RectTransform privacyPoliciesParent;

		[SerializeField]
		private Button closeButton;

		[SerializeField]
		private RectTransform dialog;

		[SerializeField]
		private CanvasGroup dialogCanvasGroup;

		[SerializeField]
		private ScrollRect scrollView;

		[SerializeField]
		private RectTransform termsView;

		[SerializeField]
		private RectTransform consentView;

		[SerializeField]
		private Vector2 dialogPadding = new Vector2( 40f, 100f );
#pragma warning restore 0649

		private readonly List<GDPRSection> sectionsUI = new List<GDPRSection>( 4 );
		private readonly List<RectTransform> sectionSeparatorsUI = new List<RectTransform>( 5 );
		private readonly List<PrivacyPolicyLink> privacyPoliciesUI = new List<PrivacyPolicyLink>( 8 );

		private SimpleGDPR.DialogClosedDelegate onDialogClosed = null;
		private int dimensionsChangeCountdown = 0;

		private float contentPaddingY;

		public static bool IsVisible
		{
			get
			{
				if( m_instance == null || m_instance.Equals( null ) )
					return false;

				return m_instance.gameObject.activeSelf;
			}
		}

		private void Awake()
		{
			if( m_instance == null )
			{
				m_instance = this;
				DontDestroyOnLoad( gameObject );
				gameObject.SetActive( false );
			}
			else if( this != m_instance )
			{
				Destroy( gameObject );
				return;
			}

			acceptButton.onClick.AddListener( OnAcceptTermsButtonClicked );
			closeButton.onClick.AddListener( OnCloseDialogButtonClicked );

			sectionSeparatorsUI.Add( (RectTransform) Instantiate( horizontalLinePrefab, sectionsParent, false ) );
			contentPaddingY = -( (RectTransform) scrollView.transform ).sizeDelta.y + 5f;

#if !UNITY_EDITOR && ( UNITY_ANDROID || UNITY_IOS )
			// On mobile platforms, ScreenMatchMode.Shrink makes texts larger (legible) on landscape orientation
			GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
#endif
		}

		private void OnRectTransformDimensionsChange()
		{
			dimensionsChangeCountdown = 2; // Wait for 2 Update cycles, UI system will rebuild the layout during this time

			Vector2 size = ( (RectTransform) transform ).rect.size;
#if UNITY_EDITOR || ( !UNITY_ANDROID && !UNITY_IOS )
			float dialogMaxWidth = size.x - dialogPadding.x;
			float dialogTargetWidth = size.y - dialogPadding.y; // Make square dialog window if possible
			dialog.sizeDelta = new Vector2( dialogTargetWidth < dialogMaxWidth ? dialogTargetWidth : dialogMaxWidth, dialog.sizeDelta.y );
#else
			// On mobile platforms, the dialog should be as wide as possible since the screen is too small
			float dialogTargetPaddingX = size.x < size.y ? dialogPadding.x : dialogPadding.y; // Swapping padding values on landscape orientation
			dialog.sizeDelta = new Vector2( size.x - dialogTargetPaddingX, dialog.sizeDelta.y );
#endif
		}

		private void Update()
		{
			if( dimensionsChangeCountdown > 0 && --dimensionsChangeCountdown == 0 )
			{
				Vector2 size = ( (RectTransform) transform ).rect.size;
#if UNITY_EDITOR || ( !UNITY_ANDROID && !UNITY_IOS )
				float dialogMaxHeight = size.y - dialogPadding.y;
#else
				float dialogTargetPaddingY = size.x < size.y ? dialogPadding.y : dialogPadding.x; // Swapping padding values on landscape orientation
				float dialogMaxHeight = size.y - dialogTargetPaddingY;
#endif
				float dialogTargetHeight = scrollView.content.rect.height + contentPaddingY;
				dialog.sizeDelta = new Vector2( dialog.sizeDelta.x, dialogTargetHeight < dialogMaxHeight ? dialogTargetHeight : dialogMaxHeight );

				if( dialogCanvasGroup.alpha < 1f )
					dialogCanvasGroup.alpha = 1f;

				// To prevent the scrollbar from overflowing when screen orientation changes
				scrollView.OnScroll( new PointerEventData( EventSystem.current ) );
			}
		}

		public static SimpleGDPR.ConsentState GetTermsOfServiceState()
		{
			return (SimpleGDPR.ConsentState) PlayerPrefs.GetInt( "GDPR_Terms", (int) SimpleGDPR.ConsentState.Unknown );
		}

		public static void SetTermsOfServiceState( SimpleGDPR.ConsentState value )
		{
			PlayerPrefs.SetInt( "GDPR_Terms", (int) value );
		}

		public static SimpleGDPR.ConsentState GetConsentState( string identifier )
		{
			return (SimpleGDPR.ConsentState) PlayerPrefs.GetInt( "GDPR_" + identifier, (int) SimpleGDPR.ConsentState.Unknown );
		}

		public static void SetConsentState( string identifier, SimpleGDPR.ConsentState value )
		{
			PlayerPrefs.SetInt( "GDPR_" + identifier, (int) value );
		}

		public void ShowTermsOfServiceDialog( string termsOfServiceLink, string privacyPolicyLink, SimpleGDPR.DialogClosedDelegate onDialogClosed )
		{
			if( !string.IsNullOrEmpty( termsOfServiceLink ) )
			{
				_termsOfService.Initialize( null, termsOfServiceLink );
				_termsOfService.gameObject.SetActive( true );
			}
			else
				_termsOfService.gameObject.SetActive( false );

			if( !string.IsNullOrEmpty( privacyPolicyLink ) )
			{
				_privacyPolicy.Initialize( null, privacyPolicyLink );
				_privacyPolicy.gameObject.SetActive( true );
			}
			else
				_privacyPolicy.gameObject.SetActive( false );

			termsView.gameObject.SetActive( true );
			consentView.gameObject.SetActive( false );
			scrollView.content = termsView;

			OnDialogShown( onDialogClosed );
		}

		public void ShowConsentDialog( List<GDPRConsentDialog.Section> sections, List<string> privacyPolicyLinks, SimpleGDPR.DialogClosedDelegate onDialogClosed )
		{
			if( sections == null || sections.Count == 0 )
				sectionsParent.gameObject.SetActive( false );
			else
			{
				sectionsParent.gameObject.SetActive( true );

				for( int i = sectionsUI.Count; i < sections.Count; i++ )
				{
					sectionsUI.Add( (GDPRSection) Instantiate( sectionPrefab, sectionsParent, false ) );
					sectionSeparatorsUI.Add( (RectTransform) Instantiate( horizontalLinePrefab, sectionsParent, false ) );
				}

				for( int i = 0; i < sectionsUI.Count; i++ )
				{
					bool isActive = i < sections.Count;

					sectionsUI[i].gameObject.SetActive( isActive );
					sectionSeparatorsUI[i + 1].gameObject.SetActive( isActive );
				}

				for( int i = 0; i < sections.Count; i++ )
					sectionsUI[i].Initialize( sections[i] );
			}

			if( privacyPolicyLinks == null || privacyPolicyLinks.Count == 0 )
				privacyPoliciesParent.gameObject.SetActive( false );
			else
			{
				privacyPoliciesParent.gameObject.SetActive( true );

				for( int i = privacyPoliciesUI.Count; i < privacyPolicyLinks.Count; i++ )
					privacyPoliciesUI.Add( (PrivacyPolicyLink) Instantiate( privacyPolicyPrefab, privacyPoliciesParent, false ) );

				for( int i = 0; i < privacyPoliciesUI.Count; i++ )
					privacyPoliciesUI[i].gameObject.SetActive( i < privacyPolicyLinks.Count );

				for( int i = 0; i < privacyPolicyLinks.Count; i++ )
					privacyPoliciesUI[i].Initialize( privacyPolicyLinks[i], privacyPolicyLinks[i] );
			}

			termsView.gameObject.SetActive( false );
			consentView.gameObject.SetActive( true );
			scrollView.content = consentView;

			OnDialogShown( onDialogClosed );
		}

		private void OnAcceptTermsButtonClicked()
		{
			SetTermsOfServiceState( SimpleGDPR.ConsentState.Yes );
			OnDialogClosed();
		}

		private void OnCloseDialogButtonClicked()
		{
			for( int i = 0; i < sectionsUI.Count; i++ )
				sectionsUI[i].SaveConsent();

			OnDialogClosed();
		}

		private void OnDialogShown( SimpleGDPR.DialogClosedDelegate onDialogClosed )
		{
			this.onDialogClosed = onDialogClosed;

			scrollView.verticalNormalizedPosition = 1f;
			dialogCanvasGroup.alpha = 0f; // To hide the annoying size flicker glitch

			OnRectTransformDimensionsChange();
			gameObject.SetActive( true );
		}

		private void OnDialogClosed()
		{
			PlayerPrefs.Save();
			gameObject.SetActive( false );

			if( onDialogClosed != null )
				onDialogClosed();
		}
	}
}