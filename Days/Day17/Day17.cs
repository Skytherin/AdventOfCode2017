using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
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

            Do2(input, 50_000_000).Should().Be(27361412);
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
            }

            return ll;
        }

        private int Do1(int step, int iterations, int afterValue)
        {
            var ll = Do0(step, iterations);
            return ll.SkipWhile(it => it != afterValue).Skip(1).First();
        }

        private int Do2(int step, int iterations)
        {
            var currentLength = 1;
            var zeroIndex = 0;
            var current = 0;
            var afterZero = 0;
            foreach (var value in Enumerable.Range(1, iterations))
            {
                current = (current + step) % currentLength;

                if (current == zeroIndex)
                {
                    afterZero = value;
                }

                if (current < zeroIndex)
                {
                    zeroIndex += 1;
                }

                current += 1;
                currentLength += 1;
            }

            return afterZero;
        }
    }
    
}