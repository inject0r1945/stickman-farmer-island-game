using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCGame.Utils.Static
{
    public static class StaticUtils
    {
        public static T GetUniqueComponentOnScene<T>() where T : Component
        {
            T[] findedComponents = Object.FindObjectsOfType<T>();

            int maxComponentsOnScene = 1;

            if (findedComponents.Length > maxComponentsOnScene)
                throw new System.Exception($"На сцене присутствует более {maxComponentsOnScene} компонента {typeof(T)}");
            else if (findedComponents.Length == 0)
                throw new System.Exception($"Не найден компонент {typeof(T)} на сцене");

            return findedComponents[0];
        }

        public static bool IsBitsSet(int number, int bitPositions)
        {
            if (bitPositions < 0)
                throw new System.ArgumentOutOfRangeException(nameof(bitPositions));

            if (number < 0)
                throw new System.ArgumentOutOfRangeException(nameof(number));

            if (bitPositions != 0)
            {
                if ((number & bitPositions) == bitPositions)
                    return true;
            }
            else
            {
                if (number == 0)
                    return true;
            }

            return false;
        }
    }
}