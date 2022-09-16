// See https://aka.ms/new-console-template for more information
// Ping main
global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.Threading.Tasks;
global using System.IO;

using ping;
using pong;

int w, h;
(w, h) = pong.OnScreen.init();
Ball2 b2 = new ();
Debug.Print($"Ball2={b2.ToString()}");
b2.moveBy(1.1f, 2.3f);
b2.speedUp(0.1f, -2.4f);
Debug.Print($"Ball2={b2.ToString()}");

    public class Ball2 {
        Point2f point = new Point2f(0, 0);
        Vector2f speed = new Vector2f(0, 0);
        public void moveBy(float dx, float dy) {
            this.point.add(dx, dy);
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