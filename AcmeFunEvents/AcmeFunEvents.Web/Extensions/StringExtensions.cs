using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AcmeFunEvents.Web.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public static string ReadTextFromFile(this string pathToFile)
        {
            try
            {
                return File.ReadAllText(new DirectoryInfo(pathToFile).ToString());
            }
            catch (Exception dirEx)
            {
                return "Directory not found: " + dirEx.Message;
            }
        }
        
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        public static string GetDescription(this Enum genericEnum) //Hint: Change the method signature and input paramter to use the type parameter T
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if (memberInfo.Length > 0)
            {
                var attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attribs.Any())
                {
                    return ((System.ComponentModel.DescriptionAttribute)attribs.ElementAt(0)).Description;
                }
            }
            return genericEnum.ToString();
        }
    }
}