﻿using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace Lynx.Core.Services
{
    public abstract class HandshakeToken : Token, IHandshakeToken
    {
        private ID _id;
        public ID Id
        {
            get
            {
                return _id;
            }

            set
            {
                SetOnPayload("idAddr", value.Address);
                _id = value;
            }
        }

        public string PublicKey
        {
            get
            {
                return GetFromHeader("pubkey");
            }

            set
            {
                SetOnHeader("pubkey", value);
            }
        }

        public bool Encrypted
        {
            get
            {
                return Boolean.Parse(GetFromHeader("encrypted"));
            }

            set
            {
                SetOnHeader("encrypted", value.ToString());
            }
        }
    }
}
