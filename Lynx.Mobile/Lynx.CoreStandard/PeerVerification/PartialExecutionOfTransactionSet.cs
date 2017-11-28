using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.PeerVerification
{
    public class PartialExecutionOfTransactionSet<T> : UserFacingException
    {
        public T[] SuccessfulTransactions { get; set; }
        public T[] UnsuccessfulTransactions { get; set; }

        public PartialExecutionOfTransactionSet() : base("Failed to deploy all transactions.")
        {
        }

        public PartialExecutionOfTransactionSet(string message) : base(message)
        {
        }

        public PartialExecutionOfTransactionSet(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
