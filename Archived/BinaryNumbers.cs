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
    static void Main(string[] args) {
        int n = Convert.ToInt32(Console.ReadLine());
        var binNum = Convert.ToString(n, 2);
        var max = 0;
        var count = 0;
        foreach(var digit in binNum) {
            if (digit == '1') {
                count++;
            }
            else {
                if (count > max) {
                    max = count;
                }
                count = 0;
            }
        }
        if (count > max) {
            max = count;
        } 
        Console.WriteLine(max);
    }
}
