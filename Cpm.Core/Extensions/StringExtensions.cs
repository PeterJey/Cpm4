using System;
using System.Collections.Generic;
using System.Linq;

namespace Cpm.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GetLastToken(this string text, string[] separators)
        {
            return text
                .Split(separators, StringSplitOptions.None)
                .LastOrDefault();
        }

        public static string GetLastToken(this string text, string separator)
            => GetLastToken(text, new[] {separator});

        public static string GetFirstToken(this string text, string[] separators)
        {
            return text
                .Split(separators, StringSplitOptions.None)
                .FirstOrDefault();
        }

        public static string GetFirstToken(this string text, string separator)
            => GetFirstToken(text, new[] {separator});

        public static string JoinToString(this IEnumerable<char> source)
        {
            return string.Join(string.Empty, source);
        }
    }
}
