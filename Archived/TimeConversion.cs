using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Solution {

    /*
     * Complete the timeConversion function below.
     */
    static string timeConversion(string s) {
        /*
         * Write your code here.
         */
        var hh = s.Substring(0, 2);
        var mm = s.Substring(3, 2);
        var ss = s.Substring(6, 2);
        var amPm = s.Substring(8, 2);
        if (amPm == "PM")
        {
            int convertHour = hh == "12" ? 0 : 12;
            int hour = Convert.ToInt32(hh) + convertHour;
            hh = hour.ToString();
        }
        else if (hh == "12")
        {
            hh = "00";
        }
        return string.Format("{0}:{1}:{2}", hh, mm, ss);
    }

    static void Main(string[] args) {
        TextWriter tw = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string s = Console.ReadLine();

        string result = timeConversion(s);

        tw.WriteLine(result);

        tw.Flush();
        tw.Close();
    }
}
