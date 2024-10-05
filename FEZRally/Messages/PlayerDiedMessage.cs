// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZRally.Messages
{
  class PlayerDiedMessage
  {
    private static readonly PlayerDiedMessage _instance = new PlayerDiedMessage();
    
    public int RemainingLives { get; private set; }
    public static PlayerDiedMessage Message(int remainingLives)
    {
      _instance.RemainingLives = remainingLives;
      return _instance;
    }

    private PlayerDiedMessage() { }
  }
}
