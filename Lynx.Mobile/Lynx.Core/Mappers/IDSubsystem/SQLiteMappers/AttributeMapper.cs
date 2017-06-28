using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Mappers.IDSubsystem.SQLiteMappers
{
    public class AttributeMapper : ExternalElementMapper<Attribute>
    {
        private IMapper<Certificate> _certMapper;
        public AttributeMapper(string DBFilePath, IMapper<Certificate> certMapper) : base(DBFilePath)
        {
            _certMapper = certMapper;
        }

        public async override Task<int> SaveAsync(Attribute obj)
        {
            int baseReturn = await base.SaveAsync(obj);

            await SaveCertificates(obj);

            return baseReturn;
        }

        private async Task SaveCertificates(Attribute obj)
        {
            if (obj.Certificates == null)
                return;

            foreach (KeyValuePair<string, Certificate> entry in obj.Certificates)
            {
                await _certMapper.SaveAsync(entry.Value);

                AttributeCertificateMapping attrCert = new AttributeCertificateMapping()
                {
                    AttrUID = obj.UID,
                    CertUID = entry.Value.UID
                };

                IMapper<AttributeCertificateMapping> attrCertMapper = new Mapper<AttributeCertificateMapping>(_dbFilePath);
                await attrCertMapper.SaveAsync(attrCert);
            }
        }

        private class AttributeCertificateMapping : Model
        {
            private int _attrUID;
            private int _certUID;

            public int AttrUID
            {
                get { return _attrUID; }
                set
                {
                    UID = ComputeRelationshipID(value, CertUID);
                    _attrUID = value;
                }
            }

            public int CertUID
            {
                get { return _certUID; }
                set
                {
                    UID = ComputeRelationshipID(AttrUID, value);
                    _certUID = value;
                }
            }

            private int ComputeRelationshipID(int attrID, int certID)
            {
                return int.Parse(attrID + "" + certID);
            }
        }
    }
}
