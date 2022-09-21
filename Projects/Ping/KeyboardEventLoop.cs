using System;
using System.Threading;
using System.Threading.Tasks;
namespace sample;
/// Reference: https://ufcpp.net/study/csharp/sp_event.html
/// delegate for keyboard event
delegate void KeyboadEventHandler(ConsoleKeyInfo eventCode);

/// <summary>
/// Keyboard input event waiter class
/// </summary>
class KeyboardEventLoop
{
    /// <summary>
    /// キー入力があった時に呼ばれるイベント。
    /// </summary>
    public event KeyboadEventHandler? OnKeyDown;

    public KeyboardEventLoop() { }
    public KeyboardEventLoop(KeyboadEventHandler onKeyDown)
    {
        this.OnKeyDown += onKeyDown;
    }

    /// <summary>
    /// 待受け開始。
    /// </summary>
    /// <param name="ct">待ち受けを終了したいときにキャンセルする。</param>
    public Task Start(CancellationToken ct)
    {
        return Task.Run(() => EventLoop(ct));
    }

    /// <summary>
    /// イベント待受けループ。
    /// </summary>
    void EventLoop(CancellationToken ct)
    {
        // イベントループ
        while (!ct.IsCancellationRequested)
        {
            // 文字を読み込む
            // (「キーが押される」というイベントの発生を待つ)
            ConsoleKeyInfo eventCode = Console.ReadKey();
            // if (keyInfo.Key) char eventCode = (line == null || line.Length == 0) ? '\0' : line[0];
            // イベント処理は event を通して他のメソッドに任せる。
            OnKeyDown?.Invoke(eventCode);
        }
    }
}