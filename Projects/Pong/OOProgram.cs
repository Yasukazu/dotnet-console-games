
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
// var _rotation = 90; // Rotation.Horizontal;
var clargs = Environment.GetCommandLineArgs();
var pArgs = clargs[1..];
ParserResult<Options> parseResult = Parser.Parse<Options>(pArgs);
Options copt = parseResult.Value; // command
// Load from XML file into options
string? load_from_xml_file = copt.load_from_xml;
Options? xopt = null;
if (load_from_xml_file == "") {
    var env_xml_file = Environment.GetEnvironmentVariable("GAME_PONG_FILE_OPTIONS");
    if (env_xml_file != null)
        try {
            xopt = Options.LoadXML(env_xml_file);
        }
        catch (IOException ex) {
            Debug.WriteLine(env_xml_file + "]:Options XML file I/O Error:[" + ex.Message);
        }
}
Options opt = (xopt != null) ? Options.MergeXML(xopt, copt) : copt;

// modify opt by last games
 var points_xml_file = @"points.xml" ;

// adjust with environment screen size
 // (width, height) = OnScreen.init(opt.width, opt.height);
    opt.width = opt.width > 0 ? opt.width : Math.Min(opt.width, System.Console.WindowWidth);
    opt.height = opt.height > 0 ? opt.height : Math.Min(opt.height, System.Console.WindowHeight);

int game_repeat = 3;
string? game_env_repeat_str = Environment.GetEnvironmentVariable("GAME_PONG_REPEAT");
if(game_env_repeat_str != null){
    try {
        game_repeat = Convert.ToInt32(game_env_repeat_str);
    }
    catch(Exception ex) {
        Debug.WriteLine("repeat conversion failed:" + ex.Message);
    }
}

Points[] ppoints = new Points[game_repeat];

Points env_scores = new(0,0);
string? game_env_scores_str = Environment.GetEnvironmentVariable("GAME_PONG_SCORES");
if(game_env_scores_str != null){
    try {
        var scores_s = game_env_scores_str.Split(':');
        if(scores_s.Length >= 2){
            env_scores = new(Convert.ToInt32(scores_s[0]), Convert.ToInt32(scores_s[1]));
        }
    }
    catch(Exception ex) {
        Debug.WriteLine("scores conversion failed:" + ex.Message);
    }
}

Game game;
bool modified = false;
for ( int i = 0; i < game_repeat; ++i){
    Console.Clear(); //Write("Hit any key to start:");
	game = new Game(opt); // speed_ratio, screen_w, screen_h, paddle_width, rot, delay, oppo_delay, ball_delay, ball_angle);
	game.Run(env_scores with { });
    Console.SetCursorPosition(0, opt.height);

	string msg =
	game.score switch {
		{Self: var self, Opponent: var oppo} when (self > 0 && oppo <= 0)
		 => "You win",
		{Self: var self, Opponent: var oppo} when (self <= 0  && oppo > 0)
		 => "You lose",
		{Self: var self, Opponent: var oppo} 
		 => "Even-even"
	};

	Console.WriteLine($"{msg}\n{game.score.Self}/yours : {game.score.Opponent}/opponent's");
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
	ppoints[i] = game.score;
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
    if (yn == "yes") {
        try {
            if (opt.save_to_xml == "") {
                var filename = Prompt.Input<string>(o => o.WithMessage("Options XML filename:")
                .WithDefaultValue(Options.XmlName));
                opt.save_to_xml = filename;
                opt.SaveXML();
            }
            else
                opt.SaveXML();
            Debug.WriteLine($"Saving options to XML file \"{opt.save_to_xml}\".");
        }
        catch (IOException ex) {
            Console.WriteLine(ex.Message + "\nError in saving file:" + opt.save_to_xml);
        }
    }
}
Console.CursorVisible = true;

