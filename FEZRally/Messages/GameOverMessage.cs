// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZRally.Messages
{
  class GameOverMessage
  {
    private static readonly GameOverMessage _instance = new GameOverMessage();

    public static GameOverMessage Message()
    {
      return _instance;
    }

    private GameOverMessage() { }
  }
}
