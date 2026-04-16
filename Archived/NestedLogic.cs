using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
        string[] actual = Console.ReadLine().Split(' ');        
        string[] expected = Console.ReadLine().Split(' ');        
        var fine = CalculateFine(actual, expected);
        Console.WriteLine(fine);
    }
    
    static int CalculateFine(string[] actual, string[] expected) {
        var actualDay = Convert.ToInt32(actual[0]);
        var actualMonth = Convert.ToInt32(actual[1]);
        var actualYear = Convert.ToInt32(actual[2]);
        var expectedDay = Convert.ToInt32(expected[0]);
        var expectedMonth = Convert.ToInt32(expected[1]);
        var expectedYear = Convert.ToInt32(expected[2]);
        
        if (actualYear > expectedYear) {
            return 10000;
        }
        
        if (actualYear == expectedYear) {
            if (actualMonth > expectedMonth) {
                return 500 * (actualMonth - expectedMonth);
            }
            
            if (actualMonth == expectedMonth) {
                if (actualDay > expectedDay) {
                    return 15 * (actualDay - expectedDay);
                }
            }
        }   
   
        return 0;
    }
}
