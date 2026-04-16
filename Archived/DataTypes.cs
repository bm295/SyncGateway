
// Declare second integer, double, and String variables.
int secondInt = 0;
double secondDouble = 0;
string secondStr = string.Empty;
// Read and save an integer, double, and String to your variables.
secondInt = Convert.ToInt32(Console.ReadLine());
secondDouble = Convert.ToDouble(Console.ReadLine());
secondStr = Console.ReadLine();
// Print the sum of both integer variables on a new line.
Console.WriteLine(i + secondInt);
// Print the sum of the double variables on a new line.
Console.WriteLine((d + secondDouble).ToString("N1"));
// Concatenate and print the String variables on a new line
// The 's' variable above should be printed first.
Console.WriteLine(s + secondStr);
