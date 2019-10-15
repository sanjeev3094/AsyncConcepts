
namespace AsyncConcepts
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    class InsideAsyncAwait3
    {
        public async Task<int> FooAsync()
        {
            await Task.Delay(10000);
            return 42;
        }

        /// <summary>
        /// This is the approximate compiler transformation of the above async void method.
        /// Under the hood, we have state machine and method builder.
        /// Begins running the builder with the associated state machine.
        /// </summary>
        public Task<int> FooAsync2()
        {
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

            private TaskAwaiter _awaiter;
            private int _state;

            /// <summary>
            /// Moves the state machine to its next state.
            /// </summary>
            public void MoveNext()
            {
                if (_state == 0)
                {
                    _awaiter = Task.Delay(10000).GetAwaiter();

                    if (_awaiter.IsCompleted)
                    {
                        _state = 1;
                        goto state1;
                    }
                    else
                    {
                        _state = 1;

                        // Schedules the state machine to proceed to the next action when the specified
                        // awaiter completes.
                        MethodBuilder.AwaitOnCompleted(ref _awaiter, ref this);
                    }

                    return;
                }
state1:
                if(_state == 1)
                {
                    _awaiter.GetResult();
                    MethodBuilder.SetResult(42); // Marks the method builder as successfully completed.

                    return;
                }
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
