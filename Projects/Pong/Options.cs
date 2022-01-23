
public record Options {
	[Option('r', "rotation", Required =false, HelpText = "rotation default 0(not rotated) and others are 90(, 180 and 270).")]
	public int rotation { get; set;} = 0;
	[Option('s', "speed", Required =false, HelpText = "paddle speed ratio default 1")]
	public int speed { get; set;} = 4;
	[Option('S', "oppo_speed", Required =false, HelpText = "opponent's paddle speed ratio default 1")]
	public int oppo_speed { get; set;} = 2;
	[Option('w', "width", Required =false, HelpText = "screen width default 64")]
	public int width {get; set;} = 32;
	[Option('h', "height", Required =false, HelpText = "screen height default 24")]
	public int height {get; set;} = 24;
	[Option('p', "paddle", Required =false, HelpText = "paddle width default 8")]
	public int paddle {get; set;} = 8;
	[Option('d', "delay", Required =false, HelpText = "Loop refresh rate. default 100")]
	public int delay {get; set;} = 20;
	[Option('o', "opponent delay", Required =false, HelpText = "opponent delay rate. default 200")]
	public int oppo_delay {get; set;} = 80;
	[Option('b', "ball-delay", Required =false, HelpText = "ball delay rate. default 200")]
	public int ball_delay {get; set;} = 100;
	[Option('a', "initial-angle", Required =false, HelpText = "initial ball angle. default 0(random)")]
	public int ball_angle {get; set;} = 17;
	[Option('v', "save-to-XML", Required =false, HelpText = "Save options to XML file name. default ''(void).")]
	public string save_to_xml {get; set;} = "";

	[Option('l', "load-from-XML", Required =false, HelpText = "Load options from XML file name. default ''.")]
	public string load_from_xml {get; set;} = "";
	[Option('b', "opponent about", Required =false, HelpText = "opponent about at find difference. default 1")]
	public int oppo_about {get; set;} = 1;
	public static string XmlName = @"options.xml";
    public void SaveXML() {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Options));
        using(System.IO.StreamWriter writer = new(save_to_xml, false, new System.Text.UTF8Encoding(false))){
            serializer.Serialize(writer, this);
        }
    }
	public static Options LoadXML(string load_from_xml) {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Options));
		Options opt;
        using(System.IO.StreamReader reader = new(load_from_xml)){
			opt = (Options)serializer.Deserialize(reader);
        }
		return opt;
    }

	public static Options MergeXML(Options orgo, Options newo){
		Options defo = new Options(); // default
		Options ropts = orgo with {
			rotation = newo.rotation != defo.rotation ? newo.rotation : orgo.rotation,
			speed = newo.speed != defo.speed ? newo.speed : orgo.speed,
			oppo_speed = newo.oppo_speed != defo.oppo_speed ? newo.oppo_speed : orgo.oppo_speed,
			width = newo.width != defo.width ? newo.width : orgo.width,
			height = newo.height != defo.height ? newo.height : orgo.height, 
			paddle = newo.paddle != defo.paddle ? newo.paddle : orgo.paddle, 
			delay = newo.delay != defo.delay ? newo.delay : orgo.delay, 
			oppo_delay = newo.oppo_delay != defo.oppo_delay ? newo.oppo_delay : orgo.oppo_delay, 
			ball_delay = newo.ball_delay != defo.ball_delay ? newo.ball_delay : orgo.ball_delay, 
			ball_angle = newo.ball_angle != defo.ball_angle ? newo.ball_angle : orgo.ball_angle, 
			save_to_xml = newo.save_to_xml != defo.save_to_xml ? newo.save_to_xml : orgo.save_to_xml, 
			load_from_xml = newo.load_from_xml != defo.load_from_xml ? newo.load_from_xml : orgo.load_from_xml, 
			oppo_about = newo.oppo_about != defo.oppo_about ? newo.oppo_about : orgo.oppo_about, 
		};
		return ropts;
	}
}