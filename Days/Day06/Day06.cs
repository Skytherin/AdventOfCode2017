using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day06
{
    [UsedImplicitly]
    public class Day06: IAdventOfCode
    {
        public void Run()
        {
            var input = TypeCompiler.Parse<IReadOnlyList<int>>(this.Input());
            Do1(new []{ 0, 2, 7, 0 }.ToList()).Should().Be((5, 4));
            Do1(input).Should().Be((11137, 1037));
        }

        private (int count, int cycleTime) Do1(IReadOnlyList<int> input)
        {
            var hs = new Dictionary<string, int> { { input.Select(it => it.ToString()).Join(","), 0 } };
            var count = 0;
            while (true)
            {
                count += 1;
                input = Redistribute(input);
                var key = input.Select(it => it.ToString()).Join(",");
                if (hs.ContainsKey(key)) return (count, count - hs[key]);
                hs.Add(key, count);
            }
        }

        private static List<int> Redistribute(IReadOnlyList<int> input)
        {
            var max = input.Max();
            var index = input.WithIndices().First(it => it.Value == max).Index;

            var n = max / input.Count;
            var r = max % input.Count;

            var result = input.Select(it => it + n).ToList();
            result[index] -= input[index];
            for (var i = 1; i <= r; i++)
            { 
                result[(index + i) % input.Count] += 1;
            }

            return result;
        }
    }
}