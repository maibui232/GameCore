namespace GameCore.Services.ScreenFlow.Base.Screen
{
    using GameCore.Services.ScreenFlow.MVP;

    public interface IScreenView : IUIView
    {
    }

    public abstract class BaseScreenView : BaseUIView, IScreenView
    {
    }
}