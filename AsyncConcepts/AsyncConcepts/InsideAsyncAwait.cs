

namespace AsyncConcepts
{
    using System.Runtime.CompilerServices;

    class InsideAsyncAwait
    {
        public async void FooAsync()
        {
        }

        /// <summary>
        /// This is the approximate compiler transformation of the above async void method.
        /// Under the hood, we have state machine and method builder.
        /// Begins running the builder with the associated state machine.
        /// </summary>
        public void FooAsync2()
        {
            var stateMachine = new FooAsyncStateMachine
            {
                MethodBuilder = new AsyncVoidMethodBuilder()
            };
            stateMachine.MethodBuilder.Start(ref stateMachine);
        }

        /// <summary>
        /// Represents state machines that are generated for asynchronous methods.
        /// </summary>
        struct FooAsyncStateMachine : IAsyncStateMachine
        {
            public AsyncVoidMethodBuilder MethodBuilder;

            /// <summary>
            /// Moves the state machine to its next state.
            /// </summary>
            public void MoveNext()
            {
                MethodBuilder.SetResult(); // Marks the method builder as successfully completed.
            }

            /// <summary>
            /// This state machine is lifted into the heap.
            /// i.e. Configures the state machine with a heap-allocated replica.
            /// </summary>
            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                MethodBuilder.SetStateMachine(stateMachine);
            }
        }
    }
}
