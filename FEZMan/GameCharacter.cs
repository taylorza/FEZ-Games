// Copyright (c) 2012 Chris Taylor


using TinyCLR.Game;


namespace FEZMan
{
  abstract class GameCharacter : Sprite
  {
    private Direction _currentDirection = Direction.Left;
    private int _speed = 4;

    public GameCharacter(Maze maze)
    {
      Maze = maze;
      TargetSpeed = 4;
      Active = true;
      Lives = 3;
    }

    protected Maze Maze { get; set; }
    protected int TargetSpeed { get; set; }

    public int Score { get; set; }
    public int Lives { get; set; }
    public bool Active { get; set; }

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
    public abstract bool CanEnterCell(byte value);

    public override void Update(float elapsedTime)
    {
      if (!Active) return;

      if ((X & 7) == 0 && (Y & 7) == 0)
      {
        int xCell = X >> 3;
        int yCell = Y >> 3;

        TargetDirection = OnSelectNewDirection(xCell, yCell);

        if (TargetDirection == Direction.Stop)
        {
          CurrentDirection = Direction.Stop;
        }
        else if (TargetDirection != _currentDirection && Maze.IsCellOpen(this, TargetDirection))
        {
          CurrentDirection = TargetDirection;
        }
        else if (!Maze.IsCellOpen(this, CurrentDirection))
        {
          CurrentDirection = Direction.Stop;
        }
      }

      if (TargetSpeed != _speed)
      {
        int modMask = TargetSpeed;
        if ((X % modMask) == 0 && (Y % modMask) == 0)
        {
          _speed = TargetSpeed;
        }
      }
      Move();

      base.Update(elapsedTime);
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
      if (CurrentDirection == Direction.Left) return Direction.Right;
      if (CurrentDirection == Direction.Right) return Direction.Left;
      if (CurrentDirection == Direction.Up) return Direction.Down;
      if (CurrentDirection == Direction.Down) return Direction.Up;
      return Direction.Stop;
    }

    public static Direction[] Directions = new Direction[] { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
  }

  enum Direction
  {
    Stop = 0,

    Left = 1,
    Right = 2,
    Up = 5,
    Down = 6,
  }
}
