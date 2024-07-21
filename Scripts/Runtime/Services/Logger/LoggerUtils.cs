namespace GameCore.Services.Logger
{
    using System.Diagnostics;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    public static class LoggerUtils
    {
        private static string ToHtmlStringRbg(Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
        }

#region Log

        public static void Log(object obj)
        {
            InternalLog(obj);
        }

        [Conditional("UNITY_EDITOR"), Conditional("ENABLE_LOG")]
        private static void InternalLog(object obj)
        {
            Debug.Log(obj);
        }

        public static void Log(object obj, Color color)
        {
            InternalLog(obj, color);
        }

        [Conditional("UNITY_EDITOR"), Conditional("ENABLE_LOG"), Conditional("DEVELOPMENT_BUILD")]
        private static void InternalLog(object obj, Color color)
        {
            Debug.Log($"{obj}{ToHtmlStringRbg(color)}");
        }

#endregion

#region Warning

        public static void Warning(object obj)
        {
            InternalWarning(obj);
        }

        [Conditional("UNITY_EDITOR"), Conditional("ENABLE_LOG"), Conditional("DEVELOPMENT_BUILD")]
        private static void InternalWarning(object obj)
        {
            Debug.LogWarning(obj);
        }

#endregion

#region Error

        public static void Error(object obj)
        {
            InternalError(obj);
        }

        [Conditional("UNITY_EDITOR"), Conditional("ENABLE_LOG"), Conditional("DEVELOPMENT_BUILD")]
        private static void InternalError(object obj)
        {
            Debug.LogError(obj);
        }

#endregion
    }
}