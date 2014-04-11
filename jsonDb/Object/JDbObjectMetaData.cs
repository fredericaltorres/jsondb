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
    public class JDbObjectMetaData
    {
        string _id;

        public int Version  { get; set; }

        public string Id { 
            get {
                if(this._id ==  null)
                    this._id = Guid.NewGuid().ToString();
                return this._id;
            } 
            set{
                this._id = value;
            }
        }
        public DateTime LastUpdate { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public IStore Store        { get; set; }

        public void NextVersion() {

            this.LastUpdate = DateTime.Now;
            this.Version += 1;
        }
    }
}
