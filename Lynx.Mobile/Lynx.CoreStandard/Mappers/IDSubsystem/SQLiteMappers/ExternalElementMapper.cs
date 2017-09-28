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
            //Serialize the content
            string content = obj.Content.ToString();

            //Define the one to one mapping between the object and its content
            ExternalElementContentMapping contentMapping = new ExternalElementContentMapping()
            {
                ExtUID = obj.UID,
                //save the serialized data directly in the mapping
                SerilizedContent = content,
                ExtType = obj.GetType().ToString(),
                ContentType = obj.Content.GetType().ToString()
            };

            //save the mapping to the database
            Mapper<ExternalElementContentMapping> contentMapper = new Mapper<ExternalElementContentMapping>(_dbFilePath);
            await contentMapper.SaveAsync(contentMapping);
        }

        /// <summary>
        /// Get object T given the UID.
        /// </summary>
        /// <returns>The object T.</returns>
        /// <param name="UID">UID.</param>
        public override async Task<T> GetAsync(int UID)
        {
            //get the base ExternalElement
            T obj = await base.GetAsync(UID);

            return await Task.Run(() =>
            {
                string typeOfT = typeof(T).ToString();
                SQLiteConnection conn = new SQLiteConnection(_dbFilePath);
                TableQuery<ExternalElementContentMapping> query = conn
                    .Table<ExternalElementContentMapping>()
                    .Where(mapping => mapping.ExtUID == obj.UID
                           &&
                           mapping.ExtType == typeOfT);

                if (query.Count() != 1)
                    throw new NoSingleMappingFound();

                ExternalElementContentMapping contentMapping = query.First();

                conn.Close();
                obj.Content = Activator.CreateInstance(Type.GetType(contentMapping.ContentType), new object[] { contentMapping.SerilizedContent }) as IContent;

                return obj;
            });
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