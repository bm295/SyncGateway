	// Add your code here
    public Difference(int[] elements) {
        this.elements = elements;
    }

    public void computeDifference() {
        for(var i = 0; i < this.elements.Length; i++) {
            for(var j = 0; j < this.elements.Length; j++) {
                if (j == i) {
                    continue;
                }
                var difference = Math.Abs(this.elements[i] - this.elements[j]);
                if ((i == 0 && j == 1) || difference > this.maximumDifference) {
                    this.maximumDifference = difference;
                }
            }
        }
    }
