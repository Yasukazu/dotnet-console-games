namespace ping;
public class BallO {
    Point2f point = new Point2f(0, 0);
    Vector2f speed = new Vector2f(0, 0);
    public float x {get => point.x;}
    public float y {get => point.y;}
    public float dx {get => speed.x;}
    public float dy {get => speed.y;}
    public void step(object sender, System.EventArgs e) {
        Debug.Print("BallO.step();");
        point.add(dx, dy);
    }
    public (float, float) moveBy(float dx, float dy) {
        this.point.add(dx, dy);
        return (dx, dy);
    }
    public (float, float) moveTo(float dx, float dy) {
        this.point.set(dx, dy);
        return (dx, dy);
    }
    public void speedUp(float dx, float dy) {
        this.speed.add(dx, dy);
    }
    public void invertX() {
        var dx = this.speed.x;
        this.speed.add(-dx-dx, 0);
    }
    public void invertY() {
        var d = this.speed.y;
        this.speed.add(0, -d-d);
    }
    override public string ToString() {
        return $"point=({point.x}, {point.y}), speed=({speed.x}, {speed.y}).";
    }
}