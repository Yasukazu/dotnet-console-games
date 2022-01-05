
public class Options {
	[Option('r', "rotation", Required =false, HelpText = "rotation default 0(not rotated) and others are 90(, 180 and 270).")]
	public int rotation { get; set;} = 0;
	[Option('s', "speed", Required =false, HelpText = "paddle speed ratio default 1")]
	public int speed { get; set;} = 1;
	[Option('w', "width", Required =false, HelpText = "screen width default 64")]
	public int width {get; set;} = 32;
	[Option('h', "height", Required =false, HelpText = "screen height default 24")]
	public int height {get; set;} = 24;
	[Option('p', "paddle", Required =false, HelpText = "paddle width default 8")]
	public int paddle {get; set;} = 8;
	[Option('d', "delay", Required =false, HelpText = "Loop refresh rate. default 100")]
	public int delay {get; set;} = 100;
	[Option('o', "opponent delay", Required =false, HelpText = "opponent delay rate. default 200")]
	public int oppo_delay {get; set;} = 150;
	[Option('b', "ball-delay", Required =false, HelpText = "ball delay rate. default 200")]
	public int ball_delay {get; set;} = 200;
	[Option('a', "initial-angle", Required =false, HelpText = "initial ball angle. default 0(random)")]
	public int ball_angle {get; set;} = 17;
	[Option('v', "save-to-XML", Required =false, HelpText = "Save options to XML file name. default ''(void).")]
	public string save_to_xml {get; set;} = "";

	[Option('l', "load-from-XML", Required =false, HelpText = "Load options from XML file name. default ''(void).")]
	public string load_from_xml {get; set;} = "";
	public static string XmlName = @"options.xml";
    public void SaveXML(string save_to_xml) {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Options));
        using(System.IO.StreamWriter writer = new(save_to_xml, false, new System.Text.UTF8Encoding(false))){
            serializer.Serialize(writer, this);
        }
    }
	public Options LoadXML(string lod_from_xml) {
        System.Xml.Serialization.XmlSerializer serializer = new (typeof(Options));
		Options opt;
        using(System.IO.StreamReader reader = new(load_from_xml)){
			opt = (Options)serializer.Deserialize(reader);
        }
		return opt;
    }
}