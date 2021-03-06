using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2017.Utils
{
    public static class EnumerableExtensions
    {
        public static void EnqueueAll<T>(this Queue<T> self, IEnumerable<T> data)
        {
            foreach(var item in data) self.Enqueue(item);
        }

        public static long Product(this IEnumerable<long> self)
        {
            return self.Aggregate((current, value) => current * value);
        }

        public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> self, int magnitude)
        {
            var list = self.ToList();
            magnitude %= list.Count;
            foreach (var item in list.Skip(list.Count - magnitude)) yield return item;
            foreach (var item in list.Take(list.Count - magnitude)) yield return item;
        }

        public static IEnumerable<T> RotateLeft<T>(this IEnumerable<T> self, int magnitude)
        {
            var list = self.ToList();
            magnitude %= list.Count;
            foreach (var item in list.Skip(magnitude)) yield return item;
            foreach (var item in list.Take(magnitude)) yield return item;
        }

        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> self) => self.ToList();

        public static IEnumerable<T> AppendAll<T>(this IEnumerable<T> self, IEnumerable<T> other)
        {
            foreach (var item in self) yield return item;
            foreach (var item in other) yield return item;
        }

        public static IEnumerable<(T first, T second)> Pairs<T>(this IEnumerable<T> self)
        {
            var l = self.ToList();
            for (var first = 0; first < l.Count - 1; first++)
            {
                for (var second = first + 1; second < l.Count; second++)
                {
                    yield return (l[first], l[second]);
                }
            }
        }

        public static List<T> ListFromItem<T>(T item)
        {
            return new List<T> { item };
        }

        public static Dictionary<TKey, List<T>> GroupToDictionary<T, TKey>(this IEnumerable<T> input, Func<T, TKey> keyFunc)
        {
            return input.GroupBy(keyFunc).ToDictionary(it => it.Key, it => it.ToList());
        }

        public static Dictionary<T, List<T>> GroupToDictionary<T>(this IEnumerable<T> input)
        {
            return input.GroupToDictionary(it => it);
        }

        public static T Mode<T>(this IEnumerable<T> self)
            => self.GroupBy(value => value).MaxBy(it => it.Count()).Key;

        public static IEnumerable<T> Modes<T>(this IEnumerable<T> self)
        {
            var groups = self.GroupBy(value => value).ToList();
            var max = groups.MaxBy(it => it.Count()).Count();
            return groups.Where(it => it.Count() == max).Select(it => it.Key);
        }

        // Flips rows and columns, eg:
        // [ [ 1, alpha, foo], [2, beta, bar] ] => [ [1, 2], [alpha, beta], [foo, bar] ]
        public static IEnumerable<List<T>> Flip<T>(this IEnumerable<IEnumerable<T>> self)
        {
            return self.Aggregate(new List<List<T>>(), (accum, current) =>
            {
                if (!accum.Any())
                {
                    return current.Select(it => new List<T> { it }).ToList();
                }

                foreach (var (first, second) in accum.Zip(current))
                {
                    first.Add(second);
                }

                return accum;
            });
        }

        public static TSource MaxBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector) where TSource : notnull
        {
            var comparer = Comparer<TKey>.Default;
            return source.ArgBy(keySelector, lag => comparer.Compare(lag.Current, lag.Previous) > 0);
        }

        public static TSource MinBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector) where TSource : notnull
        {
            var comparer = Comparer<TKey>.Default;
            return source.ArgBy(keySelector, lag => comparer.Compare(lag.Current, lag.Previous) < 0);
        }

        public static TSource ArgBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<(TKey Current, TKey Previous), bool> predicate) where TSource : notnull
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (!source.Any()) throw new InvalidOperationException("Sequence contains no elements");

            var value = source.First();
            var key = keySelector(value);

            bool hasValue = false;
            foreach (var other in source)
            {
                var otherKey = keySelector(other);
                if (otherKey == null) continue;

                if (hasValue)
                {
                    if (predicate((otherKey, key)))
                    {
                        value = other;
                        key = otherKey;
                    }
                }
                else
                {
                    value = other;
                    key = otherKey;
                    hasValue = true;
                }
            }
            if (hasValue)
            {
                return value;
            }
            throw new InvalidOperationException("Sequence contains no elements");
        }

        public static IEnumerable<List<T>> InGroupsOf<T>(this IEnumerable<T> self, int groupSize)
        {
            var result = new List<T>();
            foreach (var item in self)
            {
                result.Add(item);
                if (result.Count == groupSize)
                {
                    yield return result;
                    result = new List<T>();
                } 
            }

            if (result.Any()) yield return result;
        }

        public static string Join<T>(this IEnumerable<T> self, string separator = "") =>
            string.Join(separator, self.Select(it => it?.ToString()));

        public static List<string> Lines(this string input) =>
            input.Split("\n")
                .Select(it => it.Trim()).ToList();

        public static IEnumerable<List<T>> Choose<T>(this IEnumerable<T> self, int k)
        {
            if (k <= 0) yield break;
            var l = self.ToList();
            if (l.Count == 0 || k > l.Count) yield break;
            if (k == l.Count)
            {
                yield return l;
                yield break;
            }

            if (k == 1)
            {
                foreach (var item in l) yield return new List<T> { item };
                yield break;
            }

            var first = l[0];
            foreach (var item in l.Skip(1).Choose(k - 1))
            {
                var result = new List<T> { first };
                result.AddRange(item);
                yield return result;
            }

            foreach (var item in l.Skip(1).Choose(k)) yield return item;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> first, IEnumerable<T> other)
        {
            return !other.Except(first).Any();
        }

        public static IEnumerable<List<string>> SplitIntoParagraphs(this string input)
        {
            var lines = input.Lines();
            var p = new List<string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (p.Any()) yield return p;
                    p = new List<string>();
                }
                else
                {
                    p.Add(line);
                }
            }
            if (p.Any()) yield return p;
        }

        public static IEnumerable<T> Range<T>(T first, int count, Func<T, T> generateNext)
        {
            var current = first;
            while (count-- > 0)
            {
                yield return current;
                if (count > 0)
                {
                    current = generateNext(current);
                }
            }
        }

        public static IEnumerable<(T Value, int Index)> WithIndices<T>(this IEnumerable<T> self) =>
            self.Select((it, index) => (it, index));

        public static int FirstIndex<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            return self.WithIndices().First(it => predicate(it.Value)).Index;
        }

        public static T Pop<T>(this List<T> self)
        {
            var result = self.Last();
            self.RemoveAt(self.Count - 1);
            return result;
        }

        public static T Shift<T>(this List<T> self)
        {
            if (self.Any())
            {
                var result = self.First();
                self.RemoveAt(0);
                return result;
            }
            throw new ApplicationException("Attempt to shift empty list.");
        }

        public static LinkedListNode<T> Roll<T>(this LinkedListNode<T> self)
        {
            return self.Next ?? self.List!.First!;
        }

        public static LinkedListNode<T> Forward<T>(this LinkedListNode<T> self, int n)
        {
            var result = self;
            while (n-- > 0) result = result.Roll();
            return result;
        }
    }
}