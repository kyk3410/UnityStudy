using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class abstractAA
{
    protected int num;

    public abstractAA(int num)
    {
        this.num = num;
    }

    public abstract void abstractPrint(); //강제적으로 상속받은 클래스에서 구현부를 정의..

    public virtual void virtualPrint()
    {
        Console.WriteLine("abstractAA virtual void virtualPrint()");
    }

    public void Print()
    {
        Console.WriteLine("abstractAA Print()");
    }
}

class AA : abstractAA
{
    public AA(int num) : base(num)
    {
        Console.WriteLine("num: {0}", num);
    }

    public override void abstractPrint()
    {
        Console.WriteLine("AA override void abstractPrint()");
    }

    public override void virtualPrint()
    {
        base.virtualPrint();

        Console.WriteLine("AA override void virtualPrint()");
    }
}
namespace _090_abstract
{
    class Program
    {
        static void Main(string[] args)
        {
            AA aa = new AA(10);
            aa.abstractPrint();
            aa.virtualPrint();
            aa.Print();
        }
    }
}
