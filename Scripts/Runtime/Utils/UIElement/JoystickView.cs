namespace GameCore.Utils.UIElement
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public enum JoystickType
    {
        Stable,
        Movable
    }

    public enum JoystickState
    {
        Idle,
        Down,
        Move,
        Up
    }

    public class JoystickView : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        [SerializeField] private JoystickType joystickType;
        [SerializeField] private float        radius;
        [SerializeField] private bool         autoDisable;
        [SerializeField] private Transform    joystickHolder;
        [SerializeField] private Transform    joystickHandle;

        public Vector2       JoystickDirection { get;                      private set; }
        public JoystickState JoystickState     { get;                      private set; }
        public JoystickType  JoystickType      { get => this.joystickType; set => this.joystickType = value; }

        private Canvas        canvas;
        private RectTransform canvasRect;
        private Vector3       activeScreenPosition;
        private Vector3       initialPosition;

        private void Awake()
        {
            this.ValidateField();
        }

        private void OnEnable()
        {
            if (this.autoDisable) this.SetActiveView(false);
        }

        private void ValidateField()
        {
            this.canvas          = this.GetComponentInParent<Canvas>();
            this.canvasRect      = this.canvas.GetComponent<RectTransform>();
            this.initialPosition = this.joystickHandle.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.SetActiveView(true);
            this.JoystickState        = JoystickState.Down;
            this.activeScreenPosition = Input.mousePosition;
            var position = this.GetInputUIPosition(this.activeScreenPosition);

            this.joystickHolder.position = position;
            this.joystickHandle.position = position;
            this.JoystickState           = JoystickState.Move;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.JoystickState = JoystickState.Up;
            if (this.autoDisable) this.SetActiveView(false);
            this.joystickHolder.position = this.initialPosition;
            this.joystickHandle.position = this.initialPosition;
            this.JoystickState           = JoystickState.Idle;
        }

        private void SetActiveView(bool isActive)
        {
            this.joystickHolder.gameObject.SetActive(isActive);
            this.joystickHandle.gameObject.SetActive(isActive);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (this.JoystickState != JoystickState.Move) return;
            switch (this.joystickType)
            {
                case JoystickType.Stable:
                    this.OnUpdateStableJoystick();
                    break;
                case JoystickType.Movable:
                    this.OnUpdateMovableJoystick();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnUpdateStableJoystick()
        {
            this.JoystickDirection = (Input.mousePosition - this.activeScreenPosition).normalized;
            var distance       = Vector3.Distance(Input.mousePosition, this.activeScreenPosition);
            var offsetDistance = distance >= this.radius ? this.radius : distance;
            this.joystickHandle.position = this.GetInputUIPosition(this.activeScreenPosition + (Vector3)(offsetDistance * this.JoystickDirection));
        }

        private void OnUpdateMovableJoystick()
        {
            var mousePosition = this.GetInputUIPosition(Input.mousePosition);
            var distance      = Vector3.Distance(Input.mousePosition, this.activeScreenPosition);
            this.JoystickDirection = (Input.mousePosition - this.activeScreenPosition).normalized;

            this.joystickHandle.position = mousePosition;
            if (!(distance > this.radius)) return;

            this.activeScreenPosition    = Input.mousePosition - (Vector3)(this.JoystickDirection * this.radius);
            this.joystickHolder.position = this.GetInputUIPosition(this.activeScreenPosition);
        }

        private Vector3 GetInputUIPosition(Vector3 worldPosition)
        {
            var rootCanvas = this.canvas.isRootCanvas ? this.canvas : this.canvas.rootCanvas;
            if (rootCanvas.worldCamera == null) return worldPosition;

            switch (rootCanvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    return worldPosition;
                case RenderMode.ScreenSpaceCamera:
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(this.canvasRect, worldPosition, this.canvas.worldCamera, out var position);
                    return position;
                case RenderMode.WorldSpace:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Vector3.zero;
        }
    }
}