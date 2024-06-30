namespace GameCore.Utils.UIElement
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Playables;

    public class UITransition : MonoBehaviour
    {
        [SerializeField] private PlayableDirector introPlayable;
        [SerializeField] private PlayableDirector outroPlayable;

        private bool isPlayingTimeline;

        private void OnValidate()
        {
            this.ValidatePlayableDirector(this.introPlayable);
            this.ValidatePlayableDirector(this.outroPlayable);
        }

        private void ValidatePlayableDirector(PlayableDirector director)
        {
            if (director == null) return;

            director.extrapolationMode = DirectorWrapMode.None;
            director.timeUpdateMode    = DirectorUpdateMode.UnscaledGameTime;
        }

        private void Awake()
        {
            this.introPlayable.stopped += this.OnStop;
            this.outroPlayable.stopped += this.OnStop;
        }

        private void OnStop(PlayableDirector obj)
        {
            this.isPlayingTimeline = false;
        }

        public UniTask PlayIntro()
        {
            return this.PlayTimeline(this.introPlayable);
        }

        public UniTask PlayOutro()
        {
            return this.PlayTimeline(this.outroPlayable);
        }

        private UniTask PlayTimeline(PlayableDirector director)
        {
            this.isPlayingTimeline = true;
            if (director == null)
            {
                
                return UniTask.CompletedTask;
            }
            director.Play();
            return UniTask.WaitUntil(() => this.isPlayingTimeline);
        }
    }
}