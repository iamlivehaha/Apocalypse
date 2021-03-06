﻿using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Hunter : IEnemy
    {
        public float Attackoffset_left;
        public float Attackoffset_right;
        private Quaternion shootlookRot;
        private float Attackoffset;
        private Quaternion lookRot;
        private bool isAttack = false;
        public float mattackTempInterval;

        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            m_animator = transform.Find("Visuals/Hunter").GetComponent<Animator>();
            m_boxCollider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_skeletonComponent = transform.Find("Visuals/Hunter").GetComponent<ISkeletonComponent>();
            m_target = GameObject.FindGameObjectWithTag("Player").transform;

            if (!bPatrol)
            {
                m_defaultPosition = Instantiate(new GameObject(gameObject.name + "_defaultPos"), transform.position,
                    Quaternion.identity).transform;
                m_patrolLine.Clear();
                m_patrolLine.Add(m_defaultPosition);
                m_currentDestination = m_patrolLine[0];
                m_nextDestination = m_patrolLine[0];
                bPatrol = true;
            }

            // property setting
            m_animator.SetBool("bpatrol", bPatrol);
            mattackTempInterval = m_attackInterval - 1;
            if (m_defaultDir == DefaultDirection.Right)
            {
                Attackoffset = Attackoffset_right;
            }
            else
            {
                Attackoffset = Attackoffset_left;
            }
        }

        public void FixedUpdate()
        {
            if (isAttack)
            {
                if (Mathf.Abs(mattackTempInterval) <= 0.1f)
                {
                    bool isleft = !(m_target.transform.position.x - transform.position.x > 0);
                    m_weapon.Attack(m_target.gameObject, isleft ? lookRot : shootlookRot);
                    isAttack = false;
                    mattackTempInterval = m_attackInterval;
                }
                mattackTempInterval -= Time.fixedDeltaTime;
            }
        }
        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (bTargetInView)
            {
                LookTarget(m_target.transform.position + new Vector3(0, 1.5f, 0));
            }

            if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("startattack"))
            {
                isAttack = true;
            }
        }

        private void LookTarget(Vector3 mTargetPos)
        {
            Vector3 attackDir = (mTargetPos + Vector3.up - transform.position).normalized;
            float angle = Vector3.Angle(new Vector3(attackDir.x > 0 ? 1 : -1, 0, 0), attackDir);
            float dir = (mTargetPos - transform.position).normalized.x;
            lookRot = Quaternion.AngleAxis(Attackoffset - angle, Vector3.forward);
            m_weapon.transform.rotation = Quaternion.Slerp(m_weapon.transform.rotation, lookRot, Time.deltaTime * 2);
            //m_weapon.transform.rotation = rot;
        }
        IEnumerator StartAttack(Transform mTarget)
        {

            while (true)
            {
                if (!bTargetInView)
                {
                    //m_weapon.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                    isAttack = false;
                    mattackTempInterval = m_attackInterval;
                    yield break;
                }
                if (bTargetInAttackRange)//attack and rest for a interval
                {
                    m_animator.SetTrigger("attack");
                    //todo arrow attack 
                    shootlookRot = Quaternion.AngleAxis(180 - lookRot.eulerAngles.z, Vector3.forward);
                    yield return new WaitForSeconds(m_attackInterval);
                }
                else
                {
                    Debug.Log("attack end");
                    yield break;
                }

            }
        }

        public void AttackImmediately()
        {
            shootlookRot = Quaternion.AngleAxis(180 - lookRot.eulerAngles.z, Vector3.forward);
            bool isleft = !(m_target.transform.position.x - transform.position.x > 0);
            m_weapon.Attack(m_target.gameObject, isleft ? lookRot : shootlookRot);

        }
        public override void Attack(GameObject o)
        {
            if (!isAttack)
            {
                StartCoroutine(StartAttack(o.transform));
            }
        }

        public override void UnderAttack(GameObject o)
        {

        }
    }
}
