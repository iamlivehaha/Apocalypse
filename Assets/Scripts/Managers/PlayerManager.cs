using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController;
using Assets.Scripts.GamePlay.CharacterController.Player;
using StarPlatinum.Base;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        /// <summary>移动控制</summary>
        private PlayerMoveController m_moveCtrl = null;
        List<GameObject> spawnpoints = new List<GameObject>();

        public override void SingletonInit()
        {
            foreach (var o in GameObject.FindGameObjectsWithTag("SpawnPoint"))
            {
                spawnpoints.Add(o);
            }
            
        }

        public void SpawnPlayer(Transform spawnpoint)
        {

        }

        public void SetPlayerMoveController(PlayerMoveController moveController)
        {
            m_moveCtrl = moveController;
        }


        public void SetMoveEnable(bool isMove)
        {
            if (m_moveCtrl != null)
            {
                m_moveCtrl.SetMoveEnable(isMove);
            }
        }

        //public void JoystickMoveEvent(Vector2 vec)
        //{
        //    //Debug.Log(vec);
        //    InputService.Instance.SetAxis(KeyMap.Horizontal, vec.x);
        //    InputService.Instance.SetAxis(KeyMap.Vertical, vec.y);
        //}

        //public void JoystickMoveEndEvent()
        //{
        //    InputService.Instance.SetAxis(KeyMap.Horizontal, 0f);
        //    InputService.Instance.SetAxis(KeyMap.Vertical, 0f);

        //}

        private void OnDestroy()
        {
            if (m_moveCtrl)
            {
                m_moveCtrl = null;
            }
        }
    }
}

