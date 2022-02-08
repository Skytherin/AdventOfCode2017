using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day05
{
    [UsedImplicitly]
    public class Day05: IAdventOfCode
    {
        public void Run()
        {
            var input = TypeCompiler.Parse<IReadOnlyList<int>>(this.Input());
            Do1(input.ToList()).Should().Be(381680L);
            Do2(input.ToList()).Should().Be(29717847L);
        }

        private long Do1(List<int> input)
        {
            var steps = 0;
            var pc = 0;
            while (pc < input.Count)
            {
                steps++;
                var next = pc + input[pc];
                input[pc] += 1;
                pc = next;
            }

            return steps;
        }

        private long Do2(List<int> input)
        {
            var steps = 0;
            var pc = 0;
            while (pc < input.Count)
            {
                steps++;
                var next = pc + input[pc];
                input[pc] += input[pc] < 3 ? 1 : -1;
                pc = next;
            }

            return steps;
        }
    }
}