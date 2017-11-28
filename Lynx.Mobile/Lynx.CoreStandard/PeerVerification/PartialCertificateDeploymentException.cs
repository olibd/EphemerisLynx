using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.PeerVerification
{
    public class PartialCertificateDeploymentException : PartialExecutionOfTransactionSet<Certificate>
    {
        public PartialCertificateDeploymentException(Certificate[] undeployedContracts) : base(GenerateExceptionMessage(undeployedContracts))
        {
            UnsuccessfulTransactions = undeployedContracts;
        }

        private static string GenerateExceptionMessage(Certificate[] undeployedContracts)
        {
            string plural = "";
            if (undeployedContracts.Length > 1)
                plural += "s";

            string failedAttrs = "";
            bool first = true;
            foreach (Certificate cert in undeployedContracts)
            {
                if (first)
                {
                    first = false;
                }
                else
                    failedAttrs += ", ";
                failedAttrs += "\"" + cert.OwningAttribute.Description + "\"";
            }

            return string.Format("Failed to deploy the certificate{0} for the attribute{0} {1}.", plural, failedAttrs);
        }

        public PartialCertificateDeploymentException() : base("Failed to deploy all certificates.")
        {
        }

        public PartialCertificateDeploymentException(string message) : base(message)
        {
        }

        public PartialCertificateDeploymentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
