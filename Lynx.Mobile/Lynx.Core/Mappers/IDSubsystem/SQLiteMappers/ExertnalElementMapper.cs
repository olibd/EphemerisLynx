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
    public class ExertnalElementMapper<T> : Mapper<T> where T : ExternalElement, new()
    {
        public ExertnalElementMapper(string DBFilePath) : base(DBFilePath)
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
            string content = JsonConvert.SerializeObject(obj);
            ExternalElementContentMapping<T> contentMapping = new ExternalElementContentMapping<T>()
            {
                ExtUID = obj.UID,
                ContentUID = obj.Content.UID,
                SerilizedContent = content
            };

            _db.CreateTable<ExternalElementContentMapping<T>>();
            Mapper<ExternalElementContentMapping<T>> contentMapper = new Mapper<ExternalElementContentMapping<T>>(_db.DatabasePath);
            contentMapper.Save(contentMapping);
        }

        private class ExternalElementContentMapping<T> : Model
        {
            private int _extUID;
            private int _contentUID;

            [Unique]
            public int ExtUID { get; set; }

            [Unique]
            public int ContentUID { get; set; }

            public string SerilizedContent { get; set; }
        }
    }
}