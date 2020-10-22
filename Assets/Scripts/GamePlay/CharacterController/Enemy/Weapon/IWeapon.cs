using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy.Weapon
{
    public abstract class IWeapon : MonoBehaviour
    {
        [Header("Interactive Property")]
        public float m_range = 1.0f;

        [Header("Components")]
        protected GameObject m_gameObejct = null;
        protected ICharacter m_weaponOwner = null;


        [Header("Effect and Audio")]
        protected float m_effectDisplayTime = 0.5f;
        protected ParticleSystem m_particleSystem;
        protected AudioSource m_audio;

        protected IWeapon()
        {
        }
        public void SetOwner(ICharacter character)
        {
            if (m_weaponOwner == null)
            {
                Debug.LogError("The character owner of " + transform.gameObject + " is missing");
                return;
            }
            m_weaponOwner = character;
        }

        protected abstract void ShowAttackEffect();

        protected void ShowSoundEffect(string ClipName)
        {
            if (m_audio==null)
            {
                Debug.LogError("The audioSource of "+transform.gameObject+" is missing");
                return;
            }

            AudioManager.Instance().PlayAudioClip_Normal(ClipName);
        }

        public abstract void Attack(GameObject theTarget);
        public abstract void Attack(GameObject theTarget, Quaternion rot);

        public void Release()
        {
            m_weaponOwner = null;
            Destroy(this.gameObject);
        }


    }
}
