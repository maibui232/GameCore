namespace GameCore.Services.Audio
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public interface IAudioService
    {
        float SoundVolume { get; }
        float MusicVolume { get; }
        bool  IsVibrate   { get; }

        void SetSoundVolume(float volume);
        void SetMusicVolume(float volume);
        void SetVibrate(bool      isOn);

        void PlaySound(string                  soundName);
        void PlaySound(AssetReferenceAudioClip assetReferenceAudioClip);
        void PlaySound(AudioClip               clip);

        UniTask PlaySoundAsync(string                  soundName);
        UniTask PlaySoundAsync(AssetReferenceAudioClip assetReferenceAudioClip);
        UniTask PlaySoundAsync(AudioClip               clip);

        void PlayMusic(string                  musicName,               bool isLoop = false);
        void PlayMusic(AssetReferenceAudioClip assetReferenceAudioClip, bool isLoop = false);
        void PlayMusic(AudioClip               clip,                    bool isLoop = false);

        UniTask PlayMusicAsync(string                  musicName,               bool isLoop = false);
        UniTask PlayMusicAsync(AssetReferenceAudioClip assetReferenceAudioClip, bool isLoop = false);
        UniTask PlayMusicAsync(AudioClip               clip,                    bool isLoop = false);

        void StopAllSound(bool forceStop = true);
        void StopAllMusic();
    }

    public class AudioService : IAudioService
    {
#region Implement IAudioService

        public float SoundVolume { get; private set; }
        public float MusicVolume { get; private set; }
        public bool  IsVibrate   { get; private set; }

        public void SetSoundVolume(float volume)
        {
            this.SoundVolume = volume;
        }

        public void SetMusicVolume(float volume)
        {
            this.MusicVolume = volume;
        }

        public void SetVibrate(bool isOn)
        {
            this.IsVibrate = isOn;
        }

        public void PlaySound(string soundName)
        {
            throw new NotImplementedException();
        }

        public void PlaySound(AssetReferenceAudioClip assetReferenceAudioClip)
        {
            throw new NotImplementedException();
        }

        public void PlaySound(AudioClip clip)
        {
            throw new NotImplementedException();
        }

        public UniTask PlaySoundAsync(string soundName)
        {
            throw new NotImplementedException();
        }

        public UniTask PlaySoundAsync(AssetReferenceAudioClip assetReferenceAudioClip)
        {
            throw new NotImplementedException();
        }

        public UniTask PlaySoundAsync(AudioClip clip)
        {
            throw new NotImplementedException();
        }

        public void PlayMusic(string musicName, bool isLoop = false)
        {
            throw new NotImplementedException();
        }

        public void PlayMusic(AssetReferenceAudioClip assetReferenceAudioClip, bool isLoop = false)
        {
            throw new NotImplementedException();
        }

        public void PlayMusic(AudioClip clip, bool isLoop = false)
        {
            throw new NotImplementedException();
        }

        public UniTask PlayMusicAsync(string musicName, bool isLoop = false)
        {
            throw new NotImplementedException();
        }

        public UniTask PlayMusicAsync(AssetReferenceAudioClip assetReferenceAudioClip, bool isLoop = false)
        {
            throw new NotImplementedException();
        }

        public UniTask PlayMusicAsync(AudioClip clip, bool isLoop = false)
        {
            throw new NotImplementedException();
        }

        public void StopAllSound(bool forceStop = true)
        {
            throw new NotImplementedException();
        }

        public void StopAllMusic()
        {
            throw new NotImplementedException();
        }

#endregion
    }
}