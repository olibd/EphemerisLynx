using System;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Facade
{
    public class DummyContentService : IContentService
    {
        //Simple early implementation. Returns the location a content.
        public IContent GetContent(string location, string hash)
        {
            //TODO: Check hash
            return new StringContent(location);
        }
    }
}
