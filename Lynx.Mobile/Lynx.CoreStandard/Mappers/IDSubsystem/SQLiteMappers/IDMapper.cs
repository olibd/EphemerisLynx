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
    public class IDMapper : Mapper<ID>
    {
        private IMapper<Attribute> _attrMapper;
        public IDMapper(string DBFilePath, IMapper<Attribute> attrMapper) : base(DBFilePath)
        {
            _attrMapper = attrMapper;
        }

        /// <summary>
        /// Save the specified ID.
        /// </summary>
        /// <returns>The object UID.</returns>
        /// <param name="obj">Object.</param>
        public async override Task<int> SaveAsync(ID obj)
        {
            int baseReturn = await base.SaveAsync(obj);

            foreach (KeyValuePair<string, Attribute> entry in obj.Attributes)
            {
                await _attrMapper.SaveAsync(entry.Value);

                //Save the relationship between the ID and the attribute if it does not already exists
                IDAttribute idAttr;
                if ((idAttr = await GetIDAttributeMapping(obj.UID, entry.Value.UID)) == null)
                {
                    idAttr = new IDAttribute()
                    {
                        IDUID = obj.UID,
                        AttrUID = entry.Value.UID
                    };

                    IMapper<IDAttribute> idAttrMapper = new Mapper<IDAttribute>(_dbFilePath);
                    await idAttrMapper.SaveAsync(idAttr);
                }
            }

            return baseReturn;
        }

        /// <summary>
        /// Get object T given the UID.
        /// </summary>
        /// <returns>The object T.</returns>
        /// <param name="UID">UID.</param>
        public override async Task<ID> GetAsync(int UID)
        {
            ID id = await base.GetAsync(UID);

            //if we have a non-empty collection of certificates, then we return
            //because it means that the Attribute was loaded from the identity
            //map
            if (id.Attributes.Count > 0)
                return id;

            SQLiteConnection conn = await ConnectToTableAsync<IDAttribute>();
            TableQuery<IDAttribute> query = null;
            await Task.Run(() =>
            {
                query = conn
                    .Table<IDAttribute>()
                    .Where(mapping => mapping.IDUID == UID);
            });

            foreach (IDAttribute mapping in query)
            {
                Attribute attr = await _attrMapper.GetAsync(mapping.AttrUID);
                id.AddAttribute(attr);
            }
            conn.Close();
            return id;
        }

        private async Task<IDAttribute> GetIDAttributeMapping(int IDUID, int attrUID)
        {
            SQLiteConnection conn = await ConnectToTableAsync<IDAttribute>();
            return await Task.Run(() =>
            {
                IDAttribute idAttrMapping = conn
                    .Table<IDAttribute>()
                    .Where(mapping => mapping.IDUID == IDUID && mapping.AttrUID == attrUID).FirstOrDefault();

                conn.Close();
                return idAttrMapping;
            });
        }

        /// <summary>
        /// This private classes create a one to many relationship table in the 
        /// database when serializing an ID to the database. It will contain
        /// the UID of the attribute and of the ID. It does not support foreing
        /// keys.
        /// </summary>
        private class IDAttribute : Model
        {
            private int _iDUID;
            private int _attrUID;

            [Indexed(Name = "RelationshipID", Order = 1, Unique = true)]
            public int IDUID
            {
                get;
                set;
            }

            [Indexed(Name = "RelationshipID", Order = 2, Unique = true)]
            public int AttrUID
            {
                get;
                set;
            }
        }
    }
}
