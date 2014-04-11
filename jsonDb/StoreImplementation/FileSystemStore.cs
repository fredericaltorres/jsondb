using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DynamicSugar;
using Newtonsoft.Json.Linq;

namespace jsonDb
{
    public class FileSystemStore : IStore {

        string _fileSystemLocation;

        public FileSystemStore(string fileSystemLocation = null) {

            if(fileSystemLocation == null) {
                var appName = Path.GetFileNameWithoutExtension(Assembly.GetCallingAssembly().Location);
                fileSystemLocation = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), appName);
                if(!System.IO.Directory.Exists(fileSystemLocation))
                    System.IO.Directory.CreateDirectory(fileSystemLocation);
            }
            this._fileSystemLocation = fileSystemLocation;
        }

        private string MakeFilename(string objectKey) {

            return Path.Combine(this._fileSystemLocation, objectKey + ".json");
        }

        T IStore.Load<T>(string objectKey) {

            var json = this.__Load(objectKey);
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return t;
        }

        string __Load(string objectKey)
        {
            return System.IO.File.ReadAllText(this.MakeFilename(objectKey));
        }


        string IStore.Save(string objectKey, string json)
        {
            var fileName = this.MakeFilename(objectKey);
            System.IO.File.WriteAllText(fileName, json);
            return fileName;
        }
    }
}
