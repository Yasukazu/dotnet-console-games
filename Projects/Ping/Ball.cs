namespace ping;
public class BallO {
    PointF point;
    System.Numerics.Vector2 speed = new (0, 0);
    public float x {get => point.X;}
    public float y {get => point.Y;}
    public float dx {get => speed.X;}
    public float dy {get => speed.Y;}
    public PointF getPoint() => point;

    public BallO() {
      point = new (0, 0);
    }
    
    public void step(object sender, System.EventArgs e) {
        Debug.Print("BallO.step();");
        point.X += dx; // .add(dx, dy);
        point.Y += dy;
    }
    public (float, float) moveBy(float dx, float dy) {
        point.X += dx; // .add(dx, dy);
        point.Y += dy;
        return (dx, dy);
    }
    public (float, float) moveTo(float dx, float dy) {
        point.X = dx; // .add(dx, dy);
        point.Y = dy;
        return (dx, dy);
    }
    public void speedUp(float dx, float dy) {
        speed.X += dx; //.add(dx, dy);
        speed.Y += dy;
    }
    public void invertX() {
        speed.X = -speed.X;
    }
    public void invertY() {
        speed.Y = -speed.Y;
    }
    override public string ToString() {
        return $"point=({point.X}, {point.Y}), speed=({speed.X}, {speed.Y}).";
    }
}