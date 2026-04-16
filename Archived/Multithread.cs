using System;
using System.Threading;
					
public class Program
{
	public static void Method1()
	{
		for (var i = 0; i < 10; i++)
		{
			Console.WriteLine(string.Format("Method 1: {0}", i));
			if (i == 5)
			{
				Thread.Sleep(1000);
			}
		}
	}
	
	public static void Method2()
	{
		for (var i = 0; i < 10; i++)
		{
			Console.WriteLine(string.Format("Method 2: {0}", i));
		}
	}
	
	public static void Main()
	{
		Thread thread1 = new Thread(Method1);
		Thread thread2 = new Thread(Method2);
		thread1.Start();
		thread2.Start();
	}
}
