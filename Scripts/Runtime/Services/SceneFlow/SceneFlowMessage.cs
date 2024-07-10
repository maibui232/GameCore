namespace GameCore.Services.SceneFlow
{
    public class ReleaseSceneMessage
    {
        public ReleaseSceneMessage(string sceneName = "")
        {
            this.SceneName = sceneName;
        }
        public string SceneName { get; set; }
    }

    public class OpenSingleSceneMessage
    {
        public OpenSingleSceneMessage(string sceneName = "")
        {
            this.SceneName = sceneName;
        }
        public string SceneName { get; set; }
    }

    public class OpenAdditiveSceneMessage
    {
        public OpenAdditiveSceneMessage(string sceneName = "")
        {
            this.SceneName = sceneName;
        }
        public string SceneName { get; set; }
    }
}