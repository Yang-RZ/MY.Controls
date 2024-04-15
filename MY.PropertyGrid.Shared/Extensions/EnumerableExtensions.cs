
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System;
using System.Linq;

namespace MY.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
                                    => self.Select((item, index) => (item, index));

        public static string ToString<T>(this IEnumerable<T> dValues, string sSparator = ",")
        {
            return string.Join(sSparator, dValues.Select(v => v.ToString()));
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            var array = enumerable.ToArray();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];
                action(item);
            }

            return array;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var array = enumerable.ToArray();
            for (var i = 0; i < array.Length; i++) action(array[i], i);

            return array;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ObservableCollection<T>(enumerable);
        }

        public static bool ContainsWithCondition<T>(this IEnumerable<T> enumerable, Func<T, bool> func)
        {
            foreach (var item in enumerable)
            {
                if (func(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AllItemWithCondition<T>(this IEnumerable<T> enumerable, Func<T, bool> func)
        {
            bool bRet = true;
            foreach (var item in enumerable)
            {
                bRet = bRet && func(item);
            }

            return bRet;
        }

        public static void AddRangeWithoutDuplication<T>(this List<T> listOld, List<T> listNew) where T : ICloneable
        {
            foreach (var item in listNew)
            {
                if (listOld.Contains(item))
                {
                    listOld.Add(item);
                }
            }
        }
    }
}