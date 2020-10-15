﻿using System.Collections;
using Spine.Unity;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Researcher : IEnemy
    {
        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            m_animator = transform.Find("Visuals/Researcher").GetComponent<Animator>();
            m_boxCollider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_skeletonComponent = transform.Find("Visuals/Researcher").GetComponent<ISkeletonComponent>();

            // property setting
            m_viewDistance = 8.0f;
            m_animator.SetBool("bpatrol", bPatrol);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void Attack(GameObject o)
        {
            StartCoroutine(StartAttack(o.transform));
            //var bone = m_skeletonComponent.Skeleton.FindBone("bone01");

        }

        public override void UnderAttack(GameObject o)
        {

        }

        IEnumerator StartAttack(Transform mTarget)
        {
            while (true)
            {
                if (bTargetInView == false)
                {
                    yield break; ;
                }

                if (bTargetInAttackRange)//attack and rest for a interval
                {
                    Vector3 attackDir = (mTarget.position - transform.position).normalized;
                    m_weapon.transform.rotation.SetLookRotation(attackDir);
                    m_animator.SetTrigger("attack");
                    yield return new WaitForSeconds(m_attackInterval);
                }
                else // no target so do nothing
                {
                    yield return null;
                }

            }
        }
    }
}
