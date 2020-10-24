using System.Collections;
using Assets.Scripts.Managers;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Miner : IEnemy
    {
        public float Attackoffset = 5;
        public bool isBlock = false;

        private Quaternion lookRot;
        [Header("Shovel Attack Setting")]
        public BoxCollider m_WeaponCollider;
        public BoxCollider m_WeaponStickCollider;
        public Transform m_shovelPosition;
        public float m_shovelPulloutVelocity;

        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            m_animator = transform.Find("Visuals/Miner").GetComponent<Animator>();
            m_boxCollider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_skeletonComponent = transform.Find("Visuals/Miner").GetComponent<ISkeletonComponent>();
            m_target = GameObject.FindGameObjectWithTag("Player").transform;

            if (m_WeaponCollider==null)
            {
                Debug.LogError("m_WeaponCollider is not set correctly");
            }
            else
            {
                m_WeaponCollider.enabled = false;
            }
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
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            //check attack angle
            if (!isBlock||bTargetInAttackRange)
            {
                LookTarget(m_target.transform);
            }

            if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("pullout"))
            {
                Vector3 playerPos = PlayerManager.Instance().m_moveCtrl.transform.position;
                if (playerPos.y- m_shovelPosition.position.y<=1.5f&&Mathf.Abs(playerPos.x- m_shovelPosition.position.x)<=1f)
                {
                    Debug.Log("Pull Out! " + playerPos.y + "VS " + m_shovelPosition.position.y);
                    Debug.Log("Pull jump! " + PlayerManager.Instance().m_moveCtrl.inputJumpStart);
                    if (PlayerManager.Instance().m_moveCtrl.inputJumpStart)
                    {
                        PlayerManager.Instance().m_moveCtrl.velocity.y = m_shovelPulloutVelocity;
                    }
                }
            }
            //set weapon collider
            m_WeaponCollider.enabled = m_animator.GetCurrentAnimatorStateInfo(0).IsName("attack");
            if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("attack")
                || m_animator.GetCurrentAnimatorStateInfo(0).IsName("pull")
                || m_animator.GetCurrentAnimatorStateInfo(0).IsName("pullout"))
            {
                m_WeaponStickCollider.enabled = true;
            }
            else
            {
                m_WeaponStickCollider.enabled = false;
            }

        }

        private void CheckAttackAngle()
        {
            if (bTargetInAttackRange && bTargetInView)
            {
                float rotz = m_weapon.transform.rotation.eulerAngles.z;
                if (rotz > 40 && rotz < 180||rotz<=320&&rotz>=300)
                {
                    isBlock = true;
                }
                else
                {
                    isBlock = false;
                }
                //Debug.Log("Block angle = "+ rotz);
                m_animator.SetBool("isblock", isBlock);

            }
        }
        public override void Attack(GameObject o)
        {
            StartCoroutine(StartAttack(o.transform));
        }

        public override void UnderAttack(GameObject o)
        {
            throw new System.NotImplementedException();
        }
        IEnumerator StartAttack(Transform mTarget)
        {

            while (true)
            {
                if (bTargetInView == false)
                {
                    //m_weapon.transform.rotation = Quaternion.AngleAxis(0, Vector3.forward);
                    yield break; ;
                }
                if (bTargetInAttackRange)//attack and rest for a interval
                {
                    CheckAttackAngle();
                    m_WeaponStickCollider.enabled = true;
                    m_animator.SetTrigger("attack");
                    yield return new WaitForSeconds(m_attackInterval);
                    m_WeaponStickCollider.enabled = false; 
                    isBlock = false;
                    m_animator.SetBool("isblock", isBlock);
                }
                else // no target so do nothing
                {
                    yield return null;
                }

            }
        }
        private void LookTarget(Transform mTarget)
        {
            Vector3 attackDir = (mTarget.position - transform.position).normalized;
            float angle = Vector3.Angle(new Vector3(attackDir.x > 0 ? 1 : -1, 0, 0), attackDir);
            //float dir = (mTarget.position - transform.position).normalized.x;
            if (m_defaultDir==DefaultDirection.Right)
            {
                lookRot = Quaternion.AngleAxis(Attackoffset - angle, Vector3.forward);
            }
            else
            {
                lookRot = Quaternion.AngleAxis(Attackoffset - angle, Vector3.forward);
            }
            m_weapon.transform.rotation = Quaternion.Slerp(m_weapon.transform.rotation, lookRot, Time.deltaTime);
            //m_weapon.transform.rotation = rot;
        }
    }
}
