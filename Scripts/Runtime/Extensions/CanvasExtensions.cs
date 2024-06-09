namespace GameCore.Extensions
{
    using UnityEngine;

    public static class CanvasExtensions
    {
        public static Matrix4x4 GetCanvasMatrix(this Canvas canvas)
        {
            var rectTr       = canvas.transform as RectTransform;
            var canvasMatrix = rectTr.localToWorldMatrix;
            canvasMatrix *= Matrix4x4.Translate(-rectTr.sizeDelta / 2);
            return canvasMatrix;
        }
    }
}