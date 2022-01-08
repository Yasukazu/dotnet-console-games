
﻿global using System;
global using System.Linq;
global using System.Collections;
global using System.Collections.Generic;
global using System.Diagnostics;
global using CommandLineParser; // Original source code: https://github.com/wertrain/command-line-parser-cs (Version 0.1)
global using System.Threading.Tasks;
global using System.IO;

// ConsoleTraceListener myWriter = new GonsoleTraceListener();
// Trace.Listeners.Add(myWriter);
// Debug.Write("myWriter is added to Trace.Listeners.  OOProgram start.");
var _rotation = 90; // Rotation.Horizontal;
var clargs = Environment.GetCommandLineArgs();
var pArgs = clargs[1..];
ParserResult<Options> parseResult = Parser.Parse<Options>(pArgs);
Options opt = parseResult.Value;
// Load from XML file into options
Options new_opt = null;
if(opt.load_from_xml != ""){
	Debug.WriteLine($"Starting to load XML file:{opt.load_from_xml}");
	new_opt = opt.LoadXML(opt.load_from_xml);
}
if(new_opt != null){
	Debug.WriteLine($"Replacing opt with new_opt.");
	opt = new_opt;
}
// modify opt by last games
var points_xml_file = @"points.xml" ;
if(File.Exists(points_xml_file)){
	Debug.WriteLine("Opning: " + points_xml_file);
	Points[] pp = Points.LLoadXML(points_xml_file);
	var selfPoints = pp.Select(x => x.Self).Sum();
	var oppoPoints = pp.Select(x => x.Opponent).Sum();
	if (selfPoints > oppoPoints){
		var minus = 10;
		Console.WriteLine("oppo delay -{minus}");
		opt.oppo_delay -= minus;
	}
	else if (selfPoints < oppoPoints){
		var plus = 50;
		Console.WriteLine("ball delay +{plus}");
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
    Console.Write($"{msg}. Hit any key:");
    Console.ReadKey();
	ppoints[i] = points;
}
Debug.WriteLine("Writing points to: " + points_xml_file);
Points.SSaveXML(ppoints, points_xml_file);
Console.Write("Hit any key to finish:");
Console.CursorVisible = true;
