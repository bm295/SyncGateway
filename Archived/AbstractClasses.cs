//Write MyBook class
internal class MyBook : Book {
    private int price;
    
    public MyBook(string title, string author, int price) : base(title, author) {
        this.price = price;
    }
    
    public override void display() {
        Console.WriteLine(string.Format("Title: {0}\nAuthor: {1}\nPrice: {2}", this.title, this.author, this.price));
    }
}
