using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using FluentAssertions.Equivalency;
using JetBrains.Annotations;

namespace AdventOfCode2017.Days.Day17
{
    [UsedImplicitly]
    public class Day17: IAdventOfCode
    {
        public void Run()
        {
            var input = 329;
            Do0(3, 9).Should().Equal(0, 9, 5, 7, 2, 4, 3, 8, 6, 1);
            Do1(3, 2017, 2017).Should().Be(638);
            Do1(input, 2017, 2017).Should().Be(725);

            Do1(input, 50_000_000, 0).Should().Be(0);
        }

        private LinkedList<int> Do0(int step, int iterations)
        {
            var ll = new LinkedList<int>();
            ll.AddFirst(0);
            var current = ll.First;
            foreach (var value in Enumerable.Range(1, iterations))
            {
                current = current!.Forward(step);
                ll.AddAfter(current, value);
                current = current.Roll();
                if (value % 10_000 == 0) Console.WriteLine(value);
            }

            return ll;
        }

        private int Do1(int step, int iterations, int afterValue)
        {
            var ll = Do0(step, iterations);
            for (var node = ll.First; node != null; node = node.Next)
            {
                if (node.Value == afterValue)
                {
                    return node.Roll().Value;
                }
            }

            throw new ApplicationException();
        }
    }

    public class TernaryTree
    {
        private TernaryTree? Before = null;
        private TernaryTree? After = null;
        private readonly int Current;

        public int Count { get; private set; } = 1;

        public TernaryTree(int current)
        {
            Current = current;
        }

        public void AddAfter(int position, int value)
        {
            Count += 1;
            if (Before is { } && position < Before.Count)
            {
                Before.AddAfter(position, value);
                return;
            }

            position -= Before?.Count ?? 0;

            if (position == 0)
            {
                After = new TernaryTree(value)
                {
                    After = After
                };
                return;
            }

            After!.AddAfter(position - 1, value);
        }
    }
}