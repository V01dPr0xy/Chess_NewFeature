using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    public static class Extensions
    {
        public static bool IsEven(this int i) => i % 2 == 0;
        public static bool IsOdd(this int i) => i % 2 == 1;
    }
}