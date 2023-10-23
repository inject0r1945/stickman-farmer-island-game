using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Saving
{
    public interface IUniqueIdentifiable
    {
        public string GetUniqueIdentifivatorVariableName();

        public string GetUniqueIdentificator();
    }
}

