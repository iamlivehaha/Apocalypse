using System;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController
{

    public abstract class ICharacter : MonoBehaviour
    {

        protected string m_name = "";
        protected GameObject m_gameObejct;
        protected Animator m_animator;

        protected bool m_bKilled = false;
        protected float m_removeTime = 1.5f;
        protected bool m_bCanRemove = false;

        protected ICharacter(){}

        public void Start()
        {
            Init();
        }

        public abstract void Init();
        public void SetGameObject(GameObject gameObject)
        {
            m_gameObejct = gameObject;
            m_animator = gameObject.GetComponent<Animator>();
        }

        public GameObject GetGameObject()
        {
            if (m_gameObejct==null)
            {
                m_gameObejct = transform.gameObject;
                Debug.LogWarning("Character = "+m_name+" GameObject Is Lost.");
            }
            return m_gameObejct;
        }
        public String GetGameObjectName()
        {
            if (m_gameObejct == null)
            {
                m_gameObejct = transform.gameObject;
                Debug.LogWarning("Character = " + m_name + " GameObject Is Lost.");
            }
            return m_gameObejct.name;
        }

    }
}
