using System.Collections;
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
        [SerializeField]
        private GameObject m_playerGO;
        private PlayerMoveController m_moveCtrl = null;
        List<GameObject> spawnpoints = new List<GameObject>();
        [SerializeField]
        public Transform m_currentSP;

        public override void SingletonInit()
        {
            m_playerGO = GameObject.FindWithTag("Player");
            foreach (var o in GameObject.FindGameObjectsWithTag("SpawnPoint"))
            {
                spawnpoints.Add(o);
            }

            m_currentSP = spawnpoints[0].transform;
            m_moveCtrl = m_playerGO.GetComponent<PlayerMoveController>();
        }

        public void SpawnPlayer(Transform spawnpoint)
        {
            StartCoroutine(Spawn(spawnpoint));
        }

        IEnumerator Spawn(Transform spawnpoint)
        {
            SetMoveEnable(false);
            yield return new WaitForSeconds(2.0f);//wait for death and spawn animation
            m_playerGO.transform.position = m_currentSP.position;
            SetMoveEnable(true);
        }


        public void SetMoveEnable(bool isMove)
        {
            if (m_moveCtrl != null)
            {
                m_moveCtrl.SetMoveEnable(isMove);
            }
        }


        public void OnDestroy()
        {
            if (m_moveCtrl)
            {
                m_moveCtrl = null;
            }
        }
    }
}

