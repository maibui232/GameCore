namespace GameCore.Services.ScreenFlow.Base.Item
{
    using GameCore.Services.ScreenFlow.MVP;

    public interface IItemView : IUIView
    {
    }

    public abstract class BaseItemView : BaseUIView, IItemView
    {
    }
}