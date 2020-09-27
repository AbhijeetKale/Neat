using System;
using System.Collections.Generic;
namespace Neat.Util
{
    public static class RandomGenerator
    {
        public static object chooseAtRandom(object a, object b)
        {
            Random r = new Random();
            int num = r.Next() % 2;
            return num == 0 ? a : b;
        }

        public static T getRandomElementFromList<T>(List<T> list)
        {
            Random r = new Random();
            int idx = r.Next() % list.Count;
            return list[idx];
        }
    }
}
