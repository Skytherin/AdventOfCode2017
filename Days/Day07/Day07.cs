using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day07
{
    [UsedImplicitly]
    public class Day07: IAdventOfCode
    {
        public void Run()
        {
            Do1(TypeCompiler.ParseLines<Day07Input>(this.Example())).Should().Be("tknk");
            Do1(TypeCompiler.ParseLines<Day07Input>(this.Input())).Should().Be("hmvwl");

            Do2(TypeCompiler.ParseLines<Day07Input>(this.Example())).Should().Be(60);
            Do2(TypeCompiler.ParseLines<Day07Input>(this.Input())).Should().Be(1853L);
        }

        public string Do1(IReadOnlyList<Day07Input> input)
        {
            var suspended = input.SelectMany(it => it.Children ?? new List<string>()).ToHashSet();

            return input.Single(it => !suspended.Contains(it.Name)).Name;
        }

        public long Do2(IReadOnlyList<Day07Input> input)
        {
            var all = input.SelectMany(it => it.Children ?? new List<string>()).AppendAll(input.Select(it => it.Name)).ToHashSet();

            var weights = all.ToDictionary(it => it, it => Weight(it, input));

            foreach (var item in input)
            {
                if (item.Children is null) continue;
                var w = item.Children.Select(it => weights[it]).ToList();
                var weightsByCount = w.GroupBy(it => it).ToDictionary(it => it.Key, it => it.Count());
                if (weightsByCount.Count == 1) continue;
                weightsByCount.Should().HaveCount(2);
                var common = weightsByCount.MaxBy(it => it.Value).Key;
                var oddball = weightsByCount.MinBy(it => it.Value).Key;
                var difference = common - oddball;
                var oddballIndex = w.FindIndex(it => it != common);
                return input.Single(it => it.Name == item.Children[oddballIndex]).Weight + difference;
            }

            throw new ApplicationException();
        }

        public long Weight(string name, IReadOnlyList<Day07Input> input)
        {
            var item = input.Single(it => it.Name == name);
            return item.Weight + (item.Children ?? new List<string>()).Sum(it => Weight(it, input));
        }
    }

    public record Day07Input(
        string Name,
        [Format(Before = "(", After = ")")]int Weight,
        [Format(Before = "->", Optional = true, Min = 1, Separator = ",")] IReadOnlyList<string>? Children);
}