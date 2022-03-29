using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2017.Utils;
using FluentAssertions;
using JetBrains.Annotations;
using TypeParser;
using TypeParser.UtilityClasses;

namespace AdventOfCode2017.Days.Day18
{
    [UsedImplicitly]
    public class Day18: IAdventOfCode
    {
        public void Run()
        {
            Do1(@"set a 1
add a 2
mul a a
mod a 5
snd a
set a 0
rcv a
jgz a -1
set a 1
jgz a -2").Should().Be(4);
            Do1(this.Input()).Should().Be(8600); // 5980 too low

            Do2(@"snd 1
snd 2
snd p
rcv a
rcv b
rcv c
rcv d").Should().Be(3L);

            Do2(this.Input()).Should().Be(0);
        }

        private long Do1(string input)
        {
            var instructions = TypeCompiler.ParseLines<DuetInstruction>(input).Select(it => it.Op()).ToList();
            var state = new State();
            while (true)
            {
                var instruction = instructions[(int)state.PC];
                if (instruction is RecoverInstruction r)
                {
                    if (state.Get(r.Register) != 0)
                    {
                        return state.Sound;
                    }
                }
                instruction.Operate(state);
                if (instruction is not JumpInstruction) state.PC += 1;
            }
        }

        private long Do2(string input)
        {
            var instructions = TypeCompiler.ParseLines<DuetInstruction>(input).Select(it => it.Op()).ToReadOnlyList();
            var m1 = new Machine(0, instructions);
            var m2 = new Machine(1, instructions);
            var count = 0L;

            while (m1.RunState == Machine.RunStates.Running ||
                   m2.RunState == Machine.RunStates.Running)
            {
                count += m1.Step(m2.Receiver);
                m2.Step(m1.Receiver);
            }

            return count;
        }


        private class State
        {
            private readonly Dictionary<char, long> Registers = new();
            public long Sound;
            public long PC;

            internal long Get(char reg)
            {
                return Registers.GetValueOrDefault(reg);
            }

            internal long Get(IAlternative<long, char> other)
            {
                return other.Select(it => it, Get);
            }

            internal void Do2(char reg, IAlternative<long, char> other, Func<long, long, long> action)
            {
                Registers[reg] = action(Registers.GetValueOrDefault(reg), Get(other));
            }

            public void Set(char reg, long value)
            {
                Registers[reg] = value;
            }
        }

        private class Machine
        {
            private readonly IReadOnlyList<IOperation> Instructions;
            private readonly long Name;
            public enum RunStates {Running, Blocked, Terminated};

            public RunStates RunState { get; private set; } = RunStates.Running;
            public readonly Queue<long> Receiver = new ();
            private readonly State State = new();
            public Machine(long p, IReadOnlyList<IOperation> instructions)
            {
                Name = p;
                Instructions = instructions;
                State.Set('p', p);
            }

            // Return 0 or 1 depending on if it sent a value
            public int Step(Queue<long> output)
            {
                if (State.PC < 0 || State.PC >= Instructions.Count) RunState = RunStates.Terminated;
                if (RunState == RunStates.Terminated) return 0;
                var instruction = Instructions[(int)State.PC];
                if (instruction is RecoverInstruction r)
                {
                    if (Receiver.TryDequeue(out var received))
                    {
                        RunState = RunStates.Running;
                        State.Set(r.Register, received);
                        Console.WriteLine($"{Name} received {received}");
                    }
                    else
                    {
                        RunState = RunStates.Blocked;
                        return 0;
                    }
                }
                else if (instruction is SoundInstruction s)
                {
                    Console.WriteLine($"{Name} sent {State.Get(s.Register)}");
                    output.Enqueue(State.Get(s.Register));
                    State.PC += 1;
                    return 1;
                }
                else
                {
                    instruction.Operate(State);
                }
                if (instruction is not JumpInstruction) State.PC += 1;
                return 0;
            }
        }

        private record DuetInstruction(
            [Alternate] SoundInstruction? SoundInstruction,
            [Alternate] SetInstruction? SetInstruction,
            [Alternate] AddInstruction? AddInstruction,
            [Alternate] MultiplyInstruction? MultiplyInstruction,
            [Alternate] ModulusInstruction? ModulusInstruction,
            [Alternate] RecoverInstruction? RecoverInstruction,
            [Alternate] JumpInstruction? JumpInstruction
        )
        {
            internal IOperation Op() => SoundInstruction as IOperation ??
                                        SetInstruction as IOperation ??
                                        AddInstruction as IOperation ??
                                        MultiplyInstruction as IOperation ??
                                        ModulusInstruction as IOperation ??
                                        RecoverInstruction as IOperation ??
                                        JumpInstruction as IOperation ?? 
                                        throw new ApplicationException();
        }

        private interface IOperation
        {
            void Operate(State state);
        }

        private record SoundInstruction([Format(Before = "snd")] char Register) : IOperation
        {
            public void Operate(State state)
            {
                state.Sound = state.Get(Register);
            }
        }
        private record SetInstruction([Format(Before = "set")] char DestinationRegister, IAlternative<long, char> Source) : IOperation
        {
            public void Operate(State state)
            {
                state.Do2(DestinationRegister, Source, (_, b) => b);
            }
        }
        private record AddInstruction([Format(Before = "add")] char DestinationRegister, IAlternative<long, char> Source) : IOperation
        {
            public void Operate(State state)
            {
                state.Do2(DestinationRegister, Source, (a, b) => a + b);
            }
        }
        private record MultiplyInstruction([Format(Before = "mul")] char DestinationRegister, IAlternative<long, char> Source) : IOperation
        {
            public void Operate(State state)
            {
                state.Do2(DestinationRegister, Source, (a, b) => a * b);
            }
        }
        private record ModulusInstruction([Format(Before = "mod")] char DestinationRegister, IAlternative<long, char> Source) : IOperation
        {
            public void Operate(State state)
            {
                state.Do2(DestinationRegister, Source, MoreMath.BetterMod);
            }
        }
        private record RecoverInstruction([Format(Before = "rcv")] char Register) : IOperation
        {
            public void Operate(State state)
            {
            }
        }
        private record JumpInstruction([Format(Before = "jgz")] char SourceRegister, IAlternative<long, char> Offset) : IOperation
        {
            public void Operate(State state)
            {
                if (state.Get(SourceRegister) > 0)
                {
                    state.PC += state.Get(Offset);
                }
                else
                {
                    state.PC += 1;
                }
            }
        }
    }
}