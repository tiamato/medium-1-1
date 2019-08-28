using System;
using System.Collections.Generic;

namespace Task
{
    class Program
    {
        public static void Main(string[] args)
        {
            int obj1x = 5;
            int obj1y = 5;
            bool isalive1 = true;
            int obj2x = 10;
            int obj2y = 10;
            bool isalive2 = true;
            int obj3x = 15;
            int obj3y = 15;
            bool isalive3 = true;

            Random random = new Random();

            while (true)
            {
                if (obj1x == obj2x && obj1y == obj2y)
                {
                    isalive1 = false;
                    isalive2 = false;
                }

                if (obj1x == obj3x && obj1y == obj3y)
                {
                    isalive1 = false;
                    isalive3 = false;
                }

                if (obj2x == obj3x && obj2y == obj3y)
                {
                    isalive2 = false;
                    isalive3 = false;
                }

                obj1x += random.Next(-1, 1);
                obj1y += random.Next(-1, 1);

                obj2x += random.Next(-1, 1);
                obj2y += random.Next(-1, 1);

                obj3x += random.Next(-1, 1);
                obj3y += random.Next(-1, 1);

                if (obj1x < 0)
                    obj1x = 0;

                if (obj1y < 0)
                    obj1y = 0;

                if (obj2x < 0)
                    obj2x = 0;

                if (obj2y < 0)
                    obj2y = 0;

                if (obj3x < 0)
                    obj3x = 0;

                if (obj3y < 0)
                    obj3y = 0;

                if (isalive1)
                {
                    Console.SetCursorPosition(obj1x, obj1y);
                    Console.Write("1");
                }

                if (isalive2)
                {
                    Console.SetCursorPosition(obj2x, obj2y);
                    Console.Write("2");
                }

                if (isalive3)
                {
                    Console.SetCursorPosition(obj3x, obj3y);
                    Console.Write("3");
                }
            }
        }
    }
}