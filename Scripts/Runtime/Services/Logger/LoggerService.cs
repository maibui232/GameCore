namespace GameCore.Services.Logger
{
    using System.Diagnostics;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    public interface ILoggerService
    {
        void Log(object obj);
        void Log(object obj, Color color);

        void Warning(object obj);

        void Error(object obj);
    }

    public class LoggerService : ILoggerService
    {
        private string ToHtmlStringRbg(Color color) { return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>"; }

        #region Log

        public void Log(object obj) { this.InternalLog(obj); }

        [Conditional("UNITY_EDITOR")]
        [Conditional("ENABLE_LOG")]
        private void InternalLog(object obj) { Debug.Log(obj); }

        public void Log(object obj, Color color) { this.InternalLog(obj, color); }

        [Conditional("UNITY_EDITOR")]
        [Conditional("ENABLE_LOG")]
        [Conditional("DEVELOPMENT_BUILD")]
        private void InternalLog(object obj, Color color) { Debug.Log($"{obj}{this.ToHtmlStringRbg(color)}"); }

        #endregion

        #region Warning

        public void Warning(object obj) { this.InternalWarning(obj); }

        [Conditional("UNITY_EDITOR")]
        [Conditional("ENABLE_LOG")]
        [Conditional("DEVELOPMENT_BUILD")]
        private void InternalWarning(object obj) { Debug.LogWarning(obj); }

        #endregion

        #region Error

        public void Error(object obj) { this.InternalError(obj); }

        [Conditional("UNITY_EDITOR")]
        [Conditional("ENABLE_LOG")]
        [Conditional("DEVELOPMENT_BUILD")]
        private void InternalError(object obj) { Debug.LogError(obj); }

        #endregion
    }
}