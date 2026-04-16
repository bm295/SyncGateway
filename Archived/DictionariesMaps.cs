using System;
using System.Collections.Generic;
using System.IO;
class Solution {
  static void Main(String[] args) {
      /* Enter your code here. Read input from STDIN. Print output to STDOUT. Your class should be named Solution */
    var n = Convert.ToInt32(Console.ReadLine());
    var phoneBook = new Dictionary<string, string>();
    for(var i = 0; i < n; i++) {
      var input = Console.ReadLine().Split(' ');
      phoneBook.Add(input[0], input[1]);
    }
    for(var i = 0; i < n; i++) {
      var name = Console.ReadLine();
      var phone = "";
      if (phoneBook.TryGetValue(name, out phone)) {
        Console.WriteLine(string.Format("{0}={1}", name, phone));
      }
      else {
        Console.WriteLine("Not found");
      }
    }
  }
}
