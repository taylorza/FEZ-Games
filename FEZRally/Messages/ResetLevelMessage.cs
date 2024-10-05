// Copyright (c) 2012 Chris Taylor

using System;

namespace FEZRally.Messages
{
  class ResetLevelMessage
  {
    private static readonly ResetLevelMessage _instance = new ResetLevelMessage();

    public static ResetLevelMessage Message()
    {
      return _instance;
    }

    private ResetLevelMessage() { }
  }
}
