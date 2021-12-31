# Difference from original

1. Easier opponent control in gameloop by Task and lambda:

``` c#
// When opponent found a need to move itself:
  Task.Run(()=>{
      Task.Delay(opponentDelay).Wait(); // Delay before move
      Opponent.Move();
      Opponent.Draw();
  });
```
