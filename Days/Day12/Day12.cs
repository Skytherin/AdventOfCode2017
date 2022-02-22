using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day12
{
    [UsedImplicitly]
    public class Day12: IAdventOfCode
    {
        public void Run()
        {
            Do1(@"0 <-> 2
1 <-> 1
2 <-> 0, 3, 4
3 <-> 2, 4
4 <-> 2, 3, 6
5 <-> 6
6 <-> 4, 5").Should().Be(6);

            Do1(this.Input()).Should().Be(152);

            Do2(@"0 <-> 2
1 <-> 1
2 <-> 0, 3, 4
3 <-> 2, 4
4 <-> 2, 3, 6
5 <-> 6
6 <-> 4, 5").Should().Be(2);

            Do2(this.Input()).Should().Be(186);
        }

        private long Do1(string input)
        {
            var programs = TypeCompiler.ParseLines<Data>(input)
                .ToDictionary(it => it.Program, it => it.Clients);

            var reached = FindGroup(programs, 0);

            return reached.LongCount();
        }

        private static HashSet<int> FindGroup(Dictionary<int, IReadOnlyList<int>> programs, int start)
        {
            var open = new Queue<int>(new[] { start });
            var reached = new HashSet<int>();
            while (open.TryDequeue(out var current))
            {
                if (!reached.Add(current)) continue;
                open.EnqueueAll(programs[current]);
            }

            return reached;
        }

        private long Do2(string input)
        {
            var programs = TypeCompiler.ParseLines<Data>(input)
                .ToDictionary(it => it.Program, it => it.Clients);

            var groupCount = 0L;

            while (programs.Any())
            {
                groupCount++;
                var closed = FindGroup(programs, programs.Keys.First());
                foreach (var item in closed)
                {
                    programs.Remove(item);
                }
            }

            return groupCount;
        }


        private record Data(
            int Program,
            [Format(Separator = ",", Before = "<->")]
            IReadOnlyList<int> Clients
        );
    }
}
