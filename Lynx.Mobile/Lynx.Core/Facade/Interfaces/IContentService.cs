using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Facade.Interfaces
{
    interface IContentService
    {
        /// <summary>
        /// Fetch the content from the remote URL, verify its hash and return the content if it matches
        /// </summary>
        /// <param name="location">URL of the external content</param>
        /// <param name="hash">Hash of the external content</param>
        /// <returns>External content if hash matches. Throws exception in case of mismatch</returns>
        IContent GetContent(string location, string hash);
    }
}
