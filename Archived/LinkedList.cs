	public static  Node insert(Node head,int data)
	{
      //Complete this method
        // if list is empty
        if (head == null) {
            return new Node(data);
        }
        
        // if end of list
        if (head.next == null) {
            head.next = new Node(data);
        }
        // move to next node
        else {
            insert(head.next, data);
        }
        
        return head;
    }
