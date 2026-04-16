using System;
using System.Collections.Generic;
using System.IO;
class Solution {
    static void Main(String[] args) {
        /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
        double mealCost = Convert.ToDouble(Console.ReadLine());
        int tipPercent = Convert.ToInt32(Console.ReadLine());
        int taxPercent = Convert.ToInt32(Console.ReadLine());
        double totalCost = mealCost + mealCost*tipPercent/100 + mealCost*taxPercent/100;
        Console.WriteLine(string.Format("The total meal cost is {0} dollars.", Math.Round(totalCost)));
    }
}
