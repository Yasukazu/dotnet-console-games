using Microsoft.Extensions.Configuration;
// using System.Text.Json;
// using JsonConfig;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json");
                //.AddEnvironmentVariables(prefix: "DOTNET_")
                //.AddUserSecrets<Program>(optional: true);
            var configuration = builder.Build();

            Console.WriteLine(configuration["rotation"]);
            Console.WriteLine(configuration["speed_ratio"]);

//var rotation = Config.Default.rotation;
//Console.WriteLine($"rotation:{rotation}");
//var speed_ratio = Config.Default.speed_ratio;

/*
OptionsConfig config = new(){
rotation=0,
speed_ratio=1,
screen_width=48,
screen_height=24,
paddle_width=8,
refresh_delay=100,
oppo_delay=300,
ball_delay=100,
ball_angle=15
};*/

// string strJson = JsonSerializer.Serialize<OptionsConfig>(config);
// Console.WriteLine(strJson);
    /* var configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddJsonFile("appsettings.json")
        .Build();*/
            /* var HostBuilder = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    IConfigurationRoot configurationRoot = configuration.Build();
                    MyConfig options = new();
                    configurationRoot.GetSection(nameof(MyConfig)).Bind(options);

                    Console.WriteLine($"Setting2は「{ options.Setting2}」です");
                });
            using var host = HostBuilder.Build(); */
        /* var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(builder =>
        {
            builder.Sources.Clear();
            builder.AddConfiguration(configuration);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .Build();    */
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