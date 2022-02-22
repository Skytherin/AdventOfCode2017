using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;
using TypeParser.UtilityClasses;

namespace AdventOfCode2017.Days.Day16
{
    [UsedImplicitly]
    public class Day16: IAdventOfCode
    {
        public void Run()
        {
            Do2("s1,x3/4,pe/b", 5, 1).Should().Be("baedc");
            Do2(this.Input(), 16, 1).Should().Be("padheomkgjfnblic");

            Do2("s1,x3/4,pe/b", 5, 2).Should().Be("ceadb");

            Do2(this.Input(), 16, 1_000_000_000).Should().Be("bfcdeakhijmlgopn"); 
        }

        private string Do2(string danceMoves, int dancers, int count)
        {
            var original = Enumerable.Range('a', dancers).Select(it => (char)it).ToList();
            var moves = TypeCompiler.Parse<List<IAlternative<Spin, Exchange, Partner>>>(danceMoves, new Format { Separator = "," });

            var current = original.ToList();

            var closed = new List<string> { };
            foreach (var index in Enumerable.Range(0, count))
            {
                if (closed.Contains(current.Join()))
                {
                    var repeatLength = index;
                    var chosen = closed[1_000_000_000 % repeatLength];
                    return chosen;
                }
                closed.Add(current.Join());

                var temp = Dance(moves, current);

                current = temp;
            }

            return current.Join();
        }

        private List<char> Dance(List<IAlternative<Spin, Exchange, Partner>> moves, List<char> current)
        {
            foreach (var move in moves)
            {
                current = ProcessMove(current, move);
            }

            return current;
        }

        private List<char> ProcessMove(List<char> current, IAlternative<Spin, Exchange, Partner> move)
        {
            return move.Select(
                spin => SpinVerb(spin, current), 
                exchange => ExchangeVerb(exchange, current), 
                partner => PartnerVerb(partner, current));
        }

        private List<char> PartnerVerb(Partner partner, List<char> current)
        {
            var p1 = current.FirstIndex(it => it == partner.A);
            var p2 = current.FirstIndex(it => it == partner.B);
            (current[p1], current[p2]) = (current[p2], current[p1]);
            return current;
        }

        private List<char> ExchangeVerb(Exchange x, List<char> current)
        {
            (current[x.A], current[x.B]) = (current[x.B], current[x.A]);
            return current;
        }

        private List<char> SpinVerb(Spin spin, List<char> current)
        {
            var x1 = current.Skip(current.Count - spin.X).ToList();
            var x2 = current.Take(current.Count - spin.X).ToList();
            x1.AddRange(x2);
            return x1;
        }

        private record Spin([Format(Before = "s")] int X);
        private record Exchange([Format(Before = "x", After = "/")] int A, int B);
        private record Partner([Format(Before = "p", After = "/")] char A, char B);
    }
}