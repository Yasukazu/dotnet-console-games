using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Configuration.Ini;

public class AppConfig {
	static AppConfig? Instance;

	public OptionsConfig? Options { get; set; }

	public AppConfig() { }
	public static AppConfig Get() {
		if (Instance != null)
			return Instance;
		Instance = new ConfigulationBuilder();
	}

}

    public class OptionsConfig
    {
public int rotation { get; set; }
public int speed_ratio { get; set; }
public int screen_width { get; set; }
public int screen_height { get; set; }
public int paddle_width { get; set; }
public int refresh_delay { get; set; }
public int oppo_delay { get; set; }
public int ball_delay { get; set; }
public int ball_angle { get; set; }
    }
/*
class Options {
	[Option('r', "rotation", Required =false, HelpText = "rotation default 0(not rotated) and others are 90(, 180 and 270).")]
	public int rotation { get; set;}
	[Option('s', "speed", Required =false, HelpText = "paddle speed times default 4")]
	public int speed { get; set;}
	[Option('w', "width", Required =false, HelpText = "screen width default 64")]
	public int width {get; set;}
	[Option('h', "height", Required =false, HelpText = "screen height default 24")]
	public int height {get; set;}
	[Option('p', "paddle", Required =false, HelpText = "paddle width default 8")]
	public int paddle {get; set;}
	[Option('d', "delay", Required =false, HelpText = "ball refresh rate. default 200")]
	public int delay {get; set;}
	[Option('o', "opponent delay", Required =false, HelpText = "opponent delay rate. default 200")]
	public int oppo_delay {get; set;}
	[Option('b', "ball delay", Required =false, HelpText = "ball delay rate. default 200")]
	public int ball_delay {get; set;}
	[Option('a', "initial angle", Required =false, HelpText = "initial ball angle. default 0(random)")]
	public int ball_angle {get; set;}
}
*/