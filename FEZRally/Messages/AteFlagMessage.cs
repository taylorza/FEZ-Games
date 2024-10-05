// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZRally.Messages
{
  class AteFlagMessage
  {
    private static readonly AteFlagMessage _instance = new AteFlagMessage();

    public int TileX { get; private set; }
    public int TileY { get; private set; }
    public int Value { get; private set; }
    public int Count { get; private set; }

    public static AteFlagMessage Message(int tileX, int tileY, int value, int count)
    {
      _instance.TileX = tileX;
      _instance.TileY = tileY;
      _instance.Value = value;
      _instance.Count = count;
      return _instance;
    }

    private AteFlagMessage() { }
  }
}
