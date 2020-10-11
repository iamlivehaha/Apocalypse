using StarPlatinum.Base;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private const string SoundPrefix = "Audio/";
        public const string SoundBgModerate = "Bg(moderate)";
        public const string SoundButtonClick = "ButtonClick";
        public string Daytime = "DaytimeMusic";
        public string Night = "NightMusic";
        public string Money = "MoneyMusic";
        public string Danger = "DangerMusic";

        public AudioSource _BGAudioSource;
        public AudioSource _NoramalAudioSource;


        public override void SingletonInit()
        {
            GameObject audioSourceGo = new GameObject("AudioSource(GameObject)");
            _BGAudioSource = audioSourceGo.AddComponent<AudioSource>();
            _NoramalAudioSource = audioSourceGo.AddComponent<AudioSource>();
        }

        public void PlayAudioClip_BG(string soundName)
        {
            PlaySound(_BGAudioSource, LoadSound(soundName), 0.5f, true);
        }

        public void PlayAudioClip_Normal(string soundName)
        {
            PlaySound(_NoramalAudioSource, LoadSound(soundName), 0.5f, false);
        }
        private void PlaySound(AudioSource audioSource, AudioClip clip, float volume, bool loop)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.Play();
        }
        private AudioClip LoadSound(string soundsName)
        {
            return Resources.Load<AudioClip>(SoundPrefix + soundsName);
        }

        public void OnDestroy()
        {
            
        }

    }
}
