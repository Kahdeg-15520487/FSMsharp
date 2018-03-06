using System;
using FSMSharp;
using NUnit.Framework;

namespace FSMSharp_Tests
{
    [TestFixture]
    public class FsmTests
    {
        public class RefContainer<T>
        {
            public T Value;
        }


        [Test]
        public void SimpleExpiration()
        {
            FSM<int> F = new FSM<int>("");
            int turnCounter = 0;
            F.Add(1)
                .Expires(() => turnCounter == 5)
                .GoesTo(2);
            F.Add(2);

            F.CurrentState = 1;

            F.Process();
            Assert.AreEqual(1, F.CurrentState);
            turnCounter = 1;
            F.Process();
            Assert.AreEqual(1, F.CurrentState);
            turnCounter = 2;
            F.Process();
            Assert.AreEqual(1, F.CurrentState);
            turnCounter = 3;
            F.Process();
            Assert.AreEqual(1, F.CurrentState);
            turnCounter = 4;
            F.Process();
            Assert.AreEqual(1, F.CurrentState);
            turnCounter = 5;
            F.Process();
            Assert.AreEqual(2, F.CurrentState);
        }

        [Test]
        public void OnEnterAndOnLeaveHandlers()
        {
            FSM<int> F = new FSM<int>("");
            RefContainer<int> x = new RefContainer<int>();
            x.Value = 0;
            int turnCounter = 0;
            F.Add(1)
                .Expires(() => turnCounter == 1)
                .GoesTo(2)
                .OnLeave(() => x.Value = 10);
            F.Add(2)
                .OnEnter(() => x.Value = 20);

            F.CurrentState = 1;
            F.Process();
            Assert.AreEqual(0, x.Value);
            turnCounter = 1;
            F.Process();
            Assert.AreEqual(20, x.Value);
        }

        [Test]
        public void StateIsLeavedImmediatelyOnSuddenExpiration()
        {
            FSM<int> F = new FSM<int>("");
            RefContainer<int> x = new RefContainer<int>();
            int turnCounter = 0;
            F.Add(1)
                .Expires(() => turnCounter == 1)
                .GoesTo(2);
            F.Add(2)
                .GoesTo(3)
                .Expires(() => turnCounter == 2)
                .OnLeave(() => x.Value += 15);
            F.Add(3)
                .OnEnter(() => x.Value *= 2);

            F.CurrentState = 1;
            F.Process();
            turnCounter = 1;
            F.Process();
            turnCounter = 2;
            F.Process();

            Assert.AreEqual(3, F.CurrentState);
            Assert.AreEqual(30, x.Value);
        }
    }
}
