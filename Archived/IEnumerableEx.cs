using System;
using System.Linq;
					
public class Program
{
	public static void Main()
	{
		var numbers = from number in Enumerable.Range(0, 5) select number;
		foreach(var number in numbers.ToList())
		{
			Console.WriteLine(number);
		}
		foreach(var number in numbers.ToArray())
		{
			Console.WriteLine(number);
		}
	}
}
