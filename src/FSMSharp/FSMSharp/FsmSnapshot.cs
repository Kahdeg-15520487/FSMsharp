using System;

namespace FSMSharp
{
	public struct FsmSnapshot<T>
	{
		internal T CurrentState { get; private set; }

		internal FsmSnapshot(T currentState)
		{
			CurrentState = currentState;
		}
	}
}

