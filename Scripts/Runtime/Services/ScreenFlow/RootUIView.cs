namespace GameCore.Services.ScreenFlow
{
    using System.Collections.Generic;
    using UnityEngine;
    using VContainer;

    public class RootUIView : MonoBehaviour
    {
        [SerializeField] private Camera    uiCamera;
        [SerializeField] private Canvas    rootCanvas;
        [SerializeField] private Canvas    canvasLayerPrefab;
        [SerializeField] private Canvas    defaultLayerCanvas;
        [SerializeField] private Transform closeLayerTransform;

        public Camera    UICamera            => this.uiCamera;
        public Canvas    RootCanvas          => this.rootCanvas;
        public Canvas    CanvasLayerPrefab   => this.canvasLayerPrefab;
        public Transform CloseLayerTransform => this.closeLayerTransform;

        private readonly Dictionary<int, Canvas> orderToLayerCanvas = new();

        private void OnValidate()
        {
            if (this.defaultLayerCanvas != null)
            {
                this.defaultLayerCanvas.overrideSorting = true;
                this.defaultLayerCanvas.sortingOrder    = 0;
            }
        }

        private void Awake()
        {
            this.AddCanvas(0, this.defaultLayerCanvas);
        }

        [Inject]
        private void Construct(IScreenFlowService screenFlowService)
        {
            screenFlowService.RootUIView = this;
        }

        public Canvas GetOrCreateOverlayCanvas(int orderLayer)
        {
            if (this.orderToLayerCanvas.TryGetValue(orderLayer, out var overlayCanvas))
            {
                return overlayCanvas;
            }

            var canvas = Instantiate(this.canvasLayerPrefab, this.transform, false);
            this.AddCanvas(orderLayer, canvas);
            return canvas;
        }

        private void AddCanvas(int orderLayer, Canvas canvas)
        {
            canvas.gameObject.name = $"LayerCanvas-{orderLayer}";
            canvas.overrideSorting = true;
            canvas.sortingOrder    = orderLayer;
            this.orderToLayerCanvas.Add(orderLayer, canvas);
        }
    }
}