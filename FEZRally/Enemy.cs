// Copyright (c) 2012 Chris Taylor

using System;
using System.Drawing;
using TinyCLR.Game;

namespace FEZRally
{
  class Enemy : GameCharacter
  {
    // Changes to these constants must match the order 
    // of the animation sequences defined in the ctor.
    private const int AnimateUp = 0;
    private const int AnimateRight = 1;
    private const int AnimateDown = 2;
    private const int AnimateLeft = 3;

    private const int DefaultHeadStartTime = 3000;
    private const int DefaultChaseTime = 20000;
    private const int DefaultScatterTime = 7000;
    private const int DefaultSpinTime = 3000;

    private int _headStartTime = DefaultHeadStartTime;
    private int _chaseTime = DefaultChaseTime;
    private int _scatterTime = DefaultScatterTime;
    private int _spinTime = DefaultSpinTime;

    private EnemyState _state = EnemyState.Chase;
    private readonly Personality _personality;
    protected Player Player { get; set; }

    private readonly CountDownTimer _stateCountDown = new CountDownTimer(0);
    private Direction _lastDirection = Direction.Stop;

    public Enemy(Bitmap spriteSheet, Maze maze, Player player, Personality personality)
      : base(maze)
    {
      TargetSpeed = 2;
      Player = player;
      _personality = personality;

      // NB: The sequence of the animations must match the 
      //     "Animation Selection Constants" defined above      
      SetAnimationSequences(false,
        new AnimationSequence[]
        {
          // 0 - Up
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(32, 0, 8, 8)}),
          
          // 1 - Right
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(40, 0, 8, 8)}),

          // 2 - Down
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(48, 0, 8, 8)}),

          // 3 - Left
          new AnimationSequence(spriteSheet, -1, new Rect[] {new Rect(56, 0, 8, 8)}),                   
        });

      Reset();
      
      _stateCountDown.Expired += StateCountDown_Expired;
      TargetDirection = HuntTarget(Player);
    }

    private void StateCountDown_Expired(object sender, EventArgs e)
    {
      switch (_state)
      {
        case EnemyState.HeadStart: SetState(EnemyState.Scatter); CurrentDirection = Direction.Up; break;
        case EnemyState.Chase: SetState(EnemyState.Chase); break;
        case EnemyState.Scatter: SetState(EnemyState.Chase); break;
        case EnemyState.Spin:
          Maze.ClearSmokeBomb(X >> Maze.TileSizeShift, Y >> Maze.TileSizeShift);
          CurrentDirection = GetOppositeDirection(_lastDirection); 
          SetState(EnemyState.Scatter); 
          break;
      }
    }       

    public override void Reset()
    {
      Active = true;
      SetState(EnemyState.HeadStart);
      MoveTo(_personality.StartTileX * TileViewer.TileSize, _personality.StartTileY * TileViewer.TileSize);
      
      // TODO: Increase difficulty based on the current level
      _headStartTime = DefaultHeadStartTime;
      _chaseTime = DefaultChaseTime;
      _scatterTime = DefaultScatterTime;
      _spinTime = DefaultSpinTime;
    }

    private void SetState(EnemyState state)
    {
      _state = state;      
      switch (state)
      {
        case EnemyState.HeadStart: _stateCountDown.Start(_headStartTime); break;
        case EnemyState.Scatter: _stateCountDown.Start(_scatterTime); break;
        case EnemyState.Chase: _stateCountDown.Start(_chaseTime); break;
        case EnemyState.Spin:
          _lastDirection = CurrentDirection;
          _stateCountDown.Start(_spinTime); 
          break;
      }
      SetCharacterAnimation();
    }
    
    protected override Direction OnSelectNewDirection(int xCell, int yCell)
    {
      Direction newDirection = CurrentDirection;

      if (_state == EnemyState.HeadStart)
      {
        newDirection = Direction.Stop;
      }
      else if (_state == EnemyState.Scatter)
      {
        newDirection = HuntTarget(
          _personality.ScatterX,
          _personality.ScatterY);        
      }
      else if (_state == EnemyState.Chase)
      {
        newDirection = HuntTarget(Player);
      }

      //if (newDirection != CurrentDirection)
      //  TargetSpeed = 2;
      //else
      //  TargetSpeed = 4;
      
      return newDirection;      
    }

    public override bool IsCellOpen(int tileId)
    {
      return tileId >= Tile.Empty && tileId != Tile.Rock && tileId != Tile.Smoke;    
    }

    #region Enemy movement strategies
    private Direction HuntTarget(Player player)
    {
      int tx = player.X;
      int ty = player.Y;

      if (DistanceToTarget(tx, ty) > 4096) // 64 * 64
      {
        switch (player.CurrentDirection)
        {
          case Direction.Left: tx -= _personality.TargetOffsetX; break;
          case Direction.Right: tx += _personality.TargetOffsetX; break;
          case Direction.Up: ty -= _personality.TargetOffsetY; break;
          case Direction.Down: ty += _personality.TargetOffsetY; break;
        }
      }

      return HuntTarget(tx, ty);
    }

    
    #endregion

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
      if (!Active) return;

      if ((X == Player.X || Y == Player.Y) && Maze.IsColliding(X, Y, Player.X, Player.Y))
      {
        Player.Kill();
      }
      else if (_state != EnemyState.Spin && (X & Maze.TileModMask) == 0 && (Y & Maze.TileModMask) == 0)
      {
        if (Maze.GetTileValueAtPixel(X, Y) == 20)
        {
          SetState(EnemyState.Spin);
        }
      }

      if (_state == EnemyState.Spin)
      {
        CurrentDirection = (Direction)(((int)CurrentDirection + 1) % 5);
        if (Maze.GetTileValueAtPixel(X, Y) != 20)
        {
          _stateCountDown.Expire();
        }
      }
      else
      {
        base.Update(elapsedTime);
      }
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
    public int StartTileX { get; set; }
    public int StartTileY { get; set; }
  }

  enum EnemyState
  {  
    HeadStart,
    Chase,
    Scatter,
    Spin,
  }
}
