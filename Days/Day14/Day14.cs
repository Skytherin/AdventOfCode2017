using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2017.Days.Day14
{
    [UsedImplicitly]
    public class Day14: IAdventOfCode
    {
        public void Run()
        {
            Do1("flqrgnkx").Should().Be(8108);
            Do1("ugkiagan").Should().Be(8292);

            Do2("flqrgnkx").Should().Be(1242);
            Do2("ugkiagan").Should().Be(1069);
        }

        private long Do1(string key)
        {
            var count = 0;
            for (var row = 0; row < 128; row++)
            {
                var temp = Day10.Day10.Knot64($"{key}-{row}");
                var temp2 = HexToBinary(temp);
                count += temp2.Count(c => c == '1');
            }

            return count;
        }

        private long Do2(string key)
        {
            var grid = new List<List<int>>();
            const int free = -2;
            const int used = -1;
            foreach (var row in Enumerable.Range(0, 128))
            {
                var temp = Day10.Day10.Knot64($"{key}-{row}");
                var temp2 = HexToBinary(temp);
                grid.Add(temp2.Select(it => it == '1' ? used : free).ToList());
            }

            var nextGroup = 0;

            foreach (var row in Enumerable.Range(0, 128))
            {
                foreach (var col in Enumerable.Range(0, 128))
                {
                    if (grid[row][col] == used)
                    {
                        var thisGroup = nextGroup++;
                        var open = new Queue<(int row, int col)>();
                        open.Enqueue((row, col));
                        while (open.TryDequeue(out var next))
                        {
                            if (grid[next.row][next.col] == used)
                            {
                                grid[next.row][next.col] = thisGroup;
                                open.EnqueueAll(Adjacent(next.row, next.col));
                            }
                        }
                    }
                }
            }

            return nextGroup;
        }

        private IEnumerable<(int, int)> Adjacent(int row, int col)
        {
            foreach (var (dy, dx) in new[]
                     {
                         (1,0),(0,1),(-1,0),(0,-1)
                     })
            {
                if (row + dy is {} y && y is >= 0 and < 128 &&
                    col + dx is {} x && x is >= 0 and < 128)
                {
                    yield return (y, x);
                }
            }
        }

        private string HexToBinary(string hexString) => 
            hexString.Select(
                c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
            ).Join();
    }
}