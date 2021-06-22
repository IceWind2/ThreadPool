using System;
using System.Collections.Generic;
using ThreadPool.Task;

namespace ThreadPool
{
    class Program
    {
        class cl1
        {
            int i = 1;
        }

        class cl2<T> : cl1
        {
            T d;
        }

        static void Main(string[] args)
        {
            var tt = new cl2<string>();

            Console.WriteLine(tt.GetType().GetGenericArguments()[0]);

            Console.ReadKey();
        }
    }
}
