using System;
using System.Threading;
using System.Threading.Tasks;
namespace sample;
public class EventSample
{
    // 時刻の表示形式
    const string FULL = "yyyy/dd/MM hh:mm:ss\n";
    const string DATE = "yyyy/dd/MM\n";
    const string TIME = "hh:mm:ss\n";

    KeyboardEventLoop eventLoop;
    static bool isSuspended = true;  // プログラムの一時停止フラグ。
    static string timeFormat = TIME; // 時刻の表示形式。
    CancellationTokenSource cts;

    public EventSample() {
        cts = new CancellationTokenSource();
        eventLoop = new KeyboardEventLoop(code => OnKeyDown(code, cts));
    }
    public void Run()
    {
        WriteHelp();
        Task.WhenAll(
            eventLoop.Start(cts.Token),
            TimerLoop(cts.Token)
            ).Wait();
    }

    // 毎秒時刻表示のループ
    private static async Task TimerLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (!isSuspended)
            {
                // 1秒おきに現在時刻を表示。
                Console.Write(DateTime.Now.ToString(timeFormat));
            }
            await Task.Delay(1000);
        }
    }

    // イベント処理部。
    static void OnKeyDown(ConsoleKeyInfo eventCode, CancellationTokenSource cts)
    {
        if (eventCode.Key == ConsoleKey.Escape) {
            cts.Cancel();
            return;
        }
        switch (char.ToLower(eventCode.KeyChar))
        {
            case 'r': // run
                isSuspended = false;
                break;
            case 's': // suspend
                Console.Write("\n一時停止します\n");
                isSuspended = true;
                break;
            case 'f': // full
                timeFormat = FULL;
                break;
            case 'd': // date
                timeFormat = DATE;
                break;
            case 't': // time
                timeFormat = TIME;
                break;
            case 'q': // quit
                cts.Cancel();
                break;
            default: // ヘルプ
                WriteHelp();
                break;
        }
    }

    public static void WriteHelp()
    {
        Console.Write(
            "Usage:\n" +
            "r (run)    : Start intervally print date and/or time.\n" +
            "s (suspend): 時刻表示を一時停止します。\n" +
            "f (full)   : 時刻の表示形式を“日付＋時刻”にします。\n" +
            "d (date)   : 時刻の表示形式を“日付のみ”にします。\n" +
            "t (time)   : 時刻の表示形式を“時刻のみ”にします。\n" +
            "q (quit)   : プログラムを終了します。\n"
            );
    }
}
