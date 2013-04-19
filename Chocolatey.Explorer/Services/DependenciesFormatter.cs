using System;

namespace Chocolatey.Explorer.Services
{
    public class DependenciesFormatter
    {
        public string[] Execute(string dependencies)
        {
            var smallerthanIsTrue = false;
            var resultList = dependencies.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for(var i = 0; i < resultList.Length; i++)
            {
                resultList[i] = resultList[i].Replace(":", " ").Trim();
                if (resultList[i].EndsWith("]")) smallerthanIsTrue = true;
                resultList[i] = resultList[i].Replace("(", "(> ");
                resultList[i] = resultList[i].Replace("[", "(≥ ");
                resultList[i] = resultList[i].Replace(", ]", ")");
                resultList[i] = resultList[i].Replace(", ", " && ");
                if (resultList[i].Contains("&&")) resultList[i] = 
                    smallerthanIsTrue == true
                        ? resultList[i].Replace("&& ", "&& ≥ ")
                        : resultList[i].Replace("&& ", "&& < ");
            }
            return resultList;
        }
    }
}