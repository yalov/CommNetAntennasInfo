using System;
using System.Globalization;
using UnityEngine;

namespace CommNetAntennasInfo
{
    /// <summary>
    /// Class Logging
    /// v2
    /// update prefix, and use by
    /// using static Logging;
    /// </summary>
    public static class Logging
    {
        private const string PREFIX = "<color=green>[CNAInfo]</color> ";
        private const bool time = false;

        public static void Log(params object[] args)
        {
            Debug.Log(PREFIX + (time ? DateTime.Now.ToString("HH:mm:ss.f ", CultureInfo.InvariantCulture) : "") +
                String.Join(", ", args)
                );
        }

        public static void LogFormat(string msg, params object[] args)
        {
            Debug.LogFormat(PREFIX + (time ? DateTime.Now.ToString("HH:mm:ss.f ", CultureInfo.InvariantCulture) : "") +
                msg, args);
        }
    }
}
