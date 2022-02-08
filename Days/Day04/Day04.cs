using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using TypeParser;

namespace AdventOfCode2017.Days.Day04
{
    public class Day04: IAdventOfCode
    {
        public void Run()
        {
            var input = TypeCompiler.ParseLines<List<string>>(this.Input());
            Do1(input).Should().Be(455);
            Do2(input).Should().Be(186);
        }

        private long Do1(List<List<string>> input)
        {
            return input.Count(Valid);
        }

        private long Do2(List<List<string>> input)
        {
            return input.Count(Valid2);
        }

        private bool Valid(List<string> line)
        {
            return AllUnique(line);
        }

        private bool Valid2(List<string> line)
        {
            return AllUnique(line.Select(word => word.OrderBy(it => it).Join()));
        }

        private bool AllUnique(IEnumerable<string> s)
        {
            var hs = new HashSet<string>();
            return s.All(it => hs.Add(it));
        }
    }
}