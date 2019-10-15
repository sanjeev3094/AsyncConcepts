

namespace AsyncConcepts
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    class InsideAsyncAwait2
    {
        public async Task<int> FooAsync()
        {
            return 42;
        }

        /// <summary>
        /// This is the approximate compiler transformation of the above async void method.
        /// Under the hood, we have state machine and method builder.
        /// Begins running the builder with the associated state machine.
        /// </summary>
        public Task<int> FooAsync2()
        {
            // async methods are converted to a state machine and then the state machine is started by the 
            // method builder i.e. it drives the state machine.
            var stateMachine = new FooAsyncStateMachine
            {
                MethodBuilder = AsyncTaskMethodBuilder<int>.Create()
            };

            // calls MoveNext() of the state machine to start the synchronous portion 
            // of the code before the 1st await in the method.
            stateMachine.MethodBuilder.Start(ref stateMachine); 

            return stateMachine.MethodBuilder.Task;
        }

        /// <summary>
        /// Represents state machines that are generated for asynchronous methods.
        /// </summary>
        struct FooAsyncStateMachine : IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<int> MethodBuilder;

            /// <summary>
            /// Moves the state machine to its next state.
            /// </summary>
            public void MoveNext()
            {
                MethodBuilder.SetResult(42);
            }

            /// <summary>
            /// This state machine is lifted into the heap.
            /// i.e. Configures the state machine with a heap-allocated replica.
            /// Method builder is the thing which is hoisted to the heap and it contains the state machine.
            /// </summary>
            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                MethodBuilder.SetStateMachine(stateMachine);
            }
        }
    }
}
