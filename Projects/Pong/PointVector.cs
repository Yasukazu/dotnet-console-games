public class Tuple2f { 
    Tuple<float,float> tuple;
    public Tuple2f(float x, float y) {
        tuple = new(x, y);
    } 
   public float x {get => tuple.Item1;
   } 
   public float y {get => tuple.Item2;} 

   public void set(float x, float y) {
     tuple = new(x, y);
   }
  }    

  public class Point2f : Tuple2f {
    public Point2f(float item1, float item2) : base(item1, item2) {}

    public float distanceSquared(Point2f p1) {
        float dx = this.x - p1.x;
        float dy = this.y - p1.y;
        return dx * dx + dy * dy;
    }
  }

  public class Vector2f : Tuple2f {
    public Vector2f(float item1, float item2) : base(item1, item2) {}
    
    public float dot(Vector2f v1) {
        return this.x * v1.x + this.y * v1.y;
    }

    public float length() {
        return (float)Math.Sqrt((double)(this.x * this.x + this.y * this.y));
    }

    public float lengthSquared() {
        return this.x * this.x + this.y * this.y;
    }

    public void normalize(Vector2f v1) {
        float norm = (float)(1.0 / Math.Sqrt((double)(v1.x * v1.x + v1.y * v1.y)));
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