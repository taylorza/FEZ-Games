// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZMan.Messages
{
  class PacmanDeadMessage
  {
    private static readonly PacmanDeadMessage _instance = new PacmanDeadMessage();

    public static PacmanDeadMessage Message()
    {
      return _instance;
    }

    private PacmanDeadMessage() { }
  }
}
