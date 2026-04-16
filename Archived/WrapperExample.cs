using System;
					
public class Program
{
	public static void Main()
	{
		var farmer = new Farmer();
		farmer.Name = "Ali";
		farmer.Introduce();
		
		var farmerBoss = new FarmerWrapper(farmer);
		farmerBoss.Introduce();
		farmerBoss.Name = "Eric";
		farmerBoss.Introduce();
	}
}

public class Farmer
{
	public string Name { get; set; }
	
	public void Introduce()
	{
		Console.WriteLine(string.Format("My name is farmer {0}", Name));
	}
}

public class FarmerWrapper
{
	private Farmer _farmer;
	
	public FarmerWrapper(Farmer farmer)
	{
		_farmer = farmer;
	}
	
	public string Name
	{
		get { return _farmer.Name; }
		set { _farmer.Name = value; }
	}
	
	public void Introduce()
	{
		_farmer.Introduce();
	}
}
