using System;
using UnityEngine;

namespace CommNetAntennasInfo
{
    public static class Logging
    {
        private static string PREFIX = "<color=green>CommNetAntennasInfo:</color> ";

        public static void Log<T>(T msg)
        {
            Debug.Log(PREFIX + DateTime.Now.ToString("hh:mm:ss.f ") + msg.ToString());
        }

        public static void Log(string msg, params object[] arg)
        {
            Debug.Log(PREFIX + DateTime.Now.ToString("hh:mm:ss.f ") + String.Format(msg, arg));
        }
    }
}
