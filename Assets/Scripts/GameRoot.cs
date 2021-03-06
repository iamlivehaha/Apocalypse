﻿using Assets.Scripts.Managers;
using StarPlatinum.Base;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace Assets.Scripts
{
    public class GameRoot : MonoSingleton<GameRoot>
    {
        public override void SingletonInit()
        {

        }

        private AudioManager m_audioManager;
        private PlayerManager m_playerManager;
        private UIManager m_UIManager;
        //private CameraSys cameraSys;
        //private AISys aiSys;
        //private FurnitureSys furnitureSys;
        //private PcSys pcSys;
        //private TimeSys timeSys;
        //private UISys uiSys;

        void Start()
        {
            Cursor.visible = false;
            m_audioManager = AudioManager.Instance();
            m_playerManager = PlayerManager.Instance();
            m_UIManager = UIManager.Instance();
            //InitManager();
            m_UIManager.PlayTransitionScene(m_UIManager.m_StartStroy);
        }

        // Update is called once per frame
        void Update()
        {
            //UpdataManager();
            //if (isEnterPlaying)
            //{
            //    //EnterPlaying();开始游戏
            //    isEnterPlaying = false;
            //}
        }

        public void UpdateSpawnPoint(Transform pos)
        {
            m_playerManager.m_defaultSP = pos;
        }
        private void OnDestroy()
        {
            DestroyManager();
        }

        private void DestroyManager()
        {
            //m_audioManager.OnDestroy();
            //m_playerManager.OnDestroy();

        }
    }
}
