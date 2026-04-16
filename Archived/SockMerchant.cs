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

class Solution {

    // Complete the sockMerchant function below.
    static int sockMerchant(int[] ar) {
        int countPairs = 0;
        Dictionary<int, int> socksPairs = new Dictionary<int, int>();
        foreach(int sock in ar) {
            if (socksPairs.ContainsKey(sock)) {
                socksPairs[sock]++;
            }
            else {
                socksPairs.Add(sock, 1);
            }
        }
        foreach(var socksCategory in socksPairs) {
            countPairs += (socksCategory.Value / 2);
        }
        return countPairs;
    }

    static void Main(string[] args) {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        int n = Convert.ToInt32(Console.ReadLine());

        int[] ar = Array.ConvertAll(Console.ReadLine().TrimEnd(' ').Split(' '), arTemp => Convert.ToInt32(arTemp))
        ;
        int result = sockMerchant(ar);

        textWriter.WriteLine(result);

        textWriter.Flush();
        textWriter.Close();
    }
}
