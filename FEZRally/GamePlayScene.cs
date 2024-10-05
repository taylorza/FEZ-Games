using System;
using System.Drawing;
using TinyCLR.Game;

namespace FEZRally
{
    class GamePlayScene : GameScene
    {
        private bool _loaded = false;
        private Maze _maze;
        private Font _font;
        private readonly Bitmap _tileSet = Resources.GetBitmap(Resources.BitmapResources.Rally_X_2);

        private Sprite _gameOver;
        private Sprite _score100;
        private Sprite _score200;
        private Sprite _score300;
        private Sprite _score400;
        private Sprite _score500;
        private Sprite _score600;
        private Sprite _score700;
        private Sprite _score800;
        private Sprite _score900;
        private Sprite _score1000;
        private Sprite _lastScoreSprite;

        Player _player;
        Enemy[] _enemies;
        GameState _state;

        private int _highScore = 0;

        readonly CountDownTimer _lastScoreTimer = new CountDownTimer(5000);
        readonly CountDownTimer _gameStateTimer = new CountDownTimer(500);

        public override void LoadContent()
        {
            if (!_loaded)
            {
                _loaded = true;

                _font = Resources.GetFont(Resources.FontResources.portfolio);

                var c = _tileSet.GetPixel(0, 0);
                _tileSet.MakeTransparent(c);
                _maze = new Maze(_tileSet, 13, 14);
                _maze.GenerateLevel();
                _maze.MoveTo(8, 12);

                _player = new Player(_tileSet, _maze, 20, 50);

                _enemies = new Enemy[3];

                _enemies[0] = new Enemy(_tileSet, _maze, _player,
                  new Personality()
                  {
                      ScatterX = 5 << TileViewer.TileSizeShift,
                      ScatterY = 0 << TileViewer.TileSizeShift,
                      StartTileX = 18,
                      StartTileY = 58,
                      TargetOffsetX = 0,
                      TargetOffsetY = 0
                  });

                _enemies[1] = new Enemy(_tileSet, _maze, _player,
                  new Personality()
                  {
                      ScatterX = 40 << TileViewer.TileSizeShift,
                      ScatterY = 0 << TileViewer.TileSizeShift,
                      StartTileX = 20,
                      StartTileY = 58,
                      TargetOffsetX = 32,
                      TargetOffsetY = 32
                  });

                _enemies[2] = new Enemy(_tileSet, _maze, _player,
                  new Personality()
                  {
                      ScatterX = 40 << TileViewer.TileSizeShift,
                      ScatterY = 64 << TileViewer.TileSizeShift,
                      StartTileX = 22,
                      StartTileY = 58,
                      TargetOffsetX = -32,
                      TargetOffsetY = -32
                  });

                _gameOver = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(17, 32, 41, 5) }));
                _score100 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(1, 50, 10, 4) }));
                _score200 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(12, 50, 11, 4) }));
                _score300 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(24, 50, 11, 4) }));
                _score400 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(36, 50, 11, 4) }));
                _score500 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(48, 50, 11, 4) }));
                _score600 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(0, 58, 11, 4) }));
                _score700 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(13, 58, 10, 4) }));
                _score800 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(24, 58, 1, 4) }));
                _score900 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(36, 58, 11, 4) }));
                _score1000 = new Sprite(new AnimationSequence(_tileSet, 0, new Rect[] { new Rect(49, 58, 14, 4) }));

                _gameOver.MoveTo(_maze.X + (6 * TileViewer.TileSize - 17), _maze.Y + (6 * TileViewer.TileSize - 4));
                AddToScene(_maze);

                _maze.TrackObject(_player);

                _maze.AddPlayer(_player);
                _maze.AddEnemy(_enemies[0]);
                _maze.AddEnemy(_enemies[1]);
                _maze.AddEnemy(_enemies[2]);

                _state = GameState.Playing;
            }

            _lastScoreTimer.Expired += new EventHandler(LastScoreTimer_Expired);
            _gameStateTimer.Expired += new EventHandler(GameStateTimer_Expired);

            MessageService.Instance.Subscribe(typeof(Messages.AteFlagMessage), HandleAteFlagMessage);
            MessageService.Instance.Subscribe(typeof(Messages.PlayerDiedMessage), HandlePlayerDiedMessage);
            base.LoadContent();
        }

        void GameStateTimer_Expired(object sender, EventArgs e)
        {
            if (_state == GameState.Dying)
            {
                _state = GameState.Playing;
                if (_player.Lives == 0)
                {
                    _state = GameState.GameOver;
                }
                _player.Enabled = true;
                for (int i = 0; i < _enemies.Length; i++)
                {
                    _enemies[i].Enabled = true;
                }

                MessageService.Instance.Publish(Messages.ResetLevelMessage.Message());
            }
        }

        void LastScoreTimer_Expired(object sender, EventArgs e)
        {
            if (_lastScoreSprite != null)
            {
                _lastScoreSprite.Owner = null;
            }
        }

        private void HandlePlayerDiedMessage(object message)
        {
            _player.Enabled = false;
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].Enabled = false;
            }
            _state = GameState.Dying;
            _gameStateTimer.Start(500);
        }

        private void HandleAteFlagMessage(object message)
        {
            var ateFlag = message as Messages.AteFlagMessage;
            if (_lastScoreSprite != null)
            {
                _lastScoreSprite.Owner = null;
            }

            switch (ateFlag.Value)
            {
                case 100: _lastScoreSprite = _score100; break;
                case 200: _lastScoreSprite = _score200; break;
                case 300: _lastScoreSprite = _score300; break;
                case 400: _lastScoreSprite = _score400; break;
                case 500: _lastScoreSprite = _score500; break;
                case 600: _lastScoreSprite = _score600; break;
                case 700: _lastScoreSprite = _score700; break;
                case 800: _lastScoreSprite = _score800; break;
                case 900: _lastScoreSprite = _score900; break;
                case 1000: _lastScoreSprite = _score1000; break;
            }

            if (_lastScoreSprite != null)
            {
                _lastScoreSprite.MoveTo(ateFlag.TileX << TileViewer.TileSizeShift, ateFlag.TileY << TileViewer.TileSizeShift);
                _lastScoreSprite.Owner = _maze;
                _lastScoreTimer.Start();
            }

            if (ateFlag.Count == 10)
            {
                Pause();
                _state = GameState.CountBonus;
            }
        }

        private void Pause()
        {
            _player.Enabled = false;
            foreach (var enemy in _enemies)
            {
                enemy.Active = false;
            }
        }

        private void Continue()
        {
            _player.Enabled = true;
            foreach (var enemy in _enemies)
            {
                enemy.Active = true;
            }
        }

        private void NewGame()
        {
            _player.NewGame();
            Reset();
        }

        private void NewLevel()
        {
            _maze.GenerateLevel();
            _player.NewLevel();
            Reset();
        }

        private void Reset()
        {
            _state = GameState.Playing;
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].Reset();
            }
        }

        public override void Update(float elapsedTime)
        {
            if (_state == GameState.GameOver)
            {
                if (GameManager.Game.InputManager.Button1)
                {
                    _maze.GenerateLevel();
                    NewGame();
                }
            }
            else if (_state == GameState.CountBonus)
            {
                if (_player.Fuel > 0)
                {
                    _player.Fuel--;
                    _player.Score += 100;
                }
                else
                {
                    Continue();
                    NewLevel();
                }
            }

            base.Update(elapsedTime);
        }

        public override void Draw()
        {
            Surface.Clear();
            base.Draw();
            UpdateHud();
            if (_state == GameState.GameOver)
            {
                _gameOver.Draw();
            }
        }

        private void UpdateHud()
        {
            Bitmap miniMap = _maze.GetMiniMap();
            
            int y = 8;
            int x = 160 - miniMap.Width;

            if (_player.Score > _highScore) _highScore = _player.Score;

            Surface.DrawString("HI-SCORE", _font, Brushes.White, 16, 2);
            Surface.DrawString(_highScore.ToString(), _font, Brushes.Red, 80, 2);

            Surface.DrawString("1UP", _font, Brushes.White, x, 0);
            Surface.DrawString(_player.Score.ToString(), _font, Brushes.Cyan, x, 8);

            Surface.DrawString("FUEL", _font, Brushes.Green, x + 12, y + 18);

            int hudWidth = miniMap.Width;
            
            Surface.DrawImage(x, 128 - 12 - miniMap.Height, miniMap, 0, 0, hudWidth, miniMap.Height, 255);
          
            int fuelWidth = (int)((_player.Fuel / 100.0) * hudWidth);
            var fuelColor = _player.Fuel < 35 ? Brushes.Red : Brushes.Yellow;
            Surface.FillRectangle(fuelColor, x, y + 15 + 15, fuelWidth, 4);

            int lives = _player.Lives;
            for (int i = 0; i < lives; ++i)
            {
                Surface.DrawImage(x + (i << 3), Surface.Height-9, _tileSet, 8, 32, 8, 8, 255);
            }            
        }
    }

    enum GameState
    {
        Playing,
        Dying,
        GameOver,
        CountBonus,
    }
}
