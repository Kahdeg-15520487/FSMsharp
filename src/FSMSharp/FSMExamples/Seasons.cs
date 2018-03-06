using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FSMSharp;

namespace FSMExamples
{
    struct WrapAroundInt
    {
        private int value;
        private int max;
        private int min;

        public int Value { get => value; set { this.value = value; Wrap(); } }
        public int Max { get => max; set { max = value; Wrap(); } }
        public int Min { get => min; set { min = value; Wrap(); } }

        public WrapAroundInt(int max, int min, int value = 0)
        {
            this.max = max;
            this.min = min;
            this.value = value;
            Wrap();
        }

        private void Wrap()
        {
            if (value > max)
            {
                value = min;
            }
            else if (value < min)
            {
                value = max;
            }
        }

        public static implicit operator WrapAroundInt(int i)
        {
            return new WrapAroundInt(int.MaxValue, int.MinValue, i);
        }

        public static bool operator ==(WrapAroundInt i1, WrapAroundInt i2)
        {
            return i1.Equals(i2);
        }

        public static bool operator ==(WrapAroundInt i1, int i2)
        {
            return i1.value == i2;
        }

        public static bool operator !=(WrapAroundInt i1, WrapAroundInt i2)
        {
            return !i1.Equals(i2);
        }

        public static bool operator !=(WrapAroundInt i1, int i2)
        {
            return i1.value != i2;
        }

        public static WrapAroundInt operator ++(WrapAroundInt i)
        {
            i.value++;
            i.Wrap();
            return i;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(WrapAroundInt))
            {
                return false;
            }
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + min.GetHashCode();
                hash = hash * 23 + max.GetHashCode();
                hash = hash * 23 + value.GetHashCode();
                return hash;
            }
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }

    class Seasons
    {
        // Define an enum to define the states. Anything could work, a string, int.. but enums are likely the easiest to manage
        enum Season
        {
            Winter,
            Spring,
            Summer,
            Fall
        }

        // Create the FSM
        FSM<Season> fsm = new FSM<Season>("seasons-fsm");
        WrapAroundInt timeCounter = new WrapAroundInt(12, 1, 1);

        public void Init()
        {
            // Initialize the states, adding them to the FSM and configuring their behaviour

            fsm.Add(Season.Winter)
                .Expires(() => timeCounter == 3)
                .GoesTo(Season.Spring)
                .OnEnter(() => Console.ForegroundColor = ConsoleColor.White)
                .OnLeave(() => Console.WriteLine("Winter is ending..."))
                .Calls(d => Console.WriteLine("Winter is going on.. "));

            fsm.Add(Season.Spring)
                .Expires(() => timeCounter == 6)
                .GoesTo(Season.Summer)
                .OnEnter(() => Console.ForegroundColor = ConsoleColor.Green)
                .OnLeave(() => Console.WriteLine("Spring is ending..."))
                .Calls(d => Console.WriteLine("Spring is going on.. "));

            fsm.Add(Season.Summer)
                .Expires(() => timeCounter == 9)
                .GoesTo(Season.Fall)
                .OnEnter(() => Console.ForegroundColor = ConsoleColor.Red)
                .OnLeave(() => Console.WriteLine("Summer is ending..."))
                .Calls(d => Console.WriteLine("Summer is going on.. "));

            fsm.Add(Season.Fall)
                .Expires(() => timeCounter == 12)
                .GoesTo(Season.Winter)
                .OnEnter(() => Console.ForegroundColor = ConsoleColor.DarkYellow)
                .OnLeave(() => Console.WriteLine("Fall is ending..."))
                .Calls(d => Console.WriteLine("Fall is going on.. "));

            // Very important! set the starting state
            fsm.CurrentState = Season.Winter;
        }

        public void Run()
        {
            // Define a base time. This seems pedantic in a pure .NET world, but allows to use custom time providers,
            // Unity3D Time class (scaled or unscaled), MonoGame timing, etc.
            DateTime baseTime = DateTime.Now;

            // Initialize the FSM
            Init();

            // Call the FSM periodically... in a real world scenario this will likely be in a timer callback, or frame handling (e.g.
            // Unity's Update() method).
            while (true)
            {
                // 
                fsm.Process();
                timeCounter++;
                Console.WriteLine(timeCounter);
                Thread.Sleep(100);
            }
        }
    }
}
