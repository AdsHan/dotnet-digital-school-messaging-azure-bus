﻿using System.Linq;

namespace MED.Core.Utils
{
    public static class StringUtils
    {
        public static string JustNumbers(this string str, string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }
    }
}