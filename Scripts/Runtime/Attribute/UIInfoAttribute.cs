namespace GameCore.Attribute
{
    using System;

    public class UIInfoAttribute : Attribute
    {
        public string AddressableId { get; }
        public uint   OrderLayer    { get; }

        public UIInfoAttribute(string addressableId, uint orderLayer = 0)
        {
            this.AddressableId = addressableId;
            this.OrderLayer    = orderLayer;
        }
    }
}