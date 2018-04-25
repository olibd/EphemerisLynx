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

                //Try to get the one to many mapping between an attributes and its
                //certificates, if it already exists
                AttributeCertificateMapping attrCert;
                if ((attrCert = await GetAttributeCertificateMappingAsync(obj.UID, entry.Value.UID)) == null)
                {
                    //Create the one to many mapping between an attributes and its
                    //certificates
                    attrCert = new AttributeCertificateMapping()
                    {
                        AttrUID = obj.UID,
                        CertUID = entry.Value.UID
                    };

                    //save the mapping
                    IMapper<AttributeCertificateMapping> attrCertMapper = new Mapper<AttributeCertificateMapping>(_dbFilePath);
                    await attrCertMapper.SaveAsync(attrCert);
                }
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

            SQLiteConnection conn = await ConnectToTableAsync<AttributeCertificateMapping>();

            TableQuery<AttributeCertificateMapping> query = null;
            await Task.Run(() =>
            {
                query = conn
                   .Table<AttributeCertificateMapping>()
                   .Where(mapping => mapping.AttrUID == UID);
            });

            foreach (AttributeCertificateMapping mapping in query)
            {
                Certificate cert = await _certMapper.GetAsync(mapping.CertUID);
                cert.OwningAttribute = attr;
                attr.AddCertificate(cert);
            }
            conn.Close();
            return attr;
        }

        private async Task<AttributeCertificateMapping> GetAttributeCertificateMappingAsync(int attrUID, int certUID)
        {
            SQLiteConnection conn = await ConnectToTableAsync<AttributeCertificateMapping>();
            return await Task.Run(() =>
            {
                AttributeCertificateMapping acMapping = conn
                    .Table<AttributeCertificateMapping>()
                        .Where(mapping => mapping.AttrUID == attrUID && mapping.CertUID == certUID).FirstOrDefault();
                conn.Close();
                return acMapping;
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

            [Indexed(Name = "RelationshipID", Order = 1, Unique = true)]
            public int AttrUID
            {
                get;
                set;
            }

            [Indexed(Name = "RelationshipID", Order = 2, Unique = true)]
            public int CertUID
            {
                get;
                set;
            }
        }
    }
}
