namespace GameCore.Attribute
{
    using System;

    public class ScreenInfoAttribute : Attribute
    {
        public ScreenInfoAttribute(string addressableId, OrderLayer orderLayer = OrderLayer.Main, bool overlap = false)
        {
            this.AddressableId = addressableId;
            this.OrderLayer    = orderLayer;
            this.Overlap       = overlap;
        }

        public string           AddressableId { get; }
        public OrderLayer OrderLayer    { get; }
        public bool             Overlap       { get; }
    }

    public enum OrderLayer
    {
        Background = 0,
        Main       = 1,
        Overlay    = 2,
        Layer3     = 3,
        Layer4     = 4,
        Layer5     = 5,
        Layer6     = 6,
        Layer7     = 7,
        Layer8     = 8,
        Layer9     = 9,
        Layer10    = 10,
        Layer11    = 11,
        Layer12    = 12,
        Layer13    = 13,
        Layer14    = 14,
        Layer15    = 15,
        Layer16    = 16,
        Layer17    = 17,
        Layer18    = 18,
        Layer19    = 19,
        Layer20    = 20
    }
}