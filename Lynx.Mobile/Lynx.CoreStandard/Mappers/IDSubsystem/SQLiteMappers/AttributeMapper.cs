using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
using SQLite;
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

        /// <summary>
        /// Save the specified Attribute.
        /// </summary>
        /// <returns>The object UID.</returns>
        /// <param name="obj">Object.</param>
        public async override Task<int> SaveAsync(Attribute obj)
        {
            int baseReturn = await base.SaveAsync(obj);

            await SaveCertificates(obj);

            return baseReturn;
        }
        /// <summary>
        /// Parse the attribute object and serializes the Certificates objects.
        /// </summary>
        /// <returns>Task</returns>
        /// <param name="obj">Object.</param>
        private async Task SaveCertificates(Attribute obj)
        {
            if (obj.Certificates == null || obj.Certificates.Count == 0)
            {
                await CreateTableAsync<AttributeCertificateMapping>();
                return;
            }

            //for each certificate in the attribute
            foreach (KeyValuePair<string, Certificate> entry in obj.Certificates)
            {
                await _certMapper.SaveAsync(entry.Value);

                //Create the one to many mapping between an attributes and its
                //certificates
                AttributeCertificateMapping attrCert = new AttributeCertificateMapping()
                {
                    AttrUID = obj.UID,
                    CertUID = entry.Value.UID
                };

                //save the mapping
                IMapper<AttributeCertificateMapping> attrCertMapper = new Mapper<AttributeCertificateMapping>(_dbFilePath);
                await attrCertMapper.SaveAsync(attrCert);
            }
        }

        /// <summary>
        /// Get object T given the UID.
        /// </summary>
        /// <returns>The object T.</returns>
        /// <param name="UID">UID.</param>
        public override async Task<Attribute> GetAsync(int UID)
        {
            Attribute attr = await base.GetAsync(UID);

            //if we have a non-empty collection of certificates, then we return
            //because it means that the Attribute was loaded from the identity
            //map
            if (attr.Certificates.Count > 0)
                return attr;

            return await Task.Run(async () =>
            {
                SQLiteConnection conn = new SQLiteConnection(_dbFilePath);
                TableQuery<AttributeCertificateMapping> query = conn
                    .Table<AttributeCertificateMapping>()
                    .Where(mapping => mapping.AttrUID == UID);

                foreach (AttributeCertificateMapping mapping in query)
                {
                    Certificate cert = await _certMapper.GetAsync(mapping.CertUID);
                    cert.OwningAttribute = attr;
                    attr.AddCertificate(cert);
                }

                return attr;
            });
        }

        /// <summary>
        /// Attribute-certificate mapping. Creates a one to many relationship in
        /// the DB between an attribute and its certificates. Does not create
        /// foreign key.
        /// </summary>
        private class AttributeCertificateMapping : Model
        {
            private int _attrUID;
            private int _certUID;

            [Indexed]
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

            /// <summary>
            /// Computes the relationship identifier.
            /// </summary>
            /// <returns>The relationship identifier.</returns>
            /// <param name="attrID">Attr identifier.</param>
            /// <param name="certID">Cert identifier.</param>
            private int ComputeRelationshipID(int attrID, int certID)
            {
                return int.Parse(attrID + "" + certID);
            }
        }
    }
}
