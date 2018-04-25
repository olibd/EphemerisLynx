﻿using System;

namespace Lynx.Core.Crypto
{
    public class AddressDoesntMatchException : Exception
    {
        public AddressDoesntMatchException()
        {
        }

		public AddressDoesntMatchException(string message) : base(message)
        {
		}

		public AddressDoesntMatchException(string message, Exception innerException) : base(message, innerException)
        {
		}
    }
}

