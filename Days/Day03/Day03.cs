using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;

namespace AdventOfCode2017.Days.Day03
{
    public class Day03: IAdventOfCode
    {
        public void Run()
        {
            var spiral = CreateSpiral(277678);
            spiral.First(it => it.Value == 277678).Key.ManhattanDistance().Should().Be(475);

            CreateSpiral2().SkipWhile(it => it.Item2 <= 277678).First().Item2.Should().Be(279138);
        }

        private Dictionary<Position, int> CreateSpiral(int max)
        {
            var d = new Dictionary<Position, int> { { Position.Zero, 1 } };
            var current = Position.Zero;
            var direction = Vector.South;

            for (var i = 2; i <= max; i++)
            {
                var next = NextVector(direction);
                if (!d.ContainsKey(current + next))
                {
                    d.Add(current + next, i);
                    direction = next;
                    current += next;
                }
                else
                {
                    d.Add(current + direction, i);
                    current += direction;
                }
            }

            return d;
        }

        private IEnumerable<(Position, int)> CreateSpiral2()
        {
            var d = new Dictionary<Position, int> { { Position.Zero, 1 } };
            var current = Position.Zero;
            var direction = Vector.South;

            for (var i = 2; i < int.MaxValue; i++)
            {
                var next = NextVector(direction);
                if (!d.ContainsKey(current + next))
                {
                    d.Add(current + next, Sum(d, current + next));
                    direction = next;
                    current += next;
                }
                else
                {
                    d.Add(current + direction, Sum(d, current + direction));
                    current += direction;
                }

                yield return (current, d[current]);
            }
        }

        private int Sum(IReadOnlyDictionary<Position, int> d, Position p)
        {
            return p.Orthogonal().AppendAll(p.Diagonal())
                .Sum(neighbor => d.TryGetValue(neighbor, out var temp) ? temp : 0);
        }

        private Vector NextVector(Vector inp)
        {
            return new Vector(-inp.dX, inp.dY);
        }

    }
}