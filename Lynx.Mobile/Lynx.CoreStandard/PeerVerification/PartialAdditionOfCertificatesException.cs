using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.PeerVerification
{
    public class PartialAdditionOfCertificatesException : PartialExecutionOfTransactionSet<Certificate>
    {
        public PartialAdditionOfCertificatesException(Certificate[] undeployedContracts) : base(GenerateExceptionMessage(undeployedContracts))
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

            return string.Format("Failed to add the certificate{0} to the attribute{0} {1}.", plural, failedAttrs);
        }
    }
}
