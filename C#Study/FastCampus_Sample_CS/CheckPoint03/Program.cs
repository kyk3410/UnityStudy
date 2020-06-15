using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CheckPoint03 - 배열을 활용하기
namespace CheckPoint03
{
    class Program
    {
        const int MAP_X = 7;
        const int MAP_Y = 22;

        static void UpdateView(char[] _tile, int[,] _map)// 배열을 파라미터로 넘긴다
        {
            for (int i = 0; i < MAP_X; i++)
            {
                for (int j = 0; j < MAP_Y; j++)
                {
                    int tileIndex = _map[i, j];
                    Console.Write(_tile[tileIndex]);

                    if (j == MAP_Y - 1)
                        Console.WriteLine();
                }
            }
        }
        static void Main(string[] args)
        {
            
            //              0    1    2    3    4    5    6    7
            char[] tile = {' ', '-', '|', '1', '2', '3', '4', '5'};

            int[,] map = new int[MAP_X, MAP_Y]
            {
                //0  1  2  3  4  5  6  7  8  9  10  11  12  13  14  15  16  17  18  19  20  21
                 {1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1}, //0
                 {3, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, //1
                 {4, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, //2
                 {5, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, //3
                 {6, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, //4
                 {7, 0, 0, 0, 0, 0, 0, 0, 0, 0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0}, //5
                 {1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1}, //6
            };

            UpdateView(tile, map);
        }
    }
}
