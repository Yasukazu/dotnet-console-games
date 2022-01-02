# Difference from original

1. Easier opponent control in gameloop by Task and lambda:

``` c#
// When opponent found a need to move itself:
  Task.Run(()=>{
      Task.Delay(opponentDelay).Wait(); // Delay before move
			DrawQueue.Enqueue( () => {
			  oppoPadl.Shift(diff < 0 ? -1 : 1);
		   	screen.draw(oppoPadl);
				opponentStopwatch.Restart();
  });
```
