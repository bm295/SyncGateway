	static void levelOrder(Node root){
  		//Write your code here
        Queue<Node> q = new Queue<Node>();
        Node node;
        
        if (root != null) {
            q.Enqueue(root);
        }
        
        while(q.Count > 0) {
            node = q.Dequeue();
            Console.Write(node.data + " ");
            
            if (node.left != null) {
                q.Enqueue(node.left);
            }
            
            if (node.right != null) {
                q.Enqueue(node.right);
            }
        } 
    }
