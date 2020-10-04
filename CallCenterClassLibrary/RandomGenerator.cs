using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallCenterClassLibrary
{
    public static class RandomGenerator
    {
        private static Random _random = new Random();
        public static int GetRandom(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
    }
}
