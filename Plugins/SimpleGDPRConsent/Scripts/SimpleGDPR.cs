using SimpleGDPRConsent;
using System.Collections;
using System.Net;
using UnityEngine;

public interface IGDPRDialog
{
	void ShowDialog( SimpleGDPR.DialogClosedDelegate onDialogClosed );
}

public static class SimpleGDPR
{
	public enum ConsentState { Unknown = 0, No = 1, Yes = 2 };

	private const string EU_QUERY_URL = "http://adservice.google.com/getconfig/pubvendors";

	public delegate void ButtonClickDelegate();
	public delegate void DialogClosedDelegate();

	public static bool IsDialogVisible { get { return GDPRConsentCanvas.IsVisible; } }
	public static bool IsTermsOfServiceAccepted { get { return GDPRConsentCanvas.GetTermsOfServiceState() == ConsentState.Yes; } }

	private static bool? m_isGDPRApplicable = null;
	public static bool IsGDPRApplicable
	{
		get
		{
			if( !m_isGDPRApplicable.HasValue )
			{
				try
				{
					using( WebClient webClient = new WebClient() )
					{
						string response = webClient.DownloadString( EU_QUERY_URL );
						int index = response.IndexOf( "is_request_in_eea_or_unknown\":" );
						if( index < 0 )
							m_isGDPRApplicable = true;
						else
						{
							index += 30;
							m_isGDPRApplicable = index >= response.Length || !response.Substring( index ).TrimStart().StartsWith( "false" );
						}
					}
				}
				catch( System.Exception e )
				{
					Debug.LogException( e );
					m_isGDPRApplicable = true;
				}
			}

			return m_isGDPRApplicable.Value;
		}
	}

	public static ConsentState GetConsentState( string identifier )
	{
		return GDPRConsentCanvas.GetConsentState( identifier );
	}

	public static void OpenURL( string url )
	{
#if !UNITY_EDITOR && UNITY_WEBGL
		Application.ExternalEval( "window.open(\"" + url + "\",\"_blank\")" );
#else
		Application.OpenURL( url );
#endif
	}

	public static void ShowDialog( this IGDPRDialog dialog, DialogClosedDelegate onDialogClosed = null )
	{
		dialog.ShowDialog( onDialogClosed );
	}

	public static IEnumerator WaitForDialog( this IGDPRDialog dialog )
	{
		dialog.ShowDialog( null );

		while( IsDialogVisible )
			yield return null;
	}
}