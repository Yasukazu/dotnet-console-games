namespace ping;
using Automatonymous;

///  <summary>
/// State machine for game state
/// </summary>
/// <reference>
/// https://tech-blog.cloud-config.jp/2019-10-28-statemachine-automatonymous/
/// https://www3.ntu.edu.sg/home/ehchua/programming/java/J8d_Game_Framework.html
/// /// </reference>
public enum State {
   INITIALIZED, READY, PLAYING, PAUSE, GAMEOVER, DESTROYED
}
/*
    initGame(): Perform one-time initialization tasks, such as constructing object, opening files, setup timers and key/mouse event handlers.
    newGame(): Perform per-game initialization tasks, such as reset the score, clear the game board, etc.
    startGame(): Start all the timers.
    stopGame(): Stop timers, update game statistics such as high score.
    destroyGame(): housekeeping tasks before program exit.
    stepGame(): 2 kinds of triggers: user action such as key-press or button-push and/or timer.
    pauseGame()/resumeGame(): suspend/resume the timers.
*/
public interface StateTransition {
   /**
    * Perform one-time initialization tasks, such as constructing game objects,
    * opening files, setting up the timers, audio and images, and setting up
    * the key/mouse event handlers.
    */
   void initGame() {}

   /**
    * Perform per-game initialization tasks for a new game, such as
    * resetting the score and all the game properties, clear the board, etc.
    */
   void newGame();

   /**
    * Start the game, e.g., start the timers
    */
   void startGame();

   /**
    * Stop the game (game over), stop the timers and update game statistics
    * such as high score.
    */
   void stopGame();

   /**
    * Run one step of the game, either due to user action (via key/mouse) or timer task.
    * Hard to define here as it may have different parameters and return type.
    */
// default Xxx stepGame(Aaa) { }

   /**
    * Pause the game, e.g., pause the timers
    */
   void pauseGame();

   /**
    * Resume the paused game, e.g., resume the timers
    */
   void resumeGame();

   /**
    * Perform housekeeping tasks such as closing the files before exiting.
    */
   void destroyGame() { }
}
/// state machine class
class StateClass
{
    /// <summary>
    /// keeps current state
    /// </summary>
    /// <value></value>
    public State CurrentState { get; set; }
}

class StateMachineBehavior : AutomatonymousStateMachine<StateClass>
{
    public State Initialized {get; private set;}
    public State Ready {get; private set;}
    public State Playing {get; private set;}
    public State Pause {get; private set;}
    public State Gameover {get; private set;}
    public Event InitializedToReady {get; private set;}
    public Event ReadyToPlaying {get; private set;}
    public Event PlayingToPause {get; private set;}
    public Event PauseToPlaying {get; private set;}
    public Event PlayingToReady {get; private set;}
    public Event PauseToGameover {get; private set;}
    public Event ToInitialized {get; private set;}
    public StateMachineBehavior() {
      /// action when initial state
      Initially(
         When(ToInitialized)
         .Then(context =>
         initialize()).TransitionTo(Initialized);
         )
      )
    }
   
   public void initialize() {
      Debug.Print("To Initialized from initial state.");

   }
}


