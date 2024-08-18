using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class CollectionExtensions
    {
        public static T PickRandomElement<T>(this T[] array)
        {
            var randomIndex = Random.Range(0, array.Length);

            return array[randomIndex];
        }
        
        public static T PickRandomElement<T>(this List<T> list)
        {
            var randomIndex = Random.Range(0, list.Count);

            return list[randomIndex];
        }
    }
}