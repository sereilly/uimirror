// A simple logger class that uses Console.WriteLine by default.
// Can also do Logger.LogMethod = Debug.Log for Unity etc.
// (this way we don't have to depend on UnityEngine.DLL and don't need a
//  different version for every UnityEngine version here)
#if NETFX_CORE
using System.Diagnostics;
#else
using System;
#endif

namespace Telepathy
{
    public static class Logger
    {
#if NETFX_CORE  
        public delegate void Action<in T>(T obj);
        public static Action<string> Log = s => { Debug.WriteLine(s); };
        public static Action<string> LogWarning = s => { Debug.WriteLine(s); };
        public static Action<string> LogError= s => { Debug.WriteLine(s); };
#else
        public static Action<string> Log = Console.WriteLine;
        public static Action<string> LogWarning = Console.WriteLine;
        public static Action<string> LogError = Console.Error.WriteLine;
#endif
    }
}