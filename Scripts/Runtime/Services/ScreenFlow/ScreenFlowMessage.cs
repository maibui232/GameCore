namespace GameCore.Services.ScreenFlow
{
    using GameCore.Services.ScreenFlow.Base;
    using GameCore.Services.ScreenFlow.Base.Screen;

    public class ScreenInitializeMessage
    {
        public IScreenPresenter ScreenPresenter { get; set; }

        public ScreenInitializeMessage(IScreenPresenter screenPresenter)
        {
            this.ScreenPresenter = screenPresenter;
        }
    }

    public class ScreenOpenedMessage
    {
        public IScreenPresenter ScreenPresenter { get; set; }

        public ScreenOpenedMessage(IScreenPresenter screenPresenter)
        {
            this.ScreenPresenter = screenPresenter;
        }
    }

    public class ScreenClosedMessage
    {
        public IScreenPresenter ScreenPresenter { get; set; }

        public ScreenClosedMessage(IScreenPresenter screenPresenter)
        {
            this.ScreenPresenter = screenPresenter;
        }
    }

    public class ScreenDestroyedMessage
    {
        public IScreenPresenter ScreenPresenter { get; set; }

        public ScreenDestroyedMessage(IScreenPresenter screenPresenter)
        {
            this.ScreenPresenter = screenPresenter;
        }
    }
}