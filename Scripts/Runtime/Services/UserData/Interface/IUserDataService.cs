namespace GameCore.Services.UserData.Interface
{
    using Cysharp.Threading.Tasks;

    public interface IUserDataService
    {
        UniTask Save<T>() where T : IUserData;
        UniTask<T> Load<T>() where T : IUserData;
        UniTask SaveAll();
        UniTask LoadAll();
    }
}