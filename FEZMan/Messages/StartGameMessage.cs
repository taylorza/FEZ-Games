// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZMan.Messages
{
  class StartGameMessage
  {
    private static readonly StartGameMessage _instance = new StartGameMessage();

    public static StartGameMessage Message()
    {
      return _instance;
    }

    private StartGameMessage() { }
  }
}
