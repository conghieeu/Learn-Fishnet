using UnityEngine.Networking;

internal class AcceptAllCertificatesSignedWithASpecificPublicKey : CertificateHandler
{
	protected override bool ValidateCertificate(byte[] certificateData)
	{
		return true;
	}
}
