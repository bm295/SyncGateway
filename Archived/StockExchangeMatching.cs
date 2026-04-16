using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

class Result
{

    /*
     * Complete the 'computePrices' function below.
     *
     * The function is expected to return an INTEGER_ARRAY.
     * The function accepts following parameters:
     *  1. INTEGER_ARRAY s
     *  2. INTEGER_ARRAY p
     *  3. INTEGER_ARRAY q
     */

    public static List<int> computePrices(List<int> s, List<int> p, List<int> q)
    {
        Dictionary<int, int> sharesPrices = new Dictionary<int, int>();
        List<int> sortedShares = new List<int>();
        foreach(var share in s) {
            sortedShares.Add(share);
        }
        sortedShares.Sort();
        foreach(var share in sortedShares) {
            var index = s.IndexOf(share);
            sharesPrices.Add(share, p[index]);
        }
        Dictionary<int, int> qIndexes = new Dictionary<int, int>();
        List<int> sortedQ = new List<int>();
        foreach(var item in q) {
            sortedQ.Add(item);
        }
        sortedQ.Sort();
        foreach(var item in sortedQ) {
            qIndexes.Add(item, q.IndexOf(item));
        }
        List<int> prices = new List<int>();
        for(int i = 0; i < q.Count; i++) {
            prices.Add(0);
        }
        int selected = 0;
        foreach(var qIndex in qIndexes) {
            if (selected == s.Count - 1) {
                prices[qIndex.Value] = sharesPrices.Last().Value;
                continue;
            }            
            
            for(var i = selected; i < s.Count; i++) {
                var tempSharePrice = sharesPrices.ElementAt(i);
                if (qIndex.Key >= tempSharePrice.Key) {
                    prices[qIndex.Value] = tempSharePrice.Value;                    
                }
                else {
                    selected = i - 1;
                    break;
                }                
            }            
        }
        return prices;
    }
}

class Solution
{
    public static void Main(string[] args)
    {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        int n = Convert.ToInt32(Console.ReadLine().Trim());

        List<int> s = Console.ReadLine().TrimEnd().Split(' ').ToList().Select(sTemp => Convert.ToInt32(sTemp)).ToList();

        List<int> p = Console.ReadLine().TrimEnd().Split(' ').ToList().Select(pTemp => Convert.ToInt32(pTemp)).ToList();

        int k = Convert.ToInt32(Console.ReadLine().Trim());

        List<int> q = new List<int>();

        for (int i = 0; i < k; i++)
        {
            int qItem = Convert.ToInt32(Console.ReadLine().Trim());
            q.Add(qItem);
        }

        List<int> res = Result.computePrices(s, p, q);

        textWriter.WriteLine(String.Join("\n", res));

        textWriter.Flush();
        textWriter.Close();
    }
}
