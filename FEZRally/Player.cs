// Copyright (c) 2012 Chris Taylor
using System;
using TinyCLR.Game;
using System.Collections;
using System.Drawing;

namespace FEZRally
{
  class Player : GameCharacter
  {
    #region Animation Selection Constants

    // Changes to these constants must match the order 
    // of the animation sequences defined in the ctor.
    private const int AnimateUp = 0;
    private const int AnimateRight = 1;
    private const int AnimateDown = 2;
    private const int AnimateLeft = 3;
    private const int AnimateDying = 4;

    #endregion
        
    private Direction _lastDirection;
    private readonly int _startX;
    private readonly int _startY;
    private int _lastTileX;
    private int _lastTileY;
    private int _flagCount = 0;
    
    private readonly CountDownTimer _stateTimer = new CountDownTimer(0);
    private readonly CountDownTimer _fuelTimer = new CountDownTimer(1000, true);
    private float _levelTime = 0;
    
    public GameCharacter[] Enemies { get; set; }

    public Player(Bitmap spriteSheet, Maze maze, int startTileX, int startTileY) 
      : base(maze)
    {
      PlayerCharacter = true;
      _startX = startTileX * TileViewer.TileSize;
      _startY = startTileY * TileViewer.TileSize;
      TargetSpeed = 2;
      // NB: The sequence of the animations must match the 
      //     "Animation Selection Constants" defined above      
      SetAnimationSequences(false,
        new AnimationSequence[]
        {
          // 0 - Up
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(0, 0, 8, 8)}),
          
          // 1 - Right
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(8, 0, 8, 8)}),

          // 2 - Down
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(16, 0, 8, 8)}),

          // 3 - Left
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(24, 0, 8, 8)}),

          // 4 - Dying
          new AnimationSequence(spriteSheet, -1, new Rect[]{new Rect(0, 32, 8, 8)}),
        });
           
      _stateTimer.Expired += new EventHandler(StateTimer_Expired);
      _fuelTimer.Expired += new EventHandler(FuelTimer_Expired);

      NewGame();
    }

    void FuelTimer_Expired(object sender, EventArgs e)
    {
      DecrementFuel();      
    }

    void StateTimer_Expired(object sender, EventArgs e)
    {
      if (State == CharacterState.Dying)
      {
        Lives--;
        if (Lives == 0)
        {
          MessageService.Instance.Publish(Messages.GameOverMessage.Message());
        }

        MessageService.Instance.Publish(Messages.ResetLevelMessage.Message());        
      }
    }

    public int Fuel { get; set; }

    public void NewGame()
    {
      Score = 0;
      Lives = 3;
      Fuel = 100;
      _flagCount = 0;

      Reset();
    }

    public void NewLevel()
    {
      Fuel = 100;
      _flagCount = 0;      
      Reset();
    }

    public override void Reset()
    {      
      State = CharacterState.Alive;
      CurrentAnimation = AnimateUp;
      CurrentDirection = Direction.Up;      
      MoveTo(_startX, _startY);

      _lastTileX = -1;
      _lastTileY = -1;
      _fuelTimer.Start();
      _levelTime = 0;
    }

    public void Kill()
    {
      if (State == CharacterState.Alive)
      {
        State = CharacterState.Dying;
        CurrentDirection = Direction.Stop;
        CurrentAnimation = AnimateDying;
        if (Lives > 0) Lives--;
        _fuelTimer.Cancel();
        MessageService.Instance.Publish(Messages.PlayerDiedMessage.Message(Lives));
      }
    }

    protected override Direction OnSelectNewDirection(int xCell, int yCell)
    {
      return PickLeastTravelledDirection(_lastDirection);      
    }

    protected override void SetCharacterAnimation()
    {
      switch (CurrentDirection)
      {
        case Direction.Left: CurrentAnimation = AnimateLeft; break;
        case Direction.Right: CurrentAnimation = AnimateRight; break;
        case Direction.Up: CurrentAnimation = AnimateUp; break;
        case Direction.Down: CurrentAnimation = AnimateDown; break;      
      }
    }

    public override void Update(float elapsedTime)
    {
      _levelTime += elapsedTime;

      if (State == CharacterState.Alive && Lives > 0)
      {
        double x = GameManager.Game.InputManager.X;
        double y = GameManager.Game.InputManager.Y;

        if (x > 0.3)
        {
          TargetDirection = Direction.Right;
        }
        else if (x < -0.3)
        {
          TargetDirection = Direction.Left;
        }
        else if (y < -0.3)
        {
          TargetDirection = Direction.Up;
        }
        else if (y > 0.3)
        {
          TargetDirection = Direction.Down;
        }

        int tileX = (X + TileViewer.HalfTileSize) >> TileViewer.TileSizeShift;
        int tileY = (Y + TileViewer.HalfTileSize) >> TileViewer.TileSizeShift;

        if (_levelTime > 1 && GameManager.Game.InputManager.Button1 && Fuel > 0)
        {
          bool dropped = false;
          switch (GetOppositeDirection())
          {
            case Direction.Up: dropped = Maze.DropSmokeBomb(tileX, tileY - 1); break;
            case Direction.Down: dropped = Maze.DropSmokeBomb(tileX, tileY + 1); break;
            case Direction.Left: dropped = Maze.DropSmokeBomb(tileX - 1, tileY); break;
            case Direction.Right: dropped = Maze.DropSmokeBomb(tileX + 1, tileY); break;
          }
          if (dropped) DecrementFuel();
        }

        if (_lastTileX != tileX || _lastTileY != tileY)
        {
          _lastTileX = tileX;
          _lastTileY = tileY;
          int cell = Maze.GetTileValue(tileX, tileY);
          switch (cell)
          {
            case Tile.Rock:
              {
                Maze.SetTileValue(tileX, tileY, Tile.Empty);
                Kill();
              }
              break;

            case Tile.Flag:
              {
                Maze.SetTileValue(tileX, tileY, Tile.Empty);
                _flagCount++;
                Score += _flagCount * 100;
                MessageService.Instance.Publish(Messages.AteFlagMessage.Message(tileX, tileY, _flagCount * 100, _flagCount));
              }
              break;
          }
        }
      }      

      base.Update(elapsedTime);      

      if (CurrentDirection != Direction.Stop)
      {
        _lastDirection = CurrentDirection;
      }      
    }

    private void DecrementFuel()
    {
      Fuel = System.Math.Max(0, Fuel-1);
      if (Fuel == 0 && Lives > 0)
      {
        Lives = 0;
        Fuel = 0;
        Kill();
      }
    }    
  }
}