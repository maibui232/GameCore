namespace GameCore.Attribute
{
    using System;

    public class UIInfoAttribute : Attribute
    {
        public UIInfoAttribute(string addressableId, bool overlap = false, uint orderLayer = 0)
        {
            this.AddressableId = addressableId;
            this.OrderLayer    = orderLayer;
            this.Overlap       = overlap;
        }

        public string AddressableId { get; }
        public uint   OrderLayer    { get; }
        public bool   Overlap       { get; }
    }
}