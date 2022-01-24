using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldingStoryWeb.Shared
{
    public class CodeHelper
    {
        public static char[] CharacterArray { get; } = new char[] { 'H', '7', 'W', 'u', 'I', 'f', 'J', 'C', 'i', 'p', '6', 'G', 'k', 'v', 'L', '2', '3', 'M', 'w', 'A', 'c', 'K', '9', 'D', 'j', 'B', 'Z', '5', 'h', 'd', 'S', 'x', 'a', 'm', 'q', 'n', 'U', 'R', 'O', 'o', 'r', 'V', 'g', 'P', 's', '8', 'e', 'E', 'N', '1', 't', 'Q', 'l', 'Y', 'T', 'F', 'b', '4', '0', 'X' };

        public static string IntToString(int value, char[]? baseChars = null)
        {
            baseChars = baseChars ?? CharacterArray;
            //string result = string.Empty;
            int targetBase = baseChars.Length;
            var sbuilder = new StringBuilder();

            do
            {
                sbuilder.Insert(0, baseChars[value % targetBase]);
                //result = baseChars[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            //return result;
            return sbuilder.ToString();
        }

        public static int StringToInt(string value, char[]? baseChars = null)
        {
            baseChars = baseChars ?? CharacterArray;
            int result = 0;

            for (int i = 0; i < value.Length; i++)
            {
                var index = Array.IndexOf(baseChars, value[value.Length - 1 -i]);
                result += index * (int)Math.Pow(baseChars.Length, i);
            }
            return result;
        }

        /// <summary>
        /// An optimized method using an array as buffer instead of 
        /// string concatenation. This is faster for return values having 
        /// a length > 1.
        /// </summary>
        public static string IntToStringFast(int value, char[]? baseChars = null)
        {
            baseChars = baseChars ?? CharacterArray;
            // 32 is the worst cast buffer size for base 2 and int.MaxValue
            int i = 32;
            char[] buffer = new char[i];
            int targetBase = baseChars.Length;

            do
            {
                buffer[--i] = baseChars[value % targetBase];
                value = value / targetBase;
            }
            while (value > 0);

            char[] result = new char[32 - i];
            Array.Copy(buffer, i, result, 0, 32 - i);

            return new string(result);
        }
    }
}
