// Copyright (c) 2012 Chris Taylor

using System;
using System.Drawing;
using TinyCLR.Game;


namespace FEZMan
{
  class Ghost : GameCharacter
  {
    private static readonly Random Prng = new Random();

    #region Animation Selection Constants

    // Changes to these constants must match the order 
    // of the animation sequences defined in the ctor.
    private const int AnimateLeft = 0;
    private const int AnimateRight = 1;
    private const int AnimateUp = 2;
    private const int AnimateDown = 3;
    private const int AnimateFrightened = 4;
    private const int AnimateRecovering = 5;
    private const int AnimateDeadLeft = 6;
    private const int AnimateDeadRight = 7;
    private const int AnimateDeadUp = 8;
    private const int AnimateDeadDown = 9;

    #endregion

    
    private const int DefaultChaseTime = 20000;
    private const int DefaultScatterTime = 7000;
    private const int DefaultFrightendTime = 6000;
    private const int DefaultWarnTime = 2000;

    private readonly int _chaseTime = DefaultChaseTime;
    private int _scatterTime = DefaultScatterTime;
    private int _frightendTime = DefaultFrightendTime;
    private int _warnTime = DefaultWarnTime;

    private GhostState _state = GhostState.Chase;
    private readonly Personality _personality;
    protected Player Player { get; set; }
    public int Level { get; set; }

    private readonly CountDownTimer _stateCountDown = new CountDownTimer(0);

    public Ghost(Bitmap spriteSheet, Maze maze, Player player, int spriteSheetYOffset, Personality personality)
      : base(maze)
    {
      Player = player;
      _personality = personality;

      // NB: The sequence of the animations must match the 
      //     "Animation Selection Constants" defined above      
      SetAnimationSequences(true, new AnimationSequence[]
      { 
        // 0 - Left
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(0, spriteSheetYOffset, 8, 8),
          new Rect(8, spriteSheetYOffset, 8, 8),
        }),
        // 1 - Right
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(16, spriteSheetYOffset, 8, 8),
          new Rect(24, spriteSheetYOffset, 8, 8),
        }),
        // 2 - Up
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(32, spriteSheetYOffset, 8, 8),
          new Rect(40, spriteSheetYOffset, 8, 8),
        }),
        // 3 - Down
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(48, spriteSheetYOffset, 8, 8),
          new Rect(56, spriteSheetYOffset, 8, 8),
        }),        

        // 4 - Frightened
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(64, 8, 8, 8),
          new Rect(72, 8, 8, 8),
        }),

        // 5 - Recovering
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(64, 8, 8, 8),
          new Rect(64, 16, 8, 8),
          new Rect(72, 8, 8, 8),
          new Rect(72, 16, 8, 8),
        }),
        // 6 - Left Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(64, 24, 8, 8),          
        }),
        // 7 - Right Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(72, 24, 8, 8),          
        }),
        // 8 - Up Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(64, 32, 8, 8),          
        }),
        // 9 - Down Dead
        new AnimationSequence(spriteSheet, 6, new Rect[] 
        {
          new Rect(72, 32, 8, 8),          
        }),
      });

      Reset();

      MessageService.Instance.Subscribe(typeof(Messages.AtePowerPillMessage), HandleAtePowerPillMessage);
      MessageService.Instance.Subscribe(typeof(Messages.PacmanDyingMessage), HandlePacmanDyingMessage);
      MessageService.Instance.Subscribe(typeof(Messages.PacmanDeadMessage), HandlePacmanDeadMessage);

      _stateCountDown.Expired += StateCountDown_Expired;
    }

    #region Message subscription handlers
    private void HandleAtePowerPillMessage(object message)
    {
      if (_state != GhostState.Dead)
      {
        SetState(GhostState.Frightened);
        CurrentAnimation = AnimateFrightened;
      }
    }

    private void HandlePacmanDyingMessage(object message)
    {
      Active = false;
    }

    private void HandlePacmanDeadMessage(object message)
    {
      Reset();
    }

    private void StateCountDown_Expired(object sender, EventArgs e)
    {
      switch (_state)
      {
        case GhostState.Chase:
          SetState(GhostState.Scatter); break;
        case GhostState.Scatter:
          SetState(GhostState.Chase); break;
        case GhostState.Frightened:
          SetState(GhostState.Recovering); break;
        case GhostState.Recovering:
          SetState(GhostState.Scatter); break;
      }
    }

    #endregion

    public bool IsFrightened { get { return _state == GhostState.Frightened || _state == GhostState.Recovering; } }
    public bool IsDead { get { return _state == GhostState.Dead; } }
    public bool IsInHome
    {
      get
      {
        int xCell = X >> 3;
        int yCell = Y >> 3;
        return (yCell == 7) && (xCell >= 6 && xCell <= 8);
      }
    }

    public void Die()
    {
      SetState(GhostState.Dead);
    }

    private int DistanceToTarget(int tx, int ty, Direction direction)
    {
      int dx = 0;
      int dy = 0;

      switch (direction)
      {
        case Direction.Left: dx = tx - (X - 1); dy = ty - Y; break;
        case Direction.Right: dx = tx - (X + 1); dy = ty - Y; break;
        case Direction.Up: dy = ty - (Y - 1); dx = tx - X; break;
        case Direction.Down: dy = ty - (Y + 1); dx = tx - X; break;
      }
      return (dx * dx) + (dy * dy);
    }

    public override void Reset()
    {
      Active = true;
      SetState(GhostState.Scatter);
      MoveTo(_personality.StartX, _personality.StartY);

      // Increase difficulty based on the current level
      _scatterTime = DefaultScatterTime - ((Level - 1) * 500);
      _frightendTime = DefaultFrightendTime - ((Level - 1) * 500);
      _warnTime = DefaultWarnTime - ((Level - 1) * 100);

      _scatterTime = System.Math.Max(_scatterTime, 500);
      _frightendTime = System.Math.Max(_scatterTime, 500);
      _warnTime = System.Math.Max(_warnTime, 500);
    }

    private void SetState(GhostState state)
    {
      _state = state;
      TargetSpeed = 4;
      switch (state)
      {
        case GhostState.Scatter: _stateCountDown.Start(_scatterTime); break;
        case GhostState.Chase: _stateCountDown.Start(_chaseTime); break;
        case GhostState.Frightened:
          _stateCountDown.Start(_frightendTime);
          if (Maze.IsCellOpen(this, GetOppositeDirection()))
          {
            CurrentDirection = GetOppositeDirection();
          }
          TargetSpeed = 2;
          break;
        case GhostState.Recovering:
          _stateCountDown.Start(_warnTime);
          TargetSpeed = 2;
          break;
        case GhostState.Dead:
          if (Maze.IsCellOpen(this, GetOppositeDirection()))
          {
            CurrentDirection = GetOppositeDirection();
          }
          break;
      }
      SetCharacterAnimation();
    }

    protected override Direction OnSelectNewDirection(int xCell, int yCell)
    {
      Direction newDirection = Direction.Stop;

      if (!IsDead && Prng.NextDouble() < 0.3)
      {
        newDirection = Wander();
      }
      else if (_state == GhostState.Scatter)
      {
        newDirection = HuntTarget(
          _personality.ScatterX,
          _personality.ScatterY);
      }
      else if (_state == GhostState.Chase)
      {
        newDirection = HuntTarget(Player);
      }
      else if (IsFrightened)
      {
        newDirection = Wander();
      }
      else if (IsDead)
      {
        newDirection = HuntTarget(
          _personality.HouseX,
          _personality.HouseY);

        if (IsInHome)
        {
          SetState(GhostState.Scatter);
        }
      }

      return newDirection;
    }

    #region Ghost movement strategies
    private Direction Wander()
    {
      Direction[] open;
      open = Maze.GetOpenDirections(this, CurrentDirection);

      int count = 0;
      int length = open.Length;
      for (int i = 0; i < length; ++i)
      {
        if (open[i] == Direction.Stop) break;
        count++;
      }
      return open[Prng.Next(count)];
    }

    private Direction HuntTarget(Player player)
    {
      int tx = player.X;
      int ty = player.Y;

      switch (player.CurrentDirection)
      {
        case Direction.Left: tx -= _personality.TargetOffsetX; break;
        case Direction.Right: tx += _personality.TargetOffsetX; break;
        case Direction.Up: ty -= _personality.TargetOffsetY; break;
        case Direction.Down: ty += _personality.TargetOffsetY; break;
      }

      return HuntTarget(tx, ty);
    }

    private Direction HuntTarget(int tx, int ty)
    {
      Direction[] open = Maze.GetOpenDirections(this, CurrentDirection);
      Direction minDirection = open[0];
      int minDistance = int.MaxValue;
      int length = open.Length;
      for (int i = 0; i < length; ++i)
      {
        var dir = open[i];
        if (dir == Direction.Stop) break;
        int distance = DistanceToTarget(tx, ty, dir);
        if (distance < minDistance)
        {
          minDirection = dir;
          minDistance = distance;
        }
      }
      return minDirection;
    }
    #endregion

    protected override void SetCharacterAnimation()
    {
      if (_state == GhostState.Frightened)
      {
        CurrentAnimation = AnimateFrightened;
      }
      else if (_state == GhostState.Recovering)
      {
        CurrentAnimation = AnimateRecovering;
      }
      else if (IsDead)
      {
        switch (CurrentDirection)
        {
          case Direction.Left: CurrentAnimation = AnimateDeadLeft; break;
          case Direction.Right: CurrentAnimation = AnimateDeadRight; break;
          case Direction.Up: CurrentAnimation = AnimateDeadUp; break;
          case Direction.Down: CurrentAnimation = AnimateDeadDown; break;
        }
      }
      else
      {
        switch (CurrentDirection)
        {
          case Direction.Left: CurrentAnimation = AnimateLeft; break;
          case Direction.Right: CurrentAnimation = AnimateRight; break;
          case Direction.Up: CurrentAnimation = AnimateUp; break;
          case Direction.Down: CurrentAnimation = AnimateDown; break;
        }
      }
    }

    public override bool CanEnterCell(byte value)
    {
      // Can move through the house door when dead or already in the ghost house
      if (value == MazeCell.HouseDoor)
      {
        return (IsDead || IsInHome);
      }

      return value != MazeCell.Wall
        && value != MazeCell.HouseDoor
        && value != MazeCell.Teleport;
    }

    public override void Update(float elapsedTime)
    {
      if (!Active) return;

      if (!IsDead)
      {
        if (Maze.IsColliding(X, Y, Player.X, Player.Y))
        {
          MessageService.Instance.Publish(Messages.CollidedWithPacmanMessage.Message(this));
          if (IsFrightened) Die();
        }
      }

      base.Update(elapsedTime);
    }

    public override void Draw()
    {
      if (!Active) return;
      base.Draw();
    }
  }

  class Personality
  {
    public int ScatterX { get; set; }
    public int ScatterY { get; set; }
    public int TargetOffsetX { get; set; }
    public int TargetOffsetY { get; set; }
    public int HouseX { get; set; }
    public int HouseY { get; set; }
    public int StartX { get; set; }
    public int StartY { get; set; }
  }

  enum GhostState
  {
    Chase,
    Scatter,
    Frightened,
    Recovering,
    Dead
  }
}
