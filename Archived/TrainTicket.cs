using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Solution {


    /*
     * Complete the berthType function below.
     */
    static string berthType(int n) {
        // Return the type of berth as described in the output format section.
        int mod = n % 8;
        
        if (mod == 1 || mod == 4) {
            return "LB";
        }
        
        if (mod == 2 || mod == 5) {
            return "MB";
        }
        
        if (mod == 3 || mod == 6) {
            return "UB";
        }
        
        if (mod == 7) {
            return "SLB";
        }
        
        return "SUB";
    }


    static void Main(string[] args) {
        TextWriter tw = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        int n = Convert.ToInt32(Console.ReadLine());

        string result = berthType(n);

        tw.WriteLine(result);

        tw.Flush();
        tw.Close();
    }
}
