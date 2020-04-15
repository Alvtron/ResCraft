using System;
using System.Collections.Generic;

namespace ResourceCraft.Extensions
{
    public static class StringExtension
    {
        public static IEnumerable<int> AllIndexesOf(this string str, string substr, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("String is not specified.");
            if (string.IsNullOrWhiteSpace(substr))
                throw new ArgumentException("Substring is not specified.");

            var index = 0;

            while ((index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
            {
                yield return index++;
            }
        }

        public static bool IsRtf(this string str)
        {
            return str.Contains(@"{\rtf");
        }
    }
}
