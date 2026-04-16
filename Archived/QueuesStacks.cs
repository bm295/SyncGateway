class Solution {
  //Write your code here
  private string stack = string.Empty;
  private string queue = string.Empty;
  
  public void pushCharacter(char ch) {
    stack = ch + stack;
  }
  
  public void enqueueCharacter(char ch) {
    queue = queue + ch;
  }
  
  public char popCharacter() {
    return stack[0];
  }
  
  public char dequeueCharacter() {
    return queue[0];
  }
