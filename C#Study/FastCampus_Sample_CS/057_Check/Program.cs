using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _057_Check
{
    class Program
    {
        static void Main(string[] args)
        {
            int inputNum;
            int max = int.MinValue, min = int.MaxValue;

            for(int i = 0; i < 5; i++)
            {
                Console.Write("학생의 성적을 입력하세요: ");
                inputNum = int.Parse(Console.ReadLine());

                if (max < inputNum)
                    max = inputNum;
                if (min > inputNum)
                    min = inputNum;
            }

            Console.WriteLine("최대값: {0} 최소값: {1}", max, min);
        }
    }
}
