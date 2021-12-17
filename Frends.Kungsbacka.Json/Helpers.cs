using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Frends.Kungsbacka.Json
{
    internal static class Helpers
    {
        private static readonly Regex anglebracketsRegex;

        static Helpers()
        {
            // This is an attempt to support alternative template brackets by using regex
            // with balanced groups. You should never use regex to parse text, but it
            // is still useful if you don't try anything too advanced.
            anglebracketsRegex = new Regex(@"(?<expr>\[\[)[^\[\]]*(?<-expr>\]\])", RegexOptions.Compiled);
        }

        public static string ReplaceAngleBracketsWithCurlyBraces(string input)
        {
            var sb = new System.Text.StringBuilder(input);
            var matches = anglebracketsRegex.Matches(input);
            foreach (Match m in matches)
            {
                string value = m.Value.Substring(2, m.Value.Length - 4);
                int vlen = value.Length;
                int idx = m.Index;
                if (idx == 0 || sb[idx - 1] != '\\')
                {
                    sb[idx] = '{';
                    sb[idx + 1] = '{';
                    for (int i = 0; i < vlen; i++)
                    {
                        sb[i + idx + 2] = value[i];
                    }
                    sb[idx + vlen + 2] = '}';
                    sb[idx + vlen + 3] = '}';
                }
                else
                {
                    if (idx > 1 && sb[idx - 2] == '\\')
                    {
                        sb.Remove(idx - 2, 1);
                    }
                    else
                    {
                        sb.Remove(idx - 1, 1);
                    }
                }
            }
            return sb.ToString();
        }

        // Returns true if str ends with char c and removes c from end of str.
        // Escape char by doubling up. Examples (if c = '*'):
        // 1: "value*" returns true and str is changed to "value"
        // 2: "value**" returns false and str is changed to "value*"
        // 3: "value***" returns true and str is changed to "value*"
        // 4: "value****" returns false and str is changed to "value**"
        public static bool EndsWithChar(ref string str, char c)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            int len = str.Length;
            int pos = len - 1;
            while (pos >= 0 && str[pos] == c)
            {
                pos--;
            }
            bool b = ((len - pos) % 2) == 0;
            if (pos < len - 1)
            {
                str = str.Substring(0, len - (len - pos) / 2);
            }
            return b;
        }

        // Same as EndsWithChar above, but from the start of the string.
        public static bool StartsWithChar(ref string str, char c)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            int len = str.Length;
            int pos = 0;
            while (pos < len && str[pos] == c)
            {
                pos++;
            }
            bool b = (pos % 2) == 1;
            if (pos >= 0)
            {
                int cnt = pos == 1 ? 1 : pos / 2;
                str = str.Substring(cnt, len - cnt);
            }
            return b;
        }
    }
}
