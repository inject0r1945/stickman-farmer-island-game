using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Utils.Saving.Json
{
    public abstract class SavingStrategy : ScriptableObject
    {
        public abstract void Save(JObject State);

        public abstract JObject Load();

        public abstract void Delete();
    }
}
