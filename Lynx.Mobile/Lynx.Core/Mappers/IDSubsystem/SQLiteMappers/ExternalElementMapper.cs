using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;
using SQLite;

namespace Lynx.Core.Mappers.IDSubsystem.SQLiteMappers
{
    public class ExternalElementMapper<T> : Mapper<T> where T : ExternalElement, new()
    {
        public ExternalElementMapper(string DBFilePath) : base(DBFilePath)
        {
        }

        public override int Save(T obj)
        {
            int baseReturn = base.Save(obj);

            SaveContent(obj);

            return baseReturn;
        }

        private void SaveContent(T obj)
        {
            string content = JsonConvert.SerializeObject(obj.Content);
            ExternalElementContentMapping contentMapping = new ExternalElementContentMapping()
            {
                ExtUID = obj.UID,
                SerilizedContent = content,
                ExtType = obj.GetType().ToString(),
                ContentType = obj.Content.GetType().ToString()
            };

            Mapper<ExternalElementContentMapping> contentMapper = new Mapper<ExternalElementContentMapping>(_dbFilePath);
            contentMapper.Save(contentMapping);
        }

        private class ExternalElementContentMapping : Model
        {
            private int _extUID;
            [Indexed(Name = "ExtTypeID", Order = 2, Unique = true)]
            public string ExtType { get; set; }
            [Indexed(Name = "ExtTypeID", Order = 1, Unique = true)]
            public int ExtUID { get; set; }

            public string ContentType { get; set; }

            public string SerilizedContent { get; set; }
        }
    }
}