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
        public PlayerMoveController m_moveCtrl = null;
        List<GameObject> spawnpoints = new List<GameObject>();
        [SerializeField]
        public Transform m_defaultSP;

        public override void SingletonInit()
        {
            m_playerGO = GameObject.FindWithTag("Player");
            foreach (var o in GameObject.FindGameObjectsWithTag("SpawnPoint"))
            {
                spawnpoints.Add(o);
            }

            m_defaultSP = spawnpoints[0].transform;
            m_moveCtrl = m_playerGO.GetComponent<PlayerMoveController>();
        }

        public void SpawnPlayer(Transform spawnpoint)
        {
            if (spawnpoint==null)
            {
                spawnpoint = m_defaultSP;
            }
            StartCoroutine(Spawn(spawnpoint));
        }

        IEnumerator Spawn(Transform spawnpoint)
        {
            SetMoveEnable(false);
            m_playerGO.transform.position = spawnpoint.position+new Vector3(0,0.5f,0);
            EnemyManager.Instance().ResetEnemy();
            //m_playerGO.GetComponent<PlayerMoveController>().AwakenPlayer();
            yield return new WaitForSeconds(2f);//wait for death and spawn animation
            m_moveCtrl.velocity = Vector3.zero;
            m_moveCtrl.ChangeState(PlayerMoveController.PlayerState.Idle);
            SetMoveEnable(true);
            yield break;
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

