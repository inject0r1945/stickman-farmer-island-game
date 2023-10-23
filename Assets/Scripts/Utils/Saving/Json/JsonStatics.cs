using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonStatics
{
    private static string _xAxisName = "x";
    private static string _yAxisName = "y";
    private static string _zAxisName = "z";

    public static JToken ToToken(this Vector3 vector)
    {
        JObject state = new JObject();
        IDictionary<string, JToken> stateDictionary = state;

        stateDictionary[_xAxisName] = vector.x;
        stateDictionary[_yAxisName] = vector.y;
        stateDictionary[_zAxisName] = vector.z;

        return state;
    }

    public static Vector3 ToVector3(this JToken state)
    {
        Vector3 vector = new Vector3();

        if (state is JObject jObject)
        {
            IDictionary<string, JToken> stateDictionary = jObject;

            if (stateDictionary.TryGetValue(_xAxisName, out JToken x))
            {
                vector.x = x.ToObject<float>();
            }

            if (stateDictionary.TryGetValue(_yAxisName, out JToken y))
            {
                vector.y = y.ToObject<float>();
            }

            if (stateDictionary.TryGetValue(_zAxisName, out JToken z))
            {
                vector.z = z.ToObject<float>();
            }
        }

        return vector;
    }
}
