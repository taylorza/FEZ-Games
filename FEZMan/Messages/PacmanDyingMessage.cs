// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZMan.Messages
{
  class PacmanDyingMessage
  {
    private static readonly PacmanDyingMessage _instance = new PacmanDyingMessage();

    public static PacmanDyingMessage Message()
    {
      return _instance;
    }

    private PacmanDyingMessage() { }
  }
}
