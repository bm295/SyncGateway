	/**
	*    Name: PrintArray
	*    Print each element of the generic array on a new line. Do not return anything.
	*    @param A generic array
	**/
    // Write your code here
    public static void PrintArray<T>(T[] arr) {
        foreach(T item in arr) {
            Console.WriteLine(item);
        }
    }
