using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

        /// <summary>
        /// Save the specified ExternalElement.
        /// </summary>
        /// <returns>The object UID.</returns>
        /// <param name="obj">Object.</param>
        public async override Task<int> SaveAsync(T obj)
        {
            int baseReturn = await base.SaveAsync(obj);

            await SaveContent(obj);

            return baseReturn;
        }

        /// <summary>
        /// Saves the extrenal element content.
        /// </summary>
        /// <returns>Task</returns>
        /// <param name="obj">Object.</param>
        private async Task SaveContent(T obj)
        {
            //Serialize the content as a JSON string
            string content = JsonConvert.SerializeObject(obj.Content);

            ExternalElementContentMapping contentMapping = new ExternalElementContentMapping()
            {
                ExtUID = obj.UID,
                SerilizedContent = content,
                ExtType = obj.GetType().ToString(),
                ContentType = obj.Content.GetType().ToString()
            };

            Mapper<ExternalElementContentMapping> contentMapper = new Mapper<ExternalElementContentMapping>(_dbFilePath);
            await contentMapper.SaveAsync(contentMapping);
        }

        /// <summary>
        /// External element content mapping. This class creates a one to one
        /// mapping between then external element and its content.
        /// </summary>
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