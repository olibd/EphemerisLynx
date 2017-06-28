using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
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

        private class IDAttribute : Model
        {
            private int _iDUID;
            private int _attrUID;

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

            private int ComputeRelationshipID(int idUID, int attrUID)
            {
                return int.Parse(idUID + "" + attrUID);
            }
        }
    }
}
