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
            if (list.Count == 0) {
                return default;
            }
            Random r = new Random();
            int idx = r.Next() % list.Count;
            return list[idx];
        }

        public static int getRandomIndexFromList<T>(List<T> list)
        {
            Random r = new Random();
            return r.Next() % list.Count;
        }

        public static double getRandomDouble()
        {
            Random r = new Random();
            double k = r.NextDouble();
            return r.Next() % 2 == 0 ? k : -k;
        }

        public static int randomNumBefore(int i)
        {
            Random r = new Random();
            return r.Next() % i;
        }

        public static T getElementBasedonProbablity<T>(List<T> elements, int[] probPercentage)
        {
            int sum = 0;
            foreach(int p in probPercentage)
            {
                sum += p;
            }
            if (sum != 100)
            {
                throw new ArgumentException("Addition of probs has to be 100");
            }
            Random r = new Random();
            int num = r.Next() % 100;
            sum = 0;
            for(int i = 0; i < elements.Count; i++)
            {
                sum += probPercentage[i];
                if (num < sum)
                {
                    return elements[i];
                }
            }
            return elements[elements.Count - 1];
        }
    }
}
