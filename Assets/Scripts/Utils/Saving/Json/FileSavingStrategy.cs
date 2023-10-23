using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Saving.Json
{
    public class FileSavingStrategy : SavingStrategy
    {
        public override JObject Load()
        {
            throw new System.NotImplementedException();
        }

        public override void Save(JObject State)
        {
            throw new System.NotImplementedException();
        }

        public override void Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}
