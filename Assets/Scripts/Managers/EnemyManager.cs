using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController;
using Assets.Scripts.GamePlay.CharacterController.Enemy;
using Assets.Scripts.GamePlay.CharacterController.Player;
using StarPlatinum.Base;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        /// <summary>移动控制</summary>
        [SerializeField]
        private GameObject[] m_currentEnemyGO;
        public IEnemy m_currentEnemyCtrl = null;
        List<IEnemy> m_enemyList = new List<IEnemy>();

        public override void SingletonInit()
        {
            m_currentEnemyGO = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var o in m_currentEnemyGO)
            {
                m_currentEnemyCtrl = o.GetComponent<IEnemy>();
                m_enemyList.Add(m_currentEnemyCtrl);
            }
        }

        public void ResetEnemy()
        {
            foreach (var o in m_enemyList)
            {
                if (o.m_patrolLine[0]!=null)
                {
                    o.transform.position = o.m_patrolLine[0].position;
                }
            }
        }
    }
}

