using System;
using System.Threading.Tasks;

namespace Lynx.Core.PeerVerification.Interfaces
{
    public interface IScanningPeer
    {
		/// <summary>
		/// Parses a JSON-encoded SYN and verifies its integrity.
		/// </summary>
		/// <param name="syn">The JSON-encoded SYN</param>
		Task ProcessSyn(string syn);
    }
}
