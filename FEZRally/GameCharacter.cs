using System;
using TinyCLR.Game;

namespace FEZRally
{
  abstract class GameCharacter : Sprite
  {
    private Direction _currentDirection = Direction.Left;
    private int _speed;
    private readonly int[] _directionCounter = new int[4];
    
    public GameCharacter(Maze maze)
    {
      Maze = maze;
      TargetSpeed = _speed = 4;
      Active = true;
      Lives = 3;
      State = CharacterState.Alive;
    
      MessageService.Instance.Subscribe(typeof(Messages.ResetLevelMessage), HandleResetLevelMessage);
    }

    private void HandleResetLevelMessage(object state)
    {
      for (int i = 0; i < 4; i++)
      {
        _directionCounter[i] = Utility.GetRandomNumber(100);
      }

      Reset();
    }

    protected Maze Maze { get; set; }
    protected int TargetSpeed { get; set; }
    protected bool PlayerCharacter = false;

    public int Score { get; set; }
    public int Lives { get; set; }
    public bool Active { get; set; }
    protected CharacterState State { get; set; }

    public Direction TargetDirection
    {
      get;
      set;
    }

    public Direction CurrentDirection
    {
      get { return _currentDirection; }
      set
      {
        if (_currentDirection != value)
        {
          _currentDirection = value;
          TargetDirection = value;
          SetCharacterAnimation();
        }
      }
    }

    public abstract void Reset();
    protected abstract Direction OnSelectNewDirection(int xCell, int yCell);
    protected abstract void SetCharacterAnimation();

    public virtual bool IsCellOpen(int tileId)
    {
      return tileId >= Tile.Empty;
    }

    public override void Update(float elapsedTime)
    {      
      if (!Active) return;

      if (State == CharacterState.Alive)
      {
        if ((X & TileViewer.TileModMask) == 0 && (Y & TileViewer.TileModMask) == 0)
        {
          int xCell = X >> TileViewer.TileSizeShift;
          int yCell = Y >> TileViewer.TileSizeShift;

          if (TargetDirection == Direction.Stop)
          {
            CurrentDirection = Direction.Stop;
          }
          else if (TargetDirection != _currentDirection && Maze.IsCellOpen(this, TargetDirection))
          {
            CurrentDirection = TargetDirection;
          }
          else if (!Maze.IsCellOpen(this, CurrentDirection) || !PlayerCharacter)
          {
            CurrentDirection = OnSelectNewDirection(xCell, yCell);
            if (CurrentDirection != Direction.Stop)
            {
              _directionCounter[(int)CurrentDirection]++;
            }
          }          
        }

        if (TargetSpeed != _speed)
        {
          int modMask = TargetSpeed - 1;
          if ((X & modMask) == 0 && (Y & modMask) == 0)
          {
            _speed = TargetSpeed;
          }
        }
        Move();
      }

      base.Update(elapsedTime);      
    }

    protected Direction PickLeastTravelledDirection(Direction excludeDirection)
    {
      Direction[] open;
      open = Maze.GetOpenDirections(this, excludeDirection);

      int openLength = open.Length;
      
      Direction minDirection = open[0];
      int minTravelCount = _directionCounter[(int)minDirection];

      for (int i = 0; i < openLength; ++i)
      {
        Direction direction = open[i];

        if (direction == Direction.Stop)
        {
          break;
        }
        
        int travelCount = _directionCounter[(int)direction];
        if (travelCount < minTravelCount)
        {
          minTravelCount = travelCount;
          minDirection = direction;
        }
      }

      return minDirection;
    }

    protected Direction PickRandomDirection(Direction excludeDirection)
    {
      Direction[] open;
      open = Maze.GetOpenDirections(this, excludeDirection);
      return PickRandomDirection(open);
    }

    private Direction PickRandomDirection(Direction[] open)
    {
      int count = 0;
      int openLength = open.Length;
      for (int i = 0; i < openLength; ++i)
      {
        if (open[i] == Direction.Stop) break;
        count++;
      }
      return open[Utility.GetRandomNumber(count)];
    }

    protected Direction HuntTarget(int tx, int ty)
    {
      Direction[] open = Maze.GetOpenDirections(this, CurrentDirection);
      Direction minDirection = open[0];
      int minDistance = int.MaxValue;
      int openLength = open.Length;
      for (int i = 0; i < openLength; ++i)
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

    protected int DistanceToTarget(int tx, int ty)
    {
      int dx = tx - X;
      int dy = ty - Y;
      return (dx * dx) + (dy * dy);
    }

    protected int DistanceToTarget(int tx, int ty, Direction direction)
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

    private void Move()
    {
      int dx = 0;
      int dy = 0;
      switch (_currentDirection)
      {
        case Direction.Left: dx = -_speed; break;
        case Direction.Right: dx = _speed; break;
        case Direction.Up: dy = -_speed; break;
        case Direction.Down: dy = _speed; break;
      }
      
      Move(dx, dy);
    }

    public override void Draw()
    {
      base.Draw();
    }

    protected Direction GetOppositeDirection()
    {
      return GetOppositeDirection(CurrentDirection);
    }

    protected Direction GetOppositeDirection(Direction direction)
    {
      if (direction == Direction.Left) return Direction.Right;
      if (direction == Direction.Right) return Direction.Left;
      if (direction == Direction.Up) return Direction.Down;
      if (direction == Direction.Down) return Direction.Up;
      return Direction.Stop;
    }

    public static Direction[] Directions = new Direction[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
  }

  enum CharacterState
  {
    Alive,
    Dying,
    Dead
  }

  enum Direction
  {
    Left = 0,
    Right = 1,
    Up = 2,
    Down = 3,

    Stop = 255,
  }
}
