# Difference from original

0. Branch name: "oop_dev"
1. Easier opponent control in gameloop by Task and lambda:
3. Additional repos.: https://github.com/shibayan/Sharprompt

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
