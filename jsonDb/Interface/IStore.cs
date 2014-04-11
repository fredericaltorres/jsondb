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
    public interface IStore {

        T Load<T>(string objectKey) where T : new();
        string Save(string objectKey, string json);
    }
}
