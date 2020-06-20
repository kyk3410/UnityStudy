using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _079_this
{
    class AA
    {
        int a; // private
        public AA(int a) // int a = 10;
        {
            this.a = a;
        }

        public void Print()
        {
            int a = 100;   // Print에 있는 a
            this.a = 1000; // 외부의 a

            Console.WriteLine("a: {0}", a);
            Console.WriteLine("this.a: {0}", this.a);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AA aa = new AA(10);
            aa.Print();
        }
    }
}
