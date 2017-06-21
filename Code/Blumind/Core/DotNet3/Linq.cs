using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    static class Enumerable
    {
        public static decimal Average(this IEnumerable<decimal> source)
        {
            if (source == null)
                throw new NullReferenceException();

            decimal num = 0M;
            long num2 = 0L;
            foreach (decimal num3 in source)
            {
                num += num3;
                num2 += 1L;
            }
            
            if (num2 <= 0L)
                throw new InvalidOperationException();

            return (num / num2);
        }

        public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
                throw new NullReferenceException();

            return source.Select<TSource, decimal>(selector).Average();
        }

        public static float Average(this IEnumerable<float> source)
        {
            if (source == null)
                throw new NullReferenceException();

            double num = 0.0;
            long num2 = 0L;
            foreach (float num3 in source)
            {
                num += num3;
                num2 += 1L;
            }
            
            if (num2 <= 0L)
                throw new InvalidOperationException();

            return (float)(num / ((double)num2));
        }

        public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source == null)
                throw new NullReferenceException();

            return source.Select<TSource, float>(selector).Average();
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
                throw new NullReferenceException();

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            foreach (var s in source)
            {
                if (comparer.Equals(s, value))
                    return true;
            }

            return false;
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            var coll = source as ICollection<TSource>;
            if (coll != null)
                return coll.Contains(value);
            else
                return source.Contains<TSource>(value, null);
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            var list = source as IList<TSource>;
            if (list != null)
            {
                if (list.Count > 0)
                {
                    return list[0];
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        return enumerator.Current;
                    }
                }
            }

            throw new InvalidOperationException();
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            var list = source as IList<TSource>;
            if (list != null)
            {
                int count = list.Count;
                if (count > 0)
                {
                    return list[count - 1];
                }
            }
            else
            {
                using (var enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        TSource current;
                        do
                        {
                            current = enumerator.Current;
                        }
                        while (enumerator.MoveNext());
                        return current;
                    }
                }
            }

            throw new InvalidOperationException();
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            return Union<TSource>(first, second, null);
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            if (first == null || second == null)
                throw new ArgumentNullException();

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            foreach (var item in first)
                yield return item;

            foreach (var item in second)
            {
                if (!first.Contains(item, comparer))
                    yield return item;
            }
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new NullReferenceException();

            if (predicate == null)
                throw new ArgumentNullException();

            foreach (var item in source)
                if (predicate(item))
                    yield return item;
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            var list = new List<TSource>(source);
            return list.ToArray();
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            return new List<TSource>(source);
        }

        public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keyGetter, Func<TSource, TValue> valueGetter)
        {
            if (source == null)
                throw new NullReferenceException();

            if (keyGetter == null || valueGetter == null)
                throw new ArgumentNullException();

            var dic = new Dictionary<TKey, TValue>();
            foreach (var s in source)
            {
                var key = keyGetter(s);
                var value = valueGetter(s);
                if (!dic.ContainsKey(key))
                    dic.Add(key, value);
            }

            return dic;
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keyGetter)
        {
            if (source == null)
                throw new NullReferenceException();

            if (keyGetter == null)
                throw new ArgumentNullException();

            var dic = new Dictionary<TKey, TSource>();
            foreach (var s in source)
            {
                var key = keyGetter(s);
                if (!dic.ContainsKey(key))
                    dic.Add(key, s);
            }

            return dic;
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            return Distinct<TSource>(source, null);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null)
                throw new NullReferenceException();

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            var history = new List<TSource>();
            foreach (var item in source)
            {
                if (!history.Contains(item, comparer))
                {
                    yield return item;
                    history.Add(item);
                }
            }
        }

        public static IEnumerable<int> Range(int start, int count)
        {
            long num = (start + count) - 1L;
            if ((count < 0) || (num > 0x7fffffffL))
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = start; i < num; i++)
            {
                yield return i;
            }
        }

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            var array = source.ToArray();
            for (int i = array.Length - 1; i >= 0; i--)
                yield return array[i];
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            var comparer = Comparer<TSource>.Default;
            var y = default(TSource);
            if (y == null)
            {
                foreach (var item in source)
                {
                    if ((item != null) && ((y == null) || (comparer.Compare(item, y) > 0)))
                    {
                        y = item;
                    }
                }
                return y;
            }

            bool flag = false;
            foreach (var item in source)
            {
                if (flag)
                {
                    if (comparer.Compare(item, y) > 0)
                    {
                        y = item;
                    }
                }
                else
                {
                    y = item;
                    flag = true;
                }
            }

            if (!flag)
                throw new InvalidOperationException();

            return y;
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            var comparer = Comparer<TSource>.Default;
            var y = default(TSource);
            if (y == null)
            {
                foreach (var item in source)
                {
                    if ((item != null) && ((y == null) || (comparer.Compare(item, y) < 0)))
                    {
                        y = item;
                    }
                }
                return y;
            }

            bool flag = false;
            foreach (var item in source)
            {
                if (flag)
                {
                    if (comparer.Compare(item, y) < 0)
                    {
                        y = item;
                    }
                }
                else
                {
                    y = item;
                    flag = true;
                }
            }

            if (!flag)
                throw new InvalidOperationException();

            return y;
        }

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select<TSource, TResult>(selector).Max<TResult>();
        }

        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source.Select<TSource, TResult>(selector).Min<TResult>();
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new NullReferenceException();

            if (selector == null)
                throw new ArgumentNullException();

            foreach (var item in source)
                yield return selector(item);
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
                throw new NullReferenceException();

            return source.Select<TSource, int>(selector).Sum();
        }

        public static int Sum(this IEnumerable<int> source)
        {
            if (source == null)
                throw new NullReferenceException();

            int num = 0;
            foreach (int num2 in source)
            {
                num += num2;
            }
            return num;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
                throw new NullReferenceException();

            var is2 = source as ICollection<TSource>;
            if (is2 != null)
            {
                return is2.Count;
            }

            int num = 0;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    num++;
                }
            }
            return num;
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            if (source == null)
                throw new NullReferenceException();

            var enumerable = source as IEnumerable<TResult>;
            if (enumerable != null)
            {
                return enumerable;
            }

            return CastTo<TResult>(source);
        }

        static IEnumerable<TResult> CastTo<TResult>(IEnumerable source)
        {
            foreach (var item in source)
            {
                if (item is TResult)
                    yield return (TResult)item;
                else
                    throw new InvalidCastException();
            }
        }

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            if (source == null)
                throw new NullReferenceException();

            foreach (var s in source)
            {
                if (s is TResult)
                    yield return (TResult)s;
            }
        }

        public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null)
                throw new NullReferenceException();

            if (keySelector == null)
                throw new ArgumentNullException();

            var list = new List<TSource>(source);
            var comparer = Comparer<TKey>.Default;
            list.Sort((s1, s2) => comparer.Compare(keySelector(s1), keySelector(s2)));
            for (int i = list.Count - 1; i >= 0; i--)
                yield return list[i];
        }

        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source == null)
                throw new NullReferenceException();

            if (keySelector == null)
                throw new ArgumentNullException();

            var list = new List<TSource>(source);
            var comparer = Comparer<TKey>.Default;
            list.Sort((s1, s2) => comparer.Compare(keySelector(s1), keySelector(s2)));
            foreach (var item in list)
                yield return item;
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null)
                throw new NullReferenceException();

            if (selector == null)
                throw new ArgumentNullException();

            foreach (var item in source)
            {
                foreach (var item2 in selector(item))
                    yield return item2;
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this IEnumerable<TSource> source, 
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            if (source == null)
                throw new NullReferenceException();

            if (collectionSelector == null || resultSelector == null)
                throw new ArgumentNullException();

            foreach (var item in source)
            {
                foreach (var item2 in collectionSelector(item))
                {
                    yield return resultSelector(item, item2);
                }
            }
        }

        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new NullReferenceException();

            int i = 0;
            foreach (var item in source)
            {
                if (count > i)
                    yield return item;
                else
                    break;
                i++;
            }
        }
    }
}
