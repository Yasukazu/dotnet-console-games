
﻿global using System;
global using System.Linq;
global using System.Collections;
global using System.Collections.Generic;
global using System.Diagnostics;
global using CommandLineParser; // Original source code: https://github.com/wertrain/command-line-parser-cs (Version 0.1)
global using System.Threading.Tasks;
global using System.IO;
using Sharprompt;
using Sharprompt.Fluent;

#nullable enable

// ConsoleTraceListener myWriter = new GonsoleTraceListener();
// Trace.Listeners.Add(myWriter);
// Debug.Write("myWriter is added to Trace.Listeners.  OOProgram start.");
var _rotation = 90; // Rotation.Horizontal;
var clargs = Environment.GetCommandLineArgs();
var pArgs = clargs[1..];
ParserResult<Options> parseResult = Parser.Parse<Options>(pArgs);
Options opt = parseResult.Value;
// Load from XML file into options
Options? new_opt = null;
try {
new_opt = (opt.load_from_xml != "") switch {
	true => opt.LoadXML(opt.load_from_xml),
// Debug.WriteLine($"Starting to load XML file:{opt.load_from_xml}");
	false =>  // File.Exists(Options.XmlName) ?
	opt.LoadXML(Options.XmlName)
};
} catch(IOException ex) {
	Console.WriteLine("Error opening " + ex.Message);
}
	// Debug.WriteLine($"Starting to load XML file:" + Options.XmlName);

if(new_opt != null){
	Debug.WriteLine($"Updating opt with new_opt.");
	if(new_opt.height > 0)
		opt.height = new_opt.height;
	if(new_opt.load_from_xml != "")
		opt.load_from_xml = new_opt.load_from_xml;
	if(new_opt.oppo_delay > 0)
		opt.oppo_delay = new_opt.oppo_delay;
	if(new_opt.paddle > 0)
		opt.paddle = new_opt.paddle;
	if(new_opt.rotation != 0)
		opt.rotation = new_opt.rotation;
	if(new_opt.save_to_xml != "")
		opt.save_to_xml = new_opt.save_to_xml;
	if(new_opt.speed > 0)
		opt.speed = new_opt.speed;
	if(new_opt.width > 0)
		opt.width = new_opt.width;
	if(new_opt.ball_angle != 0)
		opt.ball_angle = new_opt.ball_angle;
	if(new_opt.ball_delay > 0)
		opt.ball_delay = new_opt.ball_delay;
	if(new_opt.delay > 0)
		opt.delay = new_opt.delay;
}
// modify opt by last games
var points_xml_file = @"points.xml" ;
if(File.Exists(points_xml_file)){
	Debug.WriteLine("Opening: " + points_xml_file);
	Points[] pp = Points.LLoadXML(points_xml_file);
	var selfPoints = pp.Select(x => x.Self).Sum();
	var oppoPoints = pp.Select(x => x.Opponent).Sum();
	if (selfPoints > oppoPoints){
		var minus = 10;
		Debug.WriteLine("oppo delay -{minus}");
		opt.oppo_delay -= minus;
	}
	else if (selfPoints < oppoPoints){
		var plus = 50;
		Debug.WriteLine("ball delay +{plus}");
		opt.ball_delay += plus;
	}
}

Rotation rot = _rotation switch {
	0 => Rotation.Horizontal, 90 => Rotation.Vertical,
	_ => throw new ArgumentException("Rotation must be one of {0, 90}.")
};
(opt.width, opt.height) = OnScreen.init(opt.width, opt.height);

const int game_repeat = 3;
Points[] ppoints = new Points[game_repeat];
Game game;
Console.Write("Hit any key to start:");
bool modified = false;
for ( int i = 0; i < game_repeat; ++i){
	game = new Game(opt); // speed_ratio, screen_w, screen_h, paddle_width, rot, delay, oppo_delay, ball_delay, ball_angle);
	var points = game.Run();
    game.screen.SetCursorPosition(0, 0);

	string msg =
	points switch {
		{Self: var self, Opponent: var oppo} when (self > 0 && oppo <= 0)
		 => "You win",
		{Self: var self, Opponent: var oppo} when (self <= 0  && oppo > 0)
		 => "You lose",
		{Self: var self, Opponent: var oppo} 
		 => "Even-even"
	};

	Console.WriteLine($"{msg}\n{points.Self}/yours : {points.Opponent}/opponent's");
    if (i < game_repeat - 1) { // except last game1
        var yn = Prompt.Select<string>(o => o.WithMessage("Modify parameters?:")
                                          .WithItems(new[] { "yes", "no" })
                                          .WithDefaultValue("no"));
        if (yn == "yes") {
			modified = true;
            Console.WriteLine("Starting to modify parameters..");
            opt = new_opts(opt);
            Console.WriteLine("Parameters are renewed.");
        }
    }
    //Console.Write($"{msg}. Hit any key:");
    //Console.ReadKey();
	ppoints[i] = points;
	Options new_opts(Options opts) {
		var new_oppo_delay = Prompt.Input<int>(o => o.WithMessage("opponent delay:").WithDefaultValue(opts.oppo_delay));
		var new_speed = Prompt.Input<int>(o => o.WithMessage("self speed:").WithDefaultValue(opts.speed));
		var new_oppo_speed = Prompt.Input<int>(o => o.WithMessage("opponent's speed:").WithDefaultValue(opts.oppo_speed));
		var new_ball_delay = Prompt.Input<int>(o => o.WithMessage("ball delay:").WithDefaultValue(opts.ball_delay));
		var new_delay = Prompt.Input<int>(o => o.WithMessage("delay(refresh rate):").WithDefaultValue(opts.delay));
		var new_opts = opts with {oppo_delay = new_oppo_delay, speed = new_speed, oppo_speed = new_oppo_speed, ball_delay = new_ball_delay, delay = new_delay};
		return new_opts;
	}
}
Debug.WriteLine("Writing points to: " + points_xml_file);
Points.SSaveXML(ppoints, points_xml_file);
	// Save options to XML
if(modified || opt.save_to_xml != ""){
	Debug.WriteLine($"Asking to save options to XML file \"{opt.save_to_xml}\".");
	var yn = Prompt.Select<string>(o => o.WithMessage("Save parameters to XML file?")
                                          .WithItems(new[] { "yes", "no" })
                                          .WithDefaultValue("no"));
	if(yn == "yes"){
		if (opt.save_to_xml == "") {
			var filename = Prompt.Input<string>(o => o.WithMessage("Options XML filename:")
			.WithDefaultValue(Options.XmlName));
			opt.SaveXML(filename);
		} else
			opt.SaveXML(opt.save_to_xml);
		Debug.WriteLine($"Saving options to XML file \"{opt.save_to_xml}\"."); 
	}
}

Console.CursorVisible = true;

