// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZMan.Messages
{
  class AtePillMessage
  {
    private static readonly AtePillMessage _instance = new AtePillMessage();

    public static AtePillMessage Message()
    {
      return _instance;
    }

    private AtePillMessage() { }
  }
}
