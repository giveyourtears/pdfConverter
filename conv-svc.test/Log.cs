using System;
using System.Collections.Generic;
using System.Text;

namespace Inex.Umk.Conv.Test
{
  static class Log
  {
    public static void Info(string msg, params object[] arg)
    {
      Write(false, msg, arg);
    }

    public static void Error(string msg, params object[] arg)
    {
      Write(true, msg, arg);
    }

    static void Write(bool isError, string msg, params object[] arg)
    {
      ConsoleColor orig = Console.ForegroundColor;
      try
      {
        if(isError)
          Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg, arg);

      }
      finally
      {
        Console.ForegroundColor = orig;
      }
    }
  }
}
