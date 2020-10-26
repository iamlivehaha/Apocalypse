using System.Collections;
using StarPlatinum.Base;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    [System.Serializable]
    public struct AudioNode
    {
        public AudioSource audioSource;          //声音池//
        public int volumeAdd;                    //声音变化，+1则递增,-1则递减//
        public float durationTime;               //渐变时间//

        //初始化构造函数//
        public AudioNode(GameObject obj, AudioClip m_clip, float m_initVolume, int m_volumeAdd, float m_durationTime)
        {
            this.audioSource = obj.AddComponent<AudioSource>();
            this.audioSource.playOnAwake = false;
            this.audioSource.volume = m_initVolume;
            this.audioSource.clip = m_clip;
            this.volumeAdd = m_volumeAdd;
            this.durationTime = m_durationTime;
        }
    }

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

        public void PlayTransitionAudio(AudioNode audioNode)
        {
            StartCoroutine(AudioSourceVolume(audioNode));
        }
        //声音渐变迭代器//
        IEnumerator AudioSourceVolume(AudioNode audioNode)
        {
            float initVolume = audioNode.audioSource.volume;
            float preTime = 1.0f / audioNode.durationTime;
            if (!audioNode.audioSource.isPlaying) audioNode.audioSource.Play();
            while (true)
            {
                initVolume += audioNode.volumeAdd * Time.deltaTime * preTime;
                if (initVolume > 1 || initVolume < 0)
                {
                    initVolume = Mathf.Clamp01(initVolume);
                    audioNode.audioSource.volume = initVolume;
                    if (initVolume == 0) audioNode.audioSource.Stop();
                    break;
                }
                else
                {
                    audioNode.audioSource.volume = initVolume;
                }
                yield return 1;
            }
        }
        public void OnDestroy()
        {
            
        }

    }
}
