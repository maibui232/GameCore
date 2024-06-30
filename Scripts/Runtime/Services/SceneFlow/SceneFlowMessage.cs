namespace GameCore.Services.SceneFlow
{
    public class ReleaseSceneMessage
    {
        public string SceneName { get; set; }

        public ReleaseSceneMessage(string sceneName = "")
        {
            this.SceneName = sceneName;
        }
    }

    public class OpenSingleSceneMessage
    {
        public string SceneName { get; set; }

        public OpenSingleSceneMessage(string sceneName = "")
        {
            this.SceneName = sceneName;
        }
    }

    public class OpenAdditiveSceneMessage
    {
        public string SceneName { get; set; }

        public OpenAdditiveSceneMessage(string sceneName = "")
        {
            this.SceneName = sceneName;
        }
    }
}