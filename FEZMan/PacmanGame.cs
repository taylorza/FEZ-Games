// Copyright (c) 2012 Chris Taylor


using System.Drawing;
using TinyCLR.Game;

namespace FEZMan
{
  class PacmanGame : GameManager
  {
    GameScene _gamePlayScene;

    public PacmanGame(Graphics surface)
      : base(surface)
    {
      TargetFrameRate = 12;
      FixedFrameRate = true;
    }

    public override void LoadContent()
    {
      _gamePlayScene = new GamePlayScene();
      AddScene(_gamePlayScene);
      ShowScene(_gamePlayScene);
    }
  }
}
