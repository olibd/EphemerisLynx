using System;
using System.Collections.Generic;
using System.Text;

namespace Lynx.Core.Interactions
{
    public class UserFacingErrorInteraction
    {
        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; }

        public UserFacingErrorInteraction(string errorMessage)
        {
            _errorMessage = errorMessage;
        }

        public UserFacingErrorInteraction(UserFacingException e)
        {
            _errorMessage = e.Message;
        }
    }
}
