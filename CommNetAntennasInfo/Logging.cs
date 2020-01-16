using System;
using System.Globalization;
using UnityEngine;

namespace CommNetAntennasInfo
{
    public static class Logging
    {
        private const string PREFIX = "<color=green>[CNAInfo]</color> ";
        private const bool time = false;

        public static void Log<T>(T msg, params object[] args)
        {
            if (msg == null) { Log("null " + args); return; }

            Debug.Log(PREFIX +
                (time ? DateTime.Now.ToString("HH:mm:ss.f ", CultureInfo.InvariantCulture) : "") +
                String.Format(CultureInfo.InvariantCulture, msg.ToString(), args)
                );
        }
    }
}
