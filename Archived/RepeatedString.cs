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

    // Complete the repeatedString function below.
    static long repeatedString(string s, long n) {
        long numberOfAOneString = 0;
        foreach(char chr in s) {
            if (chr == 'a') {
                numberOfAOneString++;
            }
        }
        var numberOfRepeatedString = n / s.Length;
        var lengthOfSubstring = n % s.Length;
        var newSubstring = s.Substring(0, (int)lengthOfSubstring);
        long numberOfASubString = 0;
        foreach(char chr in newSubstring) {
            if (chr == 'a') {
                numberOfASubString++;
            }
        }
        return numberOfAOneString * numberOfRepeatedString + numberOfASubString;
    }

    static void Main(string[] args) {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string s = Console.ReadLine();

        long n = Convert.ToInt64(Console.ReadLine());

        long result = repeatedString(s, n);

        textWriter.WriteLine(result);

        textWriter.Flush();
        textWriter.Close();
    }
}
