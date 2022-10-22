using System;
using System.Collections;
using System.Collections.Generic;

namespace SunAndMoon
{
    public class RandomHelper
    {
        public static T RandomInArray<T>(IList<T> array, int exclude = -1)
        {
            var randIndex = -1;
            while (true)
            {
                randIndex = UnityEngine.Random.Range(0, array.Count);
                if (exclude < 0 || randIndex != exclude)
                {
                    break;
                }
            }
            return array[randIndex];
        }
    }
}