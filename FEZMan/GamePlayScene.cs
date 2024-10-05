using System;
using System.Drawing;
using TinyCLR.Game;
using TinyCLR.Game.Input;

namespace FEZMan
{
    class GamePlayScene : GameScene
    {
        private const long ReadyDefaultCountDown = 2000;
        private const long BonusDefaultCountDown = 10000;

        private Font _font;
        private Bitmap _spriteSheet;
        private Player _pacman;
        private Ghost _blinky;
        private Ghost _pinky;
        private Ghost _inky;
        private Ghost _clyde;
        private Maze _maze;
        private readonly CountDownTimer _readyCountDown = new CountDownTimer(ReadyDefaultCountDown);
        private readonly CountDownTimer _showBonusCountDown = new CountDownTimer(BonusDefaultCountDown);
        private readonly CountDownTimer _showBonusScoreCountDown = new CountDownTimer(1000);

        private Sprite _bonus200;
        private Sprite _bonus400;
        private Sprite _bonus800;
        private Sprite _bonus1600;

        private Sprite _bonus100;
        private Sprite _bonus300;
        private Sprite _bonus500;
        private Sprite _bonus700;
        private Sprite _bonus1000;
        private Sprite _bonus2000;
        private Sprite _bonus3000;
        private Sprite _bonus5000;
        private Sprite _currentBonusSprite;

        private bool _gameOver;
        private bool _loaded = false;

        public int Level { get; private set; }

        public GamePlayScene()
        {
            Level = 1;
        }

        public override void LoadContent()
        {
            if (!_loaded)
            {
                _loaded = true;

                _font = Resources.GetFont(Resources.FontResources.portfolio);

                _spriteSheet = Resources.GetBitmap(Resources.BitmapResources.pacman);

                _maze = new Maze();
                _maze.Draw();

                _pacman = new Player(_spriteSheet, _maze);

                #region Initialize Bonus Point Sprites
                _bonus200 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(80, 104, 16, 8) }));

                _bonus400 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(96, 104, 16, 8) }));

                _bonus800 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(112, 104, 16, 8) }));

                _bonus1600 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(128, 104, 16, 8) }));


                _bonus100 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(0, 112, 16, 8) }));

                _bonus300 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(16, 112, 16, 8) }));

                _bonus500 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(32, 112, 16, 8) }));

                _bonus700 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(48, 112, 16, 8) }));

                _bonus1000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(64, 112, 20, 8) }));

                _bonus2000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(84, 112, 20, 8) }));

                _bonus3000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(106, 112, 20, 8) }));

                _bonus5000 = new Sprite(new AnimationSequence(_spriteSheet, 0,
                  new Rect[] { new Rect(128, 112, 16, 8) }));
                #endregion

                #region Initialize Ghosts
                _blinky = new Ghost(_spriteSheet, _maze, _pacman, 8,
                  new Personality()
                  {
                      ScatterX = 13 * 8,
                      ScatterY = -2 * 8,
                      TargetOffsetX = 0,
                      TargetOffsetY = 0,
                      HouseX = 7 * 8,
                      HouseY = 7 * 8,
                      StartX = 7 * 8,
                      StartY = 5 * 8
                  })
                {
                    CurrentDirection = Direction.Left
                };

                _pinky = new Ghost(_spriteSheet, _maze, _pacman, 16,
                  new Personality()
                  {
                      ScatterX = 1 * 8,
                      ScatterY = -2 * 8,
                      TargetOffsetX = 4 * 8,
                      TargetOffsetY = 0,
                      HouseX = 6 * 8,
                      HouseY = 7 * 8,
                      StartX = 6 * 8,
                      StartY = 7 * 8
                  })
                {
                    CurrentDirection = Direction.Left
                };

                _inky = new Ghost(_spriteSheet, _maze, _pacman, 24,
                  new Personality()
                  {
                      ScatterX = 14 * 8,
                      ScatterY = 15 * 8,
                      TargetOffsetX = -3 * 8,
                      TargetOffsetY = 0,
                      HouseX = 7 * 8,
                      HouseY = 7 * 8,
                      StartX = 7 * 8,
                      StartY = 7 * 8
                  })
                {
                    CurrentDirection = Direction.Left
                };

                _clyde = new Ghost(_spriteSheet, _maze, _pacman, 32,
                  new Personality()
                  {
                      ScatterX = 0 * 8,
                      ScatterY = 15 * 8,
                      TargetOffsetX = 0,
                      TargetOffsetY = -2 * 8,
                      HouseX = 8 * 8,
                      HouseY = 7 * 8,
                      StartX = 8 * 8,
                      StartY = 7 * 8
                  })
                {
                    CurrentDirection = Direction.Left
                };
                #endregion

                _pacman.Enemies = new Ghost[]
                {
          _blinky, _pinky, _inky, _clyde
                };

                #region Add objects to scene
                //AddToScene(_showBonusCountDown);
                //AddToScene(_showBonusScoreCountDown);
                AddToScene(_maze);
                AddToScene(_pacman);
                AddToScene(_blinky);
                AddToScene(_pinky);
                AddToScene(_inky);
                AddToScene(_clyde);
                #endregion

                _maze.LevelComplete += MazeLevelComplete;
                _showBonusCountDown.Expired += BonusCountDown_Expired;

                MessageService.Instance.Subscribe(typeof(Messages.PacmanDeadMessage), HandlePacmanDeadMessage);
                MessageService.Instance.Subscribe(typeof(Messages.PacmanAteGhostMessage), HandlePacmanAteGhostMessage);
                MessageService.Instance.Subscribe(typeof(Messages.AteBonusItemMessage), HandleAteBonusItem);

                base.LoadContent();

                Reset();
            }
        }

        #region Message subscription handlers and events

        private void HandlePacmanDeadMessage(object message)
        {
            if (_pacman.Lives == 0)
            {
                _gameOver = true;
                MessageService.Instance.Publish(Messages.WaitForPlayerMessage.Message());
            }
            else if (!_gameOver)
            {
                Reset();
            }
        }

        private void HandlePacmanAteGhostMessage(object message)
        {
            Messages.PacmanAteGhostMessage m = (Messages.PacmanAteGhostMessage)message;
            switch (m.Value)
            {
                case 200: _currentBonusSprite = _bonus200; break;
                case 400: _currentBonusSprite = _bonus400; break;
                case 800: _currentBonusSprite = _bonus800; break;
                case 1600: _currentBonusSprite = _bonus1600; break;
            }

            _currentBonusSprite.MoveTo(m.X, m.Y);
            _showBonusScoreCountDown.Start();
        }

        private void HandleAteBonusItem(object message)
        {
            Messages.AteBonusItemMessage m = (Messages.AteBonusItemMessage)message;

            switch (m.Value)
            {
                case 100: _currentBonusSprite = _bonus100; break;
                case 300: _currentBonusSprite = _bonus300; break;
                case 500: _currentBonusSprite = _bonus500; break;
                case 700: _currentBonusSprite = _bonus700; break;
                case 1000: _currentBonusSprite = _bonus1000; break;
                case 2000: _currentBonusSprite = _bonus2000; break;
                case 3000: _currentBonusSprite = _bonus3000; break;
                case 5000: _currentBonusSprite = _bonus5000; break;
            }

            _currentBonusSprite.MoveTo(m.X, m.Y);

            _showBonusCountDown.Expire();
            _showBonusScoreCountDown.Start();
        }

        private void MazeLevelComplete(object sender, EventArgs e)
        {
            Reset();
            PrepareLevel(Level + 1);
        }

        private void BonusCountDown_Expired(object sender, EventArgs e)
        {
            _maze.BonusItem = BonusItemType.None;
        }
        #endregion

        private void Reset()
        {
            if (_gameOver)
            {
                _gameOver = false;
                _pacman.Score = 0;
                _pacman.Lives = 3;

                PrepareLevel(1);
                MessageService.Instance.Publish(Messages.StartGameMessage.Message());
            }

            _showBonusCountDown.Cancel();
            _showBonusScoreCountDown.Cancel();
            _readyCountDown.Start();
            _maze.BonusItem = BonusItemType.None;
        }

        private void PrepareLevel(int newLevel)
        {
            Level = newLevel;

            _maze.Reset();
            _pacman.Reset();

            _blinky.Level = Level;
            _pinky.Level = Level;
            _inky.Level = Level;
            _clyde.Level = Level;
            _blinky.Reset();
            _pinky.Reset();
            _inky.Reset();
            _clyde.Reset();
        }

        public override void Update(float elapsedTime)
        {
            if (GameManager.Game.InputManager.Button1)
            {
                if (_gameOver)
                {
                    Reset();
                }
            }

            if (!_readyCountDown.IsRunning)
            {
                if (_maze.DotsEaten + _maze.BonusEaten == 30 || _maze.DotsEaten + _maze.BonusEaten == 80)
                {
                    if (Level == 1)
                    {
                        _maze.BonusItem = BonusItemType.Cherry;
                    }
                    else if (Level == 2)
                    {
                        _maze.BonusItem = BonusItemType.Strawberry;
                    }
                    else if (Level >= 3 && Level <= 4)
                    {
                        _maze.BonusItem = BonusItemType.Peach;
                    }
                    else if (Level >= 5 && Level <= 6)
                    {
                        _maze.BonusItem = BonusItemType.Apple;
                    }
                    else if (Level >= 7 && Level <= 8)
                    {
                        _maze.BonusItem = BonusItemType.Grape;
                    }
                    else if (Level >= 9 && Level <= 10)
                    {
                        _maze.BonusItem = BonusItemType.Galaxian;
                    }
                    else if (Level >= 11 && Level <= 12)
                    {
                        _maze.BonusItem = BonusItemType.Bell;
                    }
                    else if (Level >= 13)
                    {
                        _maze.BonusItem = BonusItemType.Key;
                    }

                    _showBonusCountDown.Start();
                }
                base.Update(elapsedTime);
            }
        }

        public override void Draw()
        {
            base.Draw();

            UpdateHud(Surface, _pacman.Score);

            if (_gameOver)
            {
                // Show 'Game Over'
                Surface.DrawImage(
                  (120 - 36) / 2,
                  ((120 - 4) / 2) + 8,
                  _spriteSheet, 0, 52, 36, 4, 255);
            }
            else if (_readyCountDown.IsRunning)
            {
                // Show 'Ready!' Image
                Surface.DrawImage(
                  (120 - 24) / 2,
                  ((120 - 4) / 2) + 8,
                  _spriteSheet, 40, 48, 24, 4, 255);
            }
            else if (_showBonusScoreCountDown.IsRunning)
            {
                _currentBonusSprite.Draw();
            }
        }

        private static readonly Brush brush = new SolidBrush(Color.White);
        private void UpdateHud(Graphics surface, int score)
        {
            int y = 5;
            int x = 122;
            
            surface.DrawString("Level", _font, brush, x, y);
            surface.DrawString(Level.ToString(), _font, brush, x, y + 10);

            y += 45;
            surface.DrawString("Score", _font, brush, x, y);
            surface.DrawString(score.ToString(), _font, brush, x, y + 10);

            y += 45;
            surface.DrawString("Lives", _font, brush, x, y);

            int lives = _pacman.Lives;
            for (int i = 0; i < lives; ++i)
            {
                surface.DrawImage(x + (i << 3), y + 10, _spriteSheet, 16, 0, 8, 8, 255);
            }
        }
    }
}
