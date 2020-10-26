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
        public Transform m_initSpawnPoint;

        public override void SingletonInit()
        {
            m_playerGO = GameObject.FindWithTag("Player");
            foreach (var o in GameObject.FindGameObjectsWithTag("SpawnPoint"))
            {
                spawnpoints.Add(o);
            }

            m_defaultSP = spawnpoints[0].transform;
            if (m_initSpawnPoint==null)
            {
                m_initSpawnPoint = m_defaultSP;
            }
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
        public void AwakenPlayer()
        {
            StartCoroutine(Awaken());
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

        IEnumerator Awaken()
        {
            SetMoveEnable(false);
            m_playerGO.transform.position = m_initSpawnPoint.position + new Vector3(0, 0.5f, 0);
            EnemyManager.Instance().ResetEnemy();
            UIManager uiManager = UIManager.Instance();
            m_playerGO.GetComponent<PlayerMoveController>().AwakenPlayer();
            uiManager.PlayTransitionScene(uiManager.m_AwakenStroy);
            SetMoveEnable(false);
            yield return  new WaitForSeconds(uiManager.m_trainsitionTime);
            yield return new WaitForSeconds(3f);//wait for death and spawn animation
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

