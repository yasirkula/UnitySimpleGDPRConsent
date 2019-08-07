using SimpleGDPRConsent;

public class TermsOfServiceDialog : IGDPRDialog
{
	private string termsOfServiceLink;
	private string privacyPolicyLink;

	public TermsOfServiceDialog()
	{ }

	public TermsOfServiceDialog SetTermsOfServiceLink( string termsOfServiceLink )
	{
		this.termsOfServiceLink = termsOfServiceLink;
		return this;
	}

	public TermsOfServiceDialog SetPrivacyPolicyLink( string privacyPolicyLink )
	{
		this.privacyPolicyLink = privacyPolicyLink;
		return this;
	}

	void IGDPRDialog.ShowDialog( SimpleGDPR.DialogClosedDelegate onDialogClosed )
	{
		GDPRConsentCanvas.Instance.ShowTermsOfServiceDialog( termsOfServiceLink, privacyPolicyLink, onDialogClosed );
	}
}