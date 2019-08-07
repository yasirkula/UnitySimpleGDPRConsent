= Simple GDPR Consent =

Online documentation & example code available at: https://github.com/yasirkula/UnitySimpleGDPRConsent
E-mail: yasirkula@gmail.com

1. ABOUT
This plugin helps you present a GDPR consent dialog to the users. Please note that you are responsible from forwarding the consent data to your SDKs.

2. SCRIPTING API
Please see the online documentation for a more in-depth documentation of the Scripting API: https://github.com/yasirkula/UnitySimpleGDPRConsent

SimpleGDPR class has the following functions and properties:

- ConsentState GetConsentState( string identifier ): returns 'Yes', if user has given consent for the 'identifier' event to collect their data (e.g. ads personalization, analytics data collection); 'No', if user has disallowed this data collection and 'Unknown', if the permission hasn't been asked yet
- void ShowDialog( IGDPRDialog dialog, DialogClosedDelegate onDialogClosed = null ): presents the 'dialog' to the user. When the dialog is closed, onDialogClosed is invoked (DialogClosedDelegate takes no parameters). See the DIALOGS section below for more info about the dialogs
- IEnumerator WaitForDialog( IGDPRDialog dialog ): coroutine equivalent of ShowDialog
- void OpenURL( string url ): opens the specified url in the web browser. On WebGL, the url is opened in a new tab

- bool IsDialogVisible: returns true if a dialog is currently visible
- bool IsTermsOfServiceAccepted: returns true if user has accepted the Terms of Service and/or the Privacy Policy
- bool IsGDPRApplicable: returns true, if user is located in the EEA or the request location is unknown. Returns false, if user is not located in the EEA. This value is determined by sending a request to http://adservice.google.com/getconfig/pubvendors, so an active internet connection is required (on Android, set "Internet Access" to Require in Player Settings)

NOTE: To comply with GDPR, users must be allowed to change the consents they've provided at any time. So, make sure that users can access the consent dialog from e.g. the settings menu.

3. DIALOGS
There are two types of dialogs (IGDPRDialog). After creating a dialog instance, you can customize it by chaining the dialog's functions. Then, you can show it to the user via the ShowDialog or WaitForDialog functions.

3.1. TermsOfServiceDialog
This dialog prompts the user to accept your Terms of Service and/or Privacy Policy. User must press the Accept button to close the dialog.

- new TermsOfServiceDialog(): creates a new instance of this dialog
- SetTermsOfServiceLink( string termsOfServiceLink ): sets the Terms of Service url
- SetPrivacyPolicyLink( string privacyPolicyLink ): sets the Privacy Policy url

3.2. GDPRConsentDialog
This dialog consists of a number of sections and a list of privacy policies that explain how the user's data is collected and used. Each section of the dialog asks for a different event's consent (e.g. ads personalization, analytics data collection). There are two types of sections: sections with a toggle and sections with a button. For SDKs that handle their consents in their own way, a button can be used (e.g. Unity Analytics handles the consent via a webpage). Otherwise, it is easier to manage a consent via a toggle.

- new GDPRConsentDialog(): creates a new instance of this dialog
- AddSectionWithToggle( string identifier, string title, string description = null ): adds a section with a toggle to the dialog. Here, 'identifier' is a unique identifier for the particular event you are asking the consent for (e.g. you can use "Ads" for ads personalization). After the dialog is closed, you can pass the same identifier to the SimpleGDPR.GetConsentState function to check whether or not you have consent to collect user's data for that event
- AddSectionWithButton( ButtonClickDelegate onButtonClicked, string title, string description = null, string buttonLabel = null ): adds a section with a button to the dialog. When the button is clicked, onButtonClicked is invoked (ButtonClickDelegate takes no parameters)
- AddPrivacyPolicy( string link ): adds a privacy policy to the list of privacy policies
- AddPrivacyPolicies( params string[] links ): adds a number of privacy policies to the list of privacy policies