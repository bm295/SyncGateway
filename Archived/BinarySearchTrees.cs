	static int getHeight(Node root){
      //Write your code here
        var heightLeft = 0;
        var heightRight = 0;
        if (root.left != null) {
            heightLeft = getHeight(root.left) + 1;
        }
        if (root.right != null) {
            heightRight = getHeight(root.right) + 1;
        }
        
        return Math.Max(heightLeft, heightRight);
    }
