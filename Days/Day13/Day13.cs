using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day13
{
    [UsedImplicitly]
    public class Day13: IAdventOfCode
    {
        public void Run()
        {
            var example = @"0: 3
1: 2
4: 4
6: 4";
            Do1(example).Should().Be(24);
            Do1(this.Input()).Should().Be(1928);
            Do2(example).Should().Be(10);
            Do2(this.Input()).Should().Be(3830344L);
        }

        private long Do1(string input)
        {
            var layers = TypeCompiler.Parse<Dictionary<int, int>>(input);
            var score = 0;
            foreach (var (layer, depth) in layers)
            {
                var mod = (depth - 1) * 2;
                if (layer % mod == 0) score += layer * depth;
            }

            return score;
        }

        private long Do2(string input)
        {
            var layers = TypeCompiler.Parse<Dictionary<int, int>>(input);

            for (var delay = 0; delay < int.MaxValue; delay++)
            {
                if (layers.All((item) =>
                    {
                        var (layer, depth) = item;
                        var mod = (depth - 1) * 2;
                        return (delay + layer) % mod != 0;
                    })) return delay;
            }

            throw new ApplicationException();
        }
    }
}