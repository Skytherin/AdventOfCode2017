using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using TypeParser;

namespace AdventOfCode2017.Days.Day02
{
    public class Day02: IAdventOfCode
    {
        public void Run()
        {
            var input = TypeCompiler.ParseLines<List<int>>(this.Input());

            Do1(input).Should().Be(51139);
            Do2(input).Should().Be(272);
        }

        private int Do1(List<List<int>> input)
        {
            return input.Sum(line => line.Max() - line.Min());
        }

        private int Do2(List<List<int>> input)
        {
            return input.Sum(line => line.Choose(2).Select(pair =>
            {
                var (numerator, denominator) = (pair.Max(), pair.Min());
                if (numerator % denominator == 0) return numerator / denominator;
                return (int?)null;
            })
                .OfType<int>()
                .First());
        }
    }
}