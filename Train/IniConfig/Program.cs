using System.Reflection;
using CommandLineParser;


var _rotation = 90; // Rotation.Horizontal;
var clargs = Environment.GetCommandLineArgs();
var pArgs = clargs[1..];
var parseResult = Parser.Parse<Options>(pArgs);
var speed_ratio = 1;
var screen_width = 32;
var screen_height = 12;
var paddle_width = 8;
var refresh_delay = 100;
var oppo_delay = 300;
var ball_delay = 100;
var ball_angle = 0;
if (parseResult.Tag == ParserResultType.Parsed){
	if(parseResult.Value.speed > 0)
		speed_ratio = parseResult.Value.speed;
	if(parseResult.Value.width > 0)
		screen_width = parseResult.Value.width;
	if(parseResult.Value.height > 0)
		screen_height = parseResult.Value.height;
	if(parseResult.Value.paddle > 0)
		paddle_width = parseResult.Value.paddle;
	_rotation = parseResult.Value.rotation; // != 0 ? parseResult.Value.rotation : 0;
	if(parseResult.Value.delay > 0)
		refresh_delay = parseResult.Value.delay;
	if(parseResult.Value.oppo_delay > 0)
		oppo_delay = parseResult.Value.oppo_delay;
	if(parseResult.Value.ball_delay > 0)
		ball_delay = parseResult.Value.ball_delay;
	if(parseResult.Value.ball_angle != 0)
		ball_angle = parseResult.Value.ball_angle;
}

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
