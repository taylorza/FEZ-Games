// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZMan.Messages
{
  class WaitForPlayerMessage
  {
    private static readonly WaitForPlayerMessage _instance = new WaitForPlayerMessage();

    public static WaitForPlayerMessage Message()
    {
      return _instance;
    }

    private WaitForPlayerMessage() { }
  }
}
