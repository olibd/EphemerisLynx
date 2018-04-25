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

            ExternalElementContentMapping contentMapping;

            if ((contentMapping = await GetExternalElementContentMappingAsync(obj.UID, obj.GetType().ToString())) == null)
            {
                contentMapping = new ExternalElementContentMapping()
                {
                    ExtUID = obj.UID,
                    //save the serialized data directly in the mapping
                    SerilizedContent = content,
                    ExtType = obj.GetType().ToString(),
                    ContentType = obj.Content.GetType().ToString()
                };
            }
            else
            {
                contentMapping.SerilizedContent = content;
                contentMapping.ContentType = obj.Content.GetType().ToString();
            }

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

            SQLiteConnection conn = await ConnectToTableAsync<ExternalElementContentMapping>();
            return await Task.Run(() =>
            {
                string typeOfT = typeof(T).ToString();
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

        private async Task<ExternalElementContentMapping> GetExternalElementContentMappingAsync(int extUID, string extType)
        {
            SQLiteConnection conn = await ConnectToTableAsync<ExternalElementContentMapping>();
            return await Task.Run(() =>
            {
                ExternalElementContentMapping eeMapping = conn
                    .Table<ExternalElementContentMapping>()
                    .Where(mapping => mapping.ExtUID == extUID && mapping.ExtType == extType).FirstOrDefault();
                conn.Close();
                return eeMapping;
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