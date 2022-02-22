using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2017.Days.Day15
{
    [UsedImplicitly]
    public class Day15: IAdventOfCode
    {
        public void Run()
        {
            Do1(65, 8921).Should().Be(588);
            Do1(591, 393).Should().Be(619);

            Do2(65, 8921).Should().Be(309);
            Do2(591, 393).Should().Be(290);
        }

        private long Do1(long start1, long start2)
        {
            var v1 = start1;
            var v2 = start2;
            var count = 0;
            foreach (var _ in Enumerable.Range(0, 40_000_000))
            {
                v1 = (v1 * 16807) % 2147483647;
                v2 = (v2 * 48271) % 2147483647;
                if ((v1 & 0xFFFF) == (v2 & 0xFFFF)) count += 1;
            }

            return count;
        }

        private long Do2(long start1, long start2)
        {
            var v1temp = Generate(start1, 16807, 4);
            var v2temp = Generate(start2, 48271, 8);
            var zipped = v1temp.Zip(v2temp);
            var count = 0;
            foreach (var (v1, v2) in zipped.Take(5_000_000))
            {
                if ((v1 & 0xFFFF) == (v2 & 0xFFFF)) count += 1;
            }

            return count;
        }

        private IEnumerable<long> Generate(long start, long multiplier, long consider)
        {
            var value = start;
            while (true)
            {
                value = (value * multiplier) % 2147483647;
                if (value % consider == 0) yield return value;
            }
        }
    }
}