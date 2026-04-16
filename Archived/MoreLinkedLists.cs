  public static Node removeDuplicates(Node head){
    //Write your code here
      Node tempNode = head;
      
      while (tempNode.next != null) {
          if (tempNode.data == tempNode.next.data) {
              tempNode.next = tempNode.next.next;
          }
          else {
              tempNode = tempNode.next;
          }
      }
      return head;
  }
