using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day10
{
    [UsedImplicitly]
    public class Day10: IAdventOfCode
    {
        private readonly List<int> Input = TypeCompiler.Parse<List<int>>("197,97,204,108,1,29,5,71,0,50,2,255,248,78,254,63", new Format { Separator = ","});
            

        public void Run()
        {
            Do1(Enumerable.Range(0, 5), new[] { 3, 4, 1, 5 }).Should().Be(12);
            Do1(Enumerable.Range(0, 256), Input).Should().Be(40132);
            Knot64("").Should().Be("a2582a3a0e66e6e86e3812dcb672a272");
            Knot64("AoC 2017").Should().Be("33efeb34ea91902bb2f59c9920caa6cd");
            Knot64("1,2,3").Should().Be("3efbe78a8d82f29979031a4aa0b16a9d");
            Knot64("1,2,4").Should().Be("63960835bcdc130f0b66d7ff4f6a5a8e");
            Knot64("197,97,204,108,1,29,5,71,0,50,2,255,248,78,254,63").Should().Be("35b028fe2c958793f7d5a61d07a008c8");
        }

        private static long Do1(IEnumerable<int> range, IEnumerable<int> lengths)
        {
            IReadOnlyList<int> list = range.ToList();
            (list, _) = Knot(lengths, list, (0,0));

            return list[0] * list[1];
        }

        private static (IReadOnlyList<int>, (int cursor, int skipSize)) Knot(IEnumerable<int> lengths, IReadOnlyList<int> list, (int cursor, int skipSize) context)
        {
            var cursor = context.cursor;
            var skipSize = context.skipSize;
            foreach (var length in lengths)
            {
                list = Reverse(list, length, cursor);
                cursor = (cursor + length + skipSize) % list.Count;
                skipSize += 1;
            }

            return (list, (cursor, skipSize));
        }

        public static string Knot64(string input)
        {
            var lengths = input.Select(it => (int)it).Concat(new[]{ 17, 31, 73, 47, 23 }).ToList();
            var context = (0, 0);
            IReadOnlyList<int> l = Enumerable.Range(0, 256).ToList();
            for (var i = 0; i < 64; i++)
            {
                (l, context) = Knot(lengths, l, context);
            }

            var dense = new List<int>();
            for (var i = 0; i < 16; i++)
            {
                dense.Add(l.Skip(i * 16).Take(16).Aggregate((current, next) => current ^ next));
            }

            return dense.Select(it => it.ToString("X2")).Join().ToLower();
        }

        private static IReadOnlyList<int> Reverse(IReadOnlyList<int> list, int length, int cursor)
        {
            var l = list.ToList();
            var head = cursor;
            while (length > 1)
            {
                var tail = (head + length - 1) % list.Count;
                (l[head], l[tail]) = (l[tail], l[head]);
                head = (head + 1) % list.Count;
                length -= 2;
            }

            return l;
        }
    }
}
