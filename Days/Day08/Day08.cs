using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;

namespace AdventOfCode2017.Days.Day08
{
    [UsedImplicitly]
    public class Day08: IAdventOfCode
    {
        internal record Instruction(
            string Register,
            [Format(Choices = "inc dec")] 
            string Operation,
            int Value,
            [Format(Before = "if")] 
            string ConditionalRegister,
            [Format(Choices = "<= < >= > == !=")] 
            string ConditionalOperator,
            int ConditionalValue
            );


        public void Run()
        {
            Do1(TypeCompiler.ParseLines<Instruction>(@"b inc 5 if a > 1
a inc 1 if b < 5
c dec -10 if a >= 1
c inc -20 if c == 10")).Last().Should().Be(1);
            Do1(TypeCompiler.ParseLines<Instruction>(this.Input())).Last().Should().Be(8022);
            Do1(TypeCompiler.ParseLines<Instruction>(this.Input())).Max().Should().Be(9819);
        }

        private IEnumerable<int> Do1(List<Instruction> instructions)
        {
            var registers = new Dictionary<string, int>();
            foreach (var instruction in instructions)
            {
                if (EvaluateCondition(instruction, registers))
                {
                    ApplyOperation(instruction, registers);
                    yield return registers.Values.Max();
                }
            }
        }

        private void ApplyOperation(Instruction instruction, Dictionary<string, int> registers)
        {
            var register = registers.GetValueOrDefault(instruction.Register);
            var value = register + instruction.Operation switch
            {
                "inc" => instruction.Value,
                "dec" => -instruction.Value
            };
            registers[instruction.Register] = value;
        }

        private bool EvaluateCondition(Instruction instruction, Dictionary<string, int> registers)
        {
            var register = registers.GetValueOrDefault(instruction.ConditionalRegister);
            var value = instruction.ConditionalValue;
            return instruction.ConditionalOperator switch
            {
                "==" => register == value,
                "<" => register < value,
                "<=" => register <= value,
                ">" => register > value,
                ">=" => register >= value,
                "!=" => register != value,
                _ => throw new ApplicationException()
            };
        }
    }
}