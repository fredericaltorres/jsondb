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
    public class JDbObject
    {
        public JDbObjectMetaData __metadata { get; set; }

        public JDbObject() {

            this.__metadata = new JDbObjectMetaData();
        }
        
        // Inherited member

        public virtual string Save(string objectKey = null) {

            if(objectKey == null)
                objectKey = this.__metadata.Id;
            return this.__metadata.Store.Save(objectKey, this.Serialize());
        }

        public virtual string Serialize() {

            this.__metadata.NextVersion();
            var json = JDbObject.Serialize(this);
            return json;
        }
        
        // Static Members

        const string VersionJsonPropertyName = "__version";

        public static T Deserialize<T>(string json) where T : new() {

            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return t;
        }

        public static string Serialize<T>(T o) where T : new() {

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented);
            return json;
        }
    }
}
