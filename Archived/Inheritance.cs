class Student : Person{
    private int[] scores;  
  
    /*	
    *   Class Constructor
    *   
    *   Parameters: 
    *   firstName - A string denoting the Person's first name.
    *   lastName - A string denoting the Person's last name.
    *   id - An integer denoting the Person's ID number.
    *   scores - An array of integers denoting the Person's test scores.
    */
    // Write your constructor here
    public Student(string firstName, string lastName, int id, int[] scores) {
        this.firstName = firstName;
        this.lastName = lastName;
        this.id = id;
        this.scores = scores;
    }
    
    /*	
    *   Method Name: Calculate
    *   Return: A character denoting the grade.
    */
    // Write your method here
    public char Calculate() {
        int sum = 0;
        for(var i = 0; i < this.scores.Length; i++) {
            sum += this.scores[i];
        }
        int avg = sum / this.scores.Length;        
        if (avg <= 100 && avg >= 90) {
            return 'O';
        }
        if (avg < 90 && avg >= 80) {
            return 'E';
        }
        if (avg < 80 && avg >= 70) {
            return 'A';
        }
        if (avg < 70 && avg >= 55) {
            return 'P';
        }
        if (avg < 55 && avg >= 40) {
            return 'D';
        }
        return 'T';
    }
}
