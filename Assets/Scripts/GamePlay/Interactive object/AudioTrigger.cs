using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.GamePlay.Interactive_object
{
    public class AudioTrigger : MonoBehaviour
    {
        public AudioNode m_audioNode;
        public AudioClip m_audioClip;
        // Start is called before the first frame update
        void Start()
        {
            this.m_audioNode.audioSource.clip = m_audioClip;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public void OnTriggerEnter(Collider col)
        {
            if (col.transform.tag == "Player")
            {
                m_audioNode.audioSource.volume = 0;
                m_audioNode.volumeAdd = 1;
                AudioManager.Instance().PlayTransitionAudio(m_audioNode);
            }
        }

        public void OnTriggerExit(Collider col)
        {
            if (col.transform.tag == "Player")
            {
                m_audioNode.audioSource.volume = 1;
                m_audioNode.volumeAdd = -1;
                AudioManager.Instance().PlayTransitionAudio(m_audioNode);
            }
        }
    }
}
