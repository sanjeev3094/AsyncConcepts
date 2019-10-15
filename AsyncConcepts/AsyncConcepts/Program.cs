
namespace AsyncConcepts
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            InsideAsyncAwait insideAsyncAwait = new InsideAsyncAwait();
            insideAsyncAwait.FooAsync2();

            InsideAsyncAwait2 insideAsyncAwait2 = new InsideAsyncAwait2();
            Console.WriteLine(insideAsyncAwait2.FooAsync2().Result);

            InsideAsyncAwait3 insideAsyncAwait3 = new InsideAsyncAwait3();
            Console.WriteLine(insideAsyncAwait3.FooAsync2().Result);

            Console.ReadLine();
        }
    }
}
