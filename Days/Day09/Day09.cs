using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;
using TypeParser.UtilityClasses;

namespace AdventOfCode2017.Days.Day09
{
    [UsedImplicitly]
    public class Day09: IAdventOfCode
    {
        private readonly ITypeParser<Group> Matcher = TypeCompiler.Compile<Group>();

        public void Run()
        {
            Do1("{}").Should().Be(1);
            Do1("{{{}}}").Should().Be(6);
            Do1("{{},{}}").Should().Be(5);
            Do1("{{{},{},{{}}}}").Should().Be(16);
            Do1("{<>}").Should().Be(1);
            Do1("{<a>,<a>,<a>,<a>}").Should().Be(1);
            Do1("{{<ab>},{<ab>},{<ab>},{<ab>}}").Should().Be(9);
            Do1("{{<!!>},{<!!>},{<!!>},{<!!>}}").Should().Be(9);
            Do1("{{<a!>},{<a!>},{<a!>},{<ab>}}").Should().Be(3);
            Do1(this.Input()).Should().Be(14190L);

            Do2(this.Input()).Should().Be(7053L);
        }

        private long Do1(string input)
        {
            var group = Matcher.Match(input);
            return Score(group!.Value, 1);
        }

        private long Do2(string input)
        {
            var group = Matcher.Match(input);
            return ScoreGarbage(group!.Value);
        }

        private long ScoreGarbage(Group group)
        {
            return group.Subgroups.Select(it => it.Select(
                    ScoreGarbage,
                    garbage => Regex.Replace(garbage.Content[1..^1], "!.", "").Length))
                .Sum();
        }

        private long Score(Group group, long depth)
        {
            return depth +
                   group.Subgroups.Select(it => it.Select(subgroup => Score(subgroup, depth + 1), _ => 0L)).Sum();
        }

        [UsedImplicitly]
        private record Group(
            [Format(Before = "{", After = "}", Separator = ",")]
            IReadOnlyList<IAlternative<Group, Garbage>> Subgroups
        );

        [UsedImplicitly]
        private record Garbage(
            [Format(Regex = @"<((!.)|[^!>])*>")] string Content
        );
    }
}