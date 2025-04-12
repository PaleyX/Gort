using PaleyExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gort
{
    internal class Functions
    {
        [Function("substr")]
        public static string Substr(string text, double start, double end)
        {
            if (start < 0 || end < 0 || start >= text.Length || end >= text.Length)
            {
                throw new ArgumentOutOfRangeException("Start or end index is out of range.");
            }

            if (start > end)
            {
                throw new ArgumentException("Start index cannot be greater than end index.");
            }

            return text.Substring((int)start, (int)end - (int)start + 1);
        }
    }
}
