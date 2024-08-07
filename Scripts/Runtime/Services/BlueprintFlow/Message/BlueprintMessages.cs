namespace GameCore.Services.BlueprintFlow.Message
{
    public class LoadBlueprintDataMessage
    {
        public string Url;
        public string Hash;
    }

    public interface IProgressPercent
    {
        public float Percent { get; }
    }

    public class LoadBlueprintDataSucceedMessage
    {
    }

    public class LoadBlueprintDataProgressMessage : IProgressPercent
    {
        public float Percent { get; set; }
    }

    public class BlueprintProgressMessage : IProgressPercent
    {
        public int MaxBlueprint;
        public int CurrentProgress;

        public float Percent => 1f * this.CurrentProgress / this.MaxBlueprint;
    }
}