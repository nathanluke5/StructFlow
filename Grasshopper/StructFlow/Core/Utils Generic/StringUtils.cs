using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructFlow.Utils
{
    public class StringUtils
    {
        public static string AddPadding(string str, char padChar, int length)
        {
            if (str.Length > length)
                return str;
            else
            {
                string newStr = str.PadLeft(length, padChar);
                return newStr;
            }
        }

        /// <summary>
        /// Finds all numbers within a string. Strings will need to be seperated by at least one non digit charactors. 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<double> FindNumbersInString(string str)
        {
            List<double> numbers = new List<double>();
            string intStr = "";
            bool puncflicker = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsDigit(str[i]))
                {
                    intStr = String.Concat(intStr, str[i]);
                    if (i == str.Length - 1)
                    {
                        numbers.Add(Convert.ToDouble(intStr));
                    }
                }
                else if (str[i] == '.')   //Char.IsPunctuation(',') ||
                {
                    //currently hardcoded to only allow 
                    //there could be more than one comma in a number -- need to figure best way to deal with this. Probably comma input delination
                    if (puncflicker != true)
                    {
                        puncflicker = true;
                        if (Char.IsDigit(str[i + 1]))
                        {
                            intStr = String.Concat(intStr, str[i]);
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(intStr))
                                continue;
                            else
                            {
                                numbers.Add(Convert.ToDouble(intStr));
                                intStr = "";
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(intStr))
                            continue;
                        else
                        {
                            numbers.Add(Convert.ToDouble(intStr));
                            intStr = "";
                            puncflicker = false;
                            continue;
                        }
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(intStr))
                        continue;
                    else
                    {
                        numbers.Add(Convert.ToDouble(intStr));
                        intStr = "";
                    }
                }
            }
            return numbers;
        }
    }
}
