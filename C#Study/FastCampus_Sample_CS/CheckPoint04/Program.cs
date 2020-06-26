using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPoint04
{
    class Army
    {
        protected int _HP;
        protected int _MP;
        protected int _Speed;
        protected int _Attack;

        public virtual void Run()
        {
            Console.Write("{0}의 속도로 ", _Speed);
        }

        public virtual void Attack()
        {
            Console.WriteLine();

            if (this is Healer)
            {
                Console.Write("[마법 공격력 - {0}]으로 ", _Attack * _MP);
            }
            else
            {
                Console.Write("[공격력 - {0}]으로 ", _Attack);
            }
        }
    }

    class Barbarian : Army
    {
        public Barbarian()
        {
            _HP = 100;
            _MP = 0;
            _Speed = 100;
            _Attack = 100;
        }

        public override void Run()
        {
            base.Run();
            Console.WriteLine("Barbarian 달려갑니다.");
        }

        public override void Attack()
        {
            base.Attack();
            Console.WriteLine("Barbarian이 칼로 공격!!");
        }
    }

    class Giant : Army
    {
        public Giant()
        {
            _HP = 200;
            _MP = 0;
            _Speed = 10;
            _Attack = 200;
        }
        public override void Run()
        {
            base.Run();
            Console.WriteLine("Giant 달려갑니다.");
        }

        public override void Attack()
        {
            base.Attack();
            Console.WriteLine("Giant 공격!!");
        }
    }

    class Healer : Army
    {
        public Healer()
        {
            _HP = 50;
            _MP = 100;
            _Speed = 200;
            _Attack = 10;
        }
        public override void Run()
        {
            base.Run();
            Console.WriteLine("Healer 날아갑니다.", _Speed);
        }

        public override void Attack()
        {
            base.Attack();
            Console.WriteLine("Healer 마법 공격!!");
        }
    }

    class UnitControl
    {
        public enum UNIT
        {
            NONE,
            BARBARIAN,
            GIANT,
            HEALER
        }
        const int MAX = 10;
        Army[] arrArmys;
        int indexCount;

        public UnitControl()
        {
            indexCount = 0;
            arrArmys = new Army[MAX];
        }

        public string Menu()
        {
            Console.WriteLine("++ 유닛 관리 시스템 ++ ");
            Console.WriteLine(" (1)생성 (2)달리기 (3)공격 (0: 나가기)");

            return Console.ReadLine();
        }

        public void CreateUnit()
        {
            Console.WriteLine("++ 유닛 생성 ++ ");
            Console.WriteLine(" (1)바바리안 (2)자이언트 (3)힐러 (0: 뒤로가기)");

            UNIT selUnit = UNIT.NONE;
            string inputStr = Console.ReadLine();

            if (inputStr.Equals("1") || inputStr.Equals("2") || inputStr.Equals("3"))
                selUnit = (UNIT)int.Parse(inputStr);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            UnitControl control = new UnitControl();
            string selNumStr;

            while (true)
            {
                selNumStr =  control.Menu();

                switch (selNumStr)
                {
                    case "1": //생성
                        break;
                    case "2": //달리기
                        break;
                    case "3": //공격
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
