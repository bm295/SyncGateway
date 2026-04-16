using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
        var n = Convert.ToInt32(Console.ReadLine());
        for(var i = 0; i < n; i++) {
            var count = 0;
            var input = Console.ReadLine();
            var even = "";
            var odd = "";
            foreach(char letter in input) {
                if (count % 2 == 0) {
                    even += letter;
                }
                else {
                    odd += letter;
                }
                count++;
            }
            Console.WriteLine(even + " " + odd);
        }
    }
}
