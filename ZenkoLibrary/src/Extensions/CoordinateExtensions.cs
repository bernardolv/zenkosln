using System.Collections;
using System.Text;
using System.Numerics;
using System.Collections.Generic;
using System;

namespace Zenko.Extensions
{
    public static class CoordinateExtensions
    {
        public static V2Int ToModelCoordinates(this V3 v3)
        {
            return new V2Int((int)(v3.x), -(int)(v3.z));
        }
        public static V3 ToViewCoordinates(this V2Int v2)
        {
            return new V3(v2.x, 0, -v2.y);
        }

        //Add +1 to every value to avoid negatives (there are some -1's with current model)
        public static string GetV2Hash(this V2Int number)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(((int)number.x + 1).GetDigitCount());
            builder.Append((int)number.x + 1);
            builder.Append(((int)number.y + 1).GetDigitCount());
            builder.Append((int)number.y + 1);
            return builder.ToString();
        }

        public static int GetDigitCount(this int number)
        {
            if (number < 10) return 1;
            if (number < 100) return 2;
            if (number < 1000) return 3;
            throw new System.Exception("Out of range digit count");
        }

        public static string GetIntHash(this int number)
        {
            StringBuilder builder = new StringBuilder("");
            builder.Append(number.GetDigitCount());
            builder.Append(number);
            return builder.ToString();
        }

        public static int[] GetInts(this BigInteger bigInteger)
        {
            //IS IT BETTER TO CAST TO STRING THEN ITERATE OVER STRING TO INT CONVERSION OR TO DIVIDE
            string bigIntString = bigInteger.ToString();

            List<int> result = new List<int>();

            int digits = 0;
            int digitsCount = 0;
            bool parsing = false;
            string currentNumber = "";
            foreach (char c in bigIntString)
            {
                // Logger.Log(c);
                if (parsing)
                {
                    // Logger.Log("p");
                    currentNumber += c;
                    digitsCount++;
                    if (digitsCount == digits)
                    {
                        // Logger.Log("add" + currentNumber);
                        result.Add(Convert.ToInt16(currentNumber));
                        digits = 0;
                        digitsCount = 0;
                        parsing = false;
                        currentNumber = "";
                    }
                }
                else
                {
                    digits = Convert.ToInt32(c - '0'); //Substract to convert to numeric value

                    // Logger.Log(digits + "dig");
                    parsing = true;

                }
            }
            return result.ToArray();
        }
    }
}