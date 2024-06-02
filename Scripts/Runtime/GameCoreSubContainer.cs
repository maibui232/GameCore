namespace GameCore
{
    using GameCore.Extensions.VContainer;
    using VContainer;

    public class GameCoreSubContainer : SubContainer<GameCoreSubContainer>
    {
        public override void Configuring(IContainerBuilder builder)
        {
        }
    }
}