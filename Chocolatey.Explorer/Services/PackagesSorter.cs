using System;
using System.Collections.Generic;

namespace Chocolatey.Explorer.Services
{
    public class PackagesSorter : IComparer<string>
    {
        private static readonly char LevelSeperator = ".".ToCharArray()[0];
        private const string PreRelease = "-pre";

        public int Compare(string x, string y)
        {
            var xsplits = x.Split(LevelSeperator).GetUpperBound(0);
            var ysplits = y.Split(LevelSeperator).GetUpperBound(0);
            if (x.Contains(PreRelease)) x = x.Substring(0, x.Length - PreRelease.Length);
            if (y.Contains(PreRelease)) y = y.Substring(0, y.Length - PreRelease.Length);
            var maxsplits = ysplits > xsplits ? xsplits : ysplits;
            if (!String.IsNullOrEmpty(x))
            {
                for (var i = 1; i <= maxsplits; i++)
                {
                    if (Int32.Parse(x.Split(LevelSeperator)[i]) >
                        Int32.Parse(y.Split(LevelSeperator)[i])) return 1;
                    if (Int32.Parse(x.Split(LevelSeperator)[i]) <
                        Int32.Parse(y.Split(LevelSeperator)[i])) return -1;
                }
                if (ysplits > xsplits) return -1;
                return ysplits < xsplits ? 1 : 0;
            }
            else
            {
                if (!String.IsNullOrEmpty(y))
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}