using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
        var n = Convert.ToInt32(Console.ReadLine());
        for (var i = 0; i < n; i++) {
            var number = Convert.ToInt32(Console.ReadLine());
            var isPrime = IsPrime(number);
            Console.WriteLine(isPrime ? "Prime" : "Not prime");
        }
    }
    
    static bool IsPrime(int number) {
        bool isPrime = true;
        if (number == 1) {
            return false;
        }
        for (var i = 2; i * i <= number; i++) {
            if (number % i == 0) {
                isPrime = false;
                break;                
            }
        }
        return isPrime;
    }
}
