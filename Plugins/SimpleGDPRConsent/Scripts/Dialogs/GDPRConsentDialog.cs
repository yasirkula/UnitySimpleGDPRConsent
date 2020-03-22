using SimpleGDPRConsent;
using System.Collections.Generic;
using UnityEngine;

public class GDPRConsentDialog : IGDPRDialog
{
	public struct Section
	{
		public readonly string description;
		public readonly string title;
		public readonly string identifier;
		public readonly bool initialConsentValue;
		public readonly string buttonLabel;
		public readonly SimpleGDPR.ButtonClickDelegate onButtonClicked;

		public Section( string description, string title, string identifier, bool initialConsentValue, string buttonLabel, SimpleGDPR.ButtonClickDelegate onButtonClicked )
		{
			this.description = description;
			this.title = title;
			this.identifier = identifier;
			this.initialConsentValue = initialConsentValue;
			this.buttonLabel = buttonLabel;
			this.onButtonClicked = onButtonClicked;
		}
	}

	private List<Section> sections;
	private List<string> privacyPolicyLinks;

	public GDPRConsentDialog()
	{ }

	private GDPRConsentDialog AddSection( Section section )
	{
		if( sections == null )
			sections = new List<Section>( 2 );

		sections.Add( section );
		return this;
	}

	public GDPRConsentDialog AddSectionWithToggle( string identifier, string title, string description = null, bool initialConsentValue = true )
	{
		if( string.IsNullOrEmpty( identifier ) )
		{
			Debug.LogError( "Error: 'GDPR.identifier' was empty!" );
			return this;
		}

		return AddSection( new Section( description, title, identifier, initialConsentValue, null, null ) );
	}

	public GDPRConsentDialog AddSectionWithButton( SimpleGDPR.ButtonClickDelegate onButtonClicked, string title, string description = null, string buttonLabel = null )
	{
		if( onButtonClicked == null )
		{
			Debug.LogError( "Error: 'GDPR.onButtonClicked' was empty!" );
			return this;
		}

		return AddSection( new Section( description, title, null, true, buttonLabel, onButtonClicked ) );
	}

	public GDPRConsentDialog AddPrivacyPolicy( string link )
	{
		if( string.IsNullOrEmpty( link ) )
		{
			Debug.LogError( "Error: 'GDPR.link' was empty!" );
			return this;
		}

		if( privacyPolicyLinks == null )
			privacyPolicyLinks = new List<string>( 4 );

		if( !ContainsPrivacyPolicy( link ) )
			privacyPolicyLinks.Add( link );

		return this;
	}

	public GDPRConsentDialog AddPrivacyPolicies( params string[] links )
	{
		if( links == null || links.Length == 0 )
		{
			Debug.LogError( "Error: 'GDPR.links' was empty!" );
			return this;
		}

		if( privacyPolicyLinks == null )
			privacyPolicyLinks = new List<string>( links.Length );

		for( int i = 0; i < links.Length; i++ )
			AddPrivacyPolicy( links[i] );

		return this;
	}

	private bool ContainsPrivacyPolicy( string link )
	{
		if( privacyPolicyLinks == null )
			return false;

		for( int i = 0; i < privacyPolicyLinks.Count; i++ )
		{
			if( privacyPolicyLinks[i] == link )
				return true;
		}

		return false;
	}

	void IGDPRDialog.ShowDialog( SimpleGDPR.DialogClosedDelegate onDialogClosed )
	{
		GDPRConsentCanvas.Instance.ShowConsentDialog( sections, privacyPolicyLinks, onDialogClosed );
	}
}