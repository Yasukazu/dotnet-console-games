namespace ping;

public class Array2f { 
    float[] array = new float[2];
    public Array2f(float x, float y) {
        array[0] = x;
        array[1] = y;
    } 
   public float x {get => array[0];
     protected set {
        array[0] = value;
     }
   } 
   public float y {get => array[1]; 
     protected set {
        array[1] = value;
     }
}

   public void set(float x, float y) {
        array[0] = x;
        array[1] = y;
   }

   public void add(float x, float y) {
    this.x += x;
    this.y += y;
   }
  }    

  public class Point2f : Array2f {
    public Point2f(float item1, float item2) : base(item1, item2) {}

    public float distanceSquared(Point2f p1) {
        float dx = this.x - p1.x;
        float dy = this.y - p1.y;
        return dx * dx + dy * dy;
    }
  }

  public class Vector2f : Array2f {
    public Vector2f(float item1, float item2) : base(item1, item2) {}
    
    public float dot(Vector2f v1) {
        return this.x * v1.x + this.y * v1.y;
    }

    public float length() {
        return (float)Math.Sqrt(this.x * this.x + this.y * this.y);
    }

    public float lengthSquared() {
        return this.x * this.x + this.y * this.y;
    }

    public void normalize(Vector2f v1) {
        float norm = 1.0f / (float)Math.Sqrt(v1.x * v1.x + v1.y * v1.y);
        set(v1.x * norm, v1.y * norm);
    }

    public void normalize() {
        float norm = (float)(1.0 / Math.Sqrt((double)(this.x * this.x + this.y * this.y)));
        set(x * norm, y * norm);
    }

    public float angle(Vector2f v1) {
        double vDot = (double)(this.dot(v1) / (this.length() * v1.length()));
        if (vDot < -1.0) {
            vDot = -1.0;
        }

        if (vDot > 1.0) {
            vDot = 1.0;
        }

        return (float)Math.Acos(vDot);
    }






  }