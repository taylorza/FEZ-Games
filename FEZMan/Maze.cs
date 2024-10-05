// Copyright (c) 2012 Chris Taylor

using System;
using System.Drawing;
using TinyCLR.Game;


namespace FEZMan
{
    class Maze : GameObject
    {
        private readonly Bitmap _mazeSpriteSheet;
        private readonly Graphics _cache;
        private readonly Bitmap _cacheBmp;
        private readonly Rect[] _walls;
        private int _dotCount;
        private int _dotsEaten;
        private BonusItemType _bonusItem = BonusItemType.None;
        private Rect _bonusItemRect;

        private readonly byte[][] _maze = new byte[][]
        {
            new byte[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            new byte[] {1,2,2,2,2,2,2,1,2,2,2,2,2,2,1},
            new byte[] {1,3,1,1,2,1,1,1,1,1,2,1,1,3,1},
            new byte[] {1,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
            new byte[] {1,1,1,1,2,1,1,1,1,1,2,1,1,1,1},
            new byte[] {1,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
            new byte[] {1,1,1,1,2,1,1,5,1,1,2,1,1,1,1},
            new byte[] {4,2,2,2,2,1,0,0,0,1,2,2,2,2,4},
            new byte[] {1,1,1,1,2,1,1,1,1,1,2,1,1,1,1},
            new byte[] {1,2,2,2,2,2,2,0,2,2,2,2,2,2,1},
            new byte[] {1,2,1,1,2,1,1,1,1,1,2,1,1,2,1},
            new byte[] {1,3,2,1,2,2,2,1,2,2,2,1,2,3,1},
            new byte[] {1,1,2,1,1,1,2,1,2,1,1,1,2,1,1},
            new byte[] {1,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
            new byte[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        };

        private readonly Rect[] _bonusItemRects = new Rect[]
        {
            new Rect(16,32,8,8), // Cherry
            new Rect(24,32,8, 8), // Strawberry
            new Rect(32,32,8, 8), // Peach
            new Rect(0,40,8, 8),  // Apple
            new Rect(8,40,8, 8), // Grape
            new Rect(16,40,8, 8), // Galaxian
            new Rect(24,40,8, 8), // Bell
            new Rect(32,40,8, 8), // Key
        };

        public int DotsEaten { get { return _dotsEaten; } }
        public int BonusEaten { get; private set; }

        public BonusItemType BonusItem
        {
            get { return _bonusItem; }
            set
            {
                _bonusItem = value;
                if (_bonusItem != BonusItemType.None)
                {
                    byte bonusItemIndex = (byte)_bonusItem;
                    _bonusItemRect = _bonusItemRects[bonusItemIndex];
                    _maze[9][7] = (byte)(MazeCell.Cherry + bonusItemIndex);
                }
                else
                {
                    _maze[9][7] = MazeCell.Clear;
                }
            }
        }

        public Maze()
        {
            _mazeSpriteSheet = Resources.GetBitmap(Resources.BitmapResources.maze_walls);

            // Wall image mapping based on neighbors using a binary scheme
            // In the image below * represents the current map location, and the surrounding numbers
            // represent the bit value that used if the corresponding location has wall piece. The sum
            // of the populated neighbors represents the index into the _walls array at which the 
            // image can be found that represents the piece that integrates with this neighbor configuration.
            //     |1|
            //   |8|*|2|
            //     |4|
            // For example, the top left corner in the map has one neighbor to the right an one below
            // which means that the top left corner should be an corner piece which is open on the 
            // right and bottom, the neighbor of the right and bottom has value 2 and 4 therefore 
            // the image that integrates with these neighbors is stored at index 6 (2+4) in the wall array.
            //
            // Note: It is not the actual image, but the rect location in the sprite sheet from which the image
            //       will be extracted.

            _walls = new Rect[16];
            _walls[0] = WallToRect(18);
            _walls[1] = WallToRect(19);
            _walls[2] = WallToRect(4);
            _walls[3] = WallToRect(10);
            _walls[4] = WallToRect(14);
            _walls[5] = WallToRect(5);
            _walls[6] = WallToRect(0);
            _walls[7] = WallToRect(11);
            _walls[8] = WallToRect(9);
            _walls[9] = WallToRect(13);
            _walls[10] = WallToRect(1);
            _walls[11] = WallToRect(12);
            _walls[12] = WallToRect(3);
            _walls[13] = WallToRect(8);
            _walls[14] = WallToRect(2);
            _walls[15] = WallToRect(7);

            _cacheBmp = new Bitmap(160, 128);
            _cache = Graphics.FromImage(_cacheBmp);
            Reset();
        }

        public void Reset()
        {
            BonusItem = BonusItemType.None;
            BonusEaten = 0;
            _dotCount = 0;
            _dotsEaten = 0;

            int mazeHeight = _maze.Length;
            int mazeWidth = _maze[0].Length;
            for (int y = 0; y < mazeHeight; ++y)
            {
                for (int x = 0; x < mazeWidth; ++x)
                {
                    byte cellValue = _maze[y][x];
                    if (cellValue >= 100)
                    {
                        _maze[y][x] -= 100;
                    }
                }
            }
            _maze[9][7] = 0;
            UpdateMazeCache();
        }

        private Rect WallToRect(int index)
        {
            int x = index % 5;
            int y = index / 5;
            return new Rect(x * 8, y * 8, 8, 8);
        }

        private int GetNeighbors(byte[][] map, int x, int y, byte value)
        {
            int result = 0;
            if (x > 0 && map[y][x - 1] == value) result |= 8;
            if (x < map[y].Length - 1 && map[y][x + 1] == value) result |= 2;
            if (y > 0 && map[y - 1][x] == value) result |= 1;
            if (y < map.Length - 1 && map[y + 1][x] == value) result |= 4;
            return result;
        }

        public override void Draw()
        {
            Surface.DrawImage(0, 0, _cacheBmp, 0, 0, 160, 128, 255);
            if (_bonusItem != BonusItemType.None)
            {
                Surface.DrawImage(56, 72, _mazeSpriteSheet,
                  _bonusItemRect.X, _bonusItemRect.Y,
                  _bonusItemRect.Width, _bonusItemRect.Height, 255);
            }
        }

        private void UpdateMazeCache()
        {
            int mazeHeight = _maze.Length;
            int mazeWidth = _maze[0].Length;
            for (int y = 0; y < mazeHeight; ++y)
            {
                for (int x = 0; x < mazeWidth; ++x)
                {
                    int xCell = x << 3;
                    int yCell = y << 3;

                    byte cellValue = _maze[y][x];

                    if (cellValue == MazeCell.Wall)
                    {
                        int index = GetNeighbors(_maze, x, y, MazeCell.Wall);
                        Rect wall = _walls[index];
                        _cache.DrawImage(xCell, yCell, _mazeSpriteSheet, wall.X, wall.Y, 8, 8, 255);
                    }
                    else if (cellValue == MazeCell.Dot)
                    {
                        _cache.DrawImage(xCell, yCell, _mazeSpriteSheet, 0, 24, 8, 8, 255);
                        _dotCount++;
                    }
                    else if (cellValue == MazeCell.Enegizer)
                    {
                        _cache.DrawImage(xCell, yCell, _mazeSpriteSheet, 8, 24, 8, 8, 255);
                        _dotCount++;
                    }
                }
            }

            // Draw the House gate
            _cache.DrawImage(48 + 6, 48, _mazeSpriteSheet, 0, 16, 6, 4, 255);
        }

        private readonly Direction[] _openDirections = new Direction[4];
        public Direction[] GetOpenDirections(GameCharacter character, Direction current)
        {
            int i = 0;

            _openDirections[0] = Direction.Stop;
            _openDirections[1] = Direction.Stop;
            _openDirections[2] = Direction.Stop;
            _openDirections[3] = Direction.Stop;

            if (current != Direction.Right && IsCellOpen(character, Direction.Left)) _openDirections[i++] = Direction.Left;
            if (current != Direction.Left && IsCellOpen(character, Direction.Right)) _openDirections[i++] = Direction.Right;
            if (current != Direction.Down && IsCellOpen(character, Direction.Up)) _openDirections[i++] = Direction.Up;
            if (current != Direction.Up && IsCellOpen(character, Direction.Down)) _openDirections[i++] = Direction.Down;

            if (i == 0)
            {
                if (current == Direction.Right && IsCellOpen(character, Direction.Left)) _openDirections[i++] = Direction.Left;
                else if (current == Direction.Left && IsCellOpen(character, Direction.Right)) _openDirections[i++] = Direction.Right;
                else if (current == Direction.Down && IsCellOpen(character, Direction.Up)) _openDirections[i++] = Direction.Up;
                else if (current == Direction.Up && IsCellOpen(character, Direction.Down)) _openDirections[i++] = Direction.Down;
            }

            return _openDirections;
        }

        public bool IsCellOpen(GameCharacter character, Direction direction)
        {
            int x = character.X >> 3;
            int y = character.Y >> 3;
            switch (direction)
            {
                case Direction.Left: return character.CanEnterCell(_maze[y][x - 1]);
                case Direction.Right: return character.CanEnterCell(_maze[y][x + 1]);
                case Direction.Up: return character.CanEnterCell(_maze[y - 1][x]);
                case Direction.Down: return character.CanEnterCell(_maze[y + 1][x]);
            }

            return false;
        }

        public byte GetCellValue(int x, int y)
        {
            int xCell = (x + 4) >> 3;
            int yCell = (y + 4) >> 3;

            return _maze[yCell][xCell];
        }

        public bool IsColliding(int x1, int y1, int x2, int y2)
        {
            // Objects are colliding if centers are within
            // 6 pixels of each other. 
            // Eliminated sqrt for performance.
            int dx = (x1 + 4) - (x2 + 4);
            int dy = (y1 + 4) - (y2 + 4);

            return ((dx * dx) + (dy * dy)) < 36;
        }

        private static readonly Brush BlackBrush = new SolidBrush(Color.Black);
        public void ClearCell(int x, int y)
        {
            int xCell = (x + 4) >> 3;
            int yCell = (y + 4) >> 3;

            _cache.FillRectangle(BlackBrush, xCell << 3, yCell << 3, 8, 8);

            byte cellValue = _maze[yCell][xCell];
            switch (cellValue)
            {
                case MazeCell.Dot:
                case MazeCell.Enegizer:
                    _dotCount--;
                    _dotsEaten++;
                    break;
                case MazeCell.Cherry:
                case MazeCell.Strawberry:
                case MazeCell.Peach:
                case MazeCell.Apple:
                case MazeCell.Grape:
                case MazeCell.Galaxian:
                case MazeCell.Bell:
                case MazeCell.Key:
                    BonusEaten++;
                    break;
            }

            _maze[yCell][xCell] += 100;

            if (_dotCount == 0)
            {
                OnLevelComplete(EventArgs.Empty);
            }
        }

        protected virtual void OnLevelComplete(EventArgs e)
        {
            LevelComplete?.Invoke(this, e);
        }

        public event EventHandler LevelComplete;
    }
}
