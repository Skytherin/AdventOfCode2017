using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2017.Days.Day01
{
    [UsedImplicitly]
    public class Day01: IAdventOfCode
    {
        public void Run()
        {
            Do1("1122").Should().Be(3);
            Do1("1111").Should().Be(4);
            Do1("1234").Should().Be(0);
            Do1("91212129").Should().Be(9);
            Do1(this.Input()).Should().Be(1034);

            Do2("1212").Should().Be(6);
            Do2("1221").Should().Be(0);
            Do2("123425").Should().Be(4);
            Do2("123123").Should().Be(12);
            Do2("12131415").Should().Be(4);
            Do2(this.Input()).Should().Be(1356);
        }

        private long Do1(string input)
        {
            var sum = 0L;
            var ll = new LinkedList<int>(input.Select(it => it - '0'));
            for (var current = ll.First; current != null; current = current.Next)
            {
                if (current.Value == current.Roll().Value) sum += current.Value;
            }

            return sum;
        }

        private long Do2(string input)
        {
            var sum = 0L;
            var ll = new LinkedList<int>(input.Select(it => it - '0'));
            var halfWay = ll.First.Forward(input.Length / 2);
            for (var current = ll.First; current != null; current = current.Next, halfWay = halfWay.Roll())
            {
                if (current.Value == halfWay.Value) sum += current.Value;
            }

            return sum;
        }
    }
}