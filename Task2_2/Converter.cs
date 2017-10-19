using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2_2
{
    public class Converter
    {
        public int ToInt32(string str)
        {
            str = str?.Trim();

            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("Digits are required", nameof(str));
            }

            var skipFirstChar = str[0] == '-' || str[0] == '+';
            if (skipFirstChar && str.Length == 1)
            {
                throw new ArgumentException("Digits are required", nameof(str));
            }

            var isNegative = str[0] == '-';
            var result = 0;

            try
            {
                for (var i = skipFirstChar ? 1 : 0; i < str.Length; i++)
                {
                    if (!char.IsDigit(str[i]))
                    {
                        throw new FormatException("Only digits are allowed");
                    }

                    checked
                    {
                        result = result * 10;
                        if (isNegative)
                        {
                            result -= (int)char.GetNumericValue(str[i]);
                        }
                        else
                        {
                            result += (int)char.GetNumericValue(str[i]);
                        }
                    }
                }
            }
            catch (FormatException e)
            {
                throw e;
            }
            catch (OverflowException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new Exception("Converting error", e);
            }

            return result;
        }
    }
}
