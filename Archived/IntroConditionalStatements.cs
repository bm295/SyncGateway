using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class Solution {

    static void Main(String[] args) {
        int N = Convert.ToInt32(Console.ReadLine());
        
        Console.WriteLine(IsWeird(N));
    }
    
    static string IsWeird(int n) {
        if (n % 2 == 1) {
            return "Weird";
        }
        
        if (n >= 2 && n <= 5) {
            return "Not Weird";
        }
        
        if (n >= 6 && n <= 20) {
            return "Weird";
        }
        
        if (n > 20) {
            return "Not Weird";
        }
        
        return "Weird";
    }
}
