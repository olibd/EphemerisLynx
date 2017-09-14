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

                IDAttribute idAttr = new IDAttribute()
                {
                    IDUID = obj.UID,
                    AttrUID = entry.Value.UID
                };

                IMapper<IDAttribute> idAttrMapper = new Mapper<IDAttribute>(_dbFilePath);
                await idAttrMapper.SaveAsync(idAttr);
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

            return await Task.Run(async () =>
            {
                SQLiteConnection conn = new SQLiteConnection(_dbFilePath);
                TableQuery<IDAttribute> query = conn
                    .Table<IDAttribute>()
                    .Where(mapping => mapping.IDUID == UID);

                foreach (IDAttribute mapping in query)
                {
                    Attribute attr = await _attrMapper.GetAsync(mapping.AttrUID);
                    id.AddAttribute(attr.Description, attr);
                }

                return id;
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

            [Indexed]
            public int IDUID
            {
                get { return _iDUID; }
                set
                {
                    UID = ComputeRelationshipID(value, AttrUID);
                    _iDUID = value;
                }
            }

            public int AttrUID
            {
                get { return _attrUID; }
                set
                {
                    UID = ComputeRelationshipID(IDUID, value);
                    _attrUID = value;
                }
            }

            /// <summary>
            /// Computes the relationship identifier (primary key).
            /// </summary>
            /// <returns>The relationship identifier.</returns>
            /// <param name="idUID">Identifier uid.</param>
            /// <param name="attrUID">Attr uid.</param>
            private int ComputeRelationshipID(int idUID, int attrUID)
            {
                return int.Parse(idUID + "" + attrUID);
            }
        }
    }
}
