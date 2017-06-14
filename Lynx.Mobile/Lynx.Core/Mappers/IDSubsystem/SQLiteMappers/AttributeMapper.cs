using System;
using System.Collections.Generic;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Mappers.IDSubsystem.SQLiteMappers
{
    public class AttributeMapper : ExertnalElementMapper<Attribute>
    {
        public AttributeMapper(string DBFilePath) : base(DBFilePath)
        {
        }

        public override int Save(Attribute obj)
        {
            int baseReturn = base.Save(obj);

            SaveCertificates(obj);

            return baseReturn;
        }

        private void SaveCertificates(Attribute obj)
        {
            _db.CreateTable<AttributeCertificateMapping>();

            foreach (KeyValuePair<string, Certificate> entry in obj.Certificates)
            {
                Mapper<Certificate> certMapper = new Mapper<Certificate>(_db.DatabasePath);
                certMapper.Save(entry.Value);

                AttributeCertificateMapping attrCert = new AttributeCertificateMapping()
                {
                    AttrUID = obj.UID,
                    CertUID = entry.Value.UID
                };

                Mapper<AttributeCertificateMapping> attrCertMapper = new Mapper<AttributeCertificateMapping>(_db.DatabasePath);
                attrCertMapper.Save(attrCert);
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
