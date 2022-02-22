using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day11
{
    [UsedImplicitly]
    public class Day11: IAdventOfCode
    {
        private static IReadOnlyList<Direction> Parse(string input) => TypeCompiler.Parse<IReadOnlyList<Direction>>(input, new Format{Separator = ","});

        public void Run()
        {
            Do1("ne").Last().Should().Be(1);
            Do1("nw").Last().Should().Be(1);
            Do1("s").Last().Should().Be(1);
            Do1("n").Last().Should().Be(1);
            Do1("se").Last().Should().Be(1);
            Do1("sw").Last().Should().Be(1);

            Do1("ne,n").Last().Should().Be(2);
            Do1("nw,n").Last().Should().Be(2);
            Do1("s,se").Last().Should().Be(2);
            Do1("n,nw").Last().Should().Be(2);
            Do1("se,s").Last().Should().Be(2);
            Do1("sw,s").Last().Should().Be(2);

            Do1("ne,ne,ne").Last().Should().Be(3);
            Do1("ne,ne,sw,sw").Last().Should().Be(0);
            Do1("ne,ne,s,s").Last().Should().Be(2);
            Do1("se,sw,se,sw,sw").Last().Should().Be(3);
            Do1(this.Input()).Last().Should().Be(707);
            Do1(this.Input()).Max().Should().Be(1490);
        }

        private static IEnumerable<long> Do1(string input)
        {
            var steps = Parse(input);

            var current = (0, 0);
            foreach (var step in steps)
            {
                var delta = Delta(step);
                var next = (current.Item1 + delta.Item1, current.Item2 + delta.Item2);
                current = next;
                yield return HexManhattan(current);
            }
        }

        private static long HexManhattan((int, int) current, (int, int)? origin = null)
        {
            var x1 = current.Item1;
            var y1 = current.Item2;
            var x0 = origin?.Item1 ?? 0;
            var y0 = origin?.Item2 ?? 0;
            var dx = x1 - x0;
            var dy = y1 - y0;

            return MoreMath.Sign(dx) == MoreMath.Sign(dy) 
                ? Math.Abs(dx) + Math.Abs(dy) 
                : Math.Max(Math.Abs(dx), Math.Abs(dy))
                ;
        }

        private static (int, int) Delta(Direction step)
        {
            return step switch
            {
                Direction.n => (-1, 0),
                Direction.s => (1, 0),
                Direction.ne => (-1, 1),
                Direction.nw => (0, -1),
                Direction.se => (0, 1),
                Direction.sw => (1, -1),
                _ => throw new ArgumentOutOfRangeException(nameof(step), step, null)
            };
        }

        private enum Direction
        {
            ne,
            se,
            sw,
            nw,
            n,
            s
        }
    }
}