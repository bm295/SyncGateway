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
     * Complete the 'numbersSquare' function below.
     *
     * The function accepts following parameters:
     *  1. INTEGER n
     *  2. INTEGER s
     */

    public static void numbersSquare(int n, int s)
    {
        var result = s;
        for (var i = 0; i < n; i++) {
            for (var j = 0; j < n; j++) {
                if (i >= j) {
                    result = area(max(i, j) + 1) + s - j - 1;
                }
                else {
                    result = area(max(i, j)) + s + i;
                }
                Console.Write(j == n - 1 ? string.Format("{0}\n", result) : string.Format("{0} ", result));
            }
        }
    }
    
    private static int max(int a, int b) {
        return Math.Max(a, b);
    }
    
    private static int area(int a) {
        return a * a;
    }
}

class Solution
{
    public static void Main(string[] args)
    {
        string[] firstMultipleInput = Console.ReadLine().TrimEnd().Split(' ');

        int n = Convert.ToInt32(firstMultipleInput[0]);

        int s = Convert.ToInt32(firstMultipleInput[1]);

        Result.numbersSquare(n, s);
    }
}
