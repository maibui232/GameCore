namespace GameCore.Services.ScreenFlow
{
    using System.Collections.Generic;
    using GameCore.Attribute;
    using UnityEngine;
    using VContainer;

    public class RootUIView : MonoBehaviour
    {
        [SerializeField] private Camera    uiCamera;
        [SerializeField] private Canvas    rootCanvas;
        [SerializeField] private Canvas    canvasLayerPrefab;
        [SerializeField] private Canvas    defaultLayerCanvas;
        [SerializeField] private Transform closeLayerTransform;

        private readonly Dictionary<OrderLayer, Canvas> orderToLayerCanvas = new();

        public Camera    UICamera            => this.uiCamera;
        public Canvas    RootCanvas          => this.rootCanvas;
        public Canvas    CanvasLayerPrefab   => this.canvasLayerPrefab;
        public Transform CloseLayerTransform => this.closeLayerTransform;

        private void Awake()
        {
            this.AddCanvas(0, this.defaultLayerCanvas);
        }

        private void OnValidate()
        {
            if (this.defaultLayerCanvas != null)
            {
                this.defaultLayerCanvas.overrideSorting = true;
                this.defaultLayerCanvas.sortingOrder    = 0;
            }
        }

        [Inject]
        private void Construct(IScreenFlowService screenFlowService)
        {
            screenFlowService.RootUIView = this;
        }

        public Canvas GetOrCreateOverlayCanvas(OrderLayer orderLayer)
        {
            if (this.orderToLayerCanvas.TryGetValue(orderLayer, out var overlayCanvas)) return overlayCanvas;

            var canvas = Instantiate(this.canvasLayerPrefab, this.transform, false);
            this.AddCanvas(orderLayer, canvas);

            return canvas;
        }

        private void AddCanvas(OrderLayer orderLayer, Canvas canvas)
        {
            canvas.gameObject.name = $"LayerCanvas-{orderLayer}";
            canvas.overrideSorting = true;
            canvas.sortingOrder    = (int)orderLayer;
            this.orderToLayerCanvas.Add(orderLayer, canvas);
        }
    }
}