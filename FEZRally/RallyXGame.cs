using System;
using System.Drawing;
using TinyCLR.Game;

namespace FEZRally
{
  class RallyXGame : GameManager
  {
    GamePlayScene _gamePlayScene = new GamePlayScene();

    public static RallyXGame Instance { get; private set; }

    public RallyXGame(Graphics surface)
      : base(surface)
    {
      TargetFrameRate = 30;
      FixedFrameRate = true;      
      Instance = this;
    }

    public override void LoadContent()
    {
      AddScene(_gamePlayScene);
      ShowScene(_gamePlayScene);

      base.LoadContent();
    }    
  }
}
