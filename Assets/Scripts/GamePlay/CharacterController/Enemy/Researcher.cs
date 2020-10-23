using System.Collections;
using Spine.Unity;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Researcher : IEnemy
    {
        private Quaternion lookRot;
        public BoxCollider m_WeaponCollider;
        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            m_animator = transform.Find("Visuals/Researcher").GetComponent<Animator>();
            m_boxCollider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_skeletonComponent = transform.Find("Visuals/Researcher").GetComponent<ISkeletonComponent>();
            m_target = GameObject.FindGameObjectWithTag("Player").transform;

            if (m_WeaponCollider == null)
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
                m_patrolLine.Add(m_defaultPosition);
                m_currentDestination = m_patrolLine[0];
                m_nextDestination = m_patrolLine[0];
                bPatrol = true;
            }
            // property setting
            m_animator.SetBool("bpatrol", bPatrol);
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
                    m_WeaponCollider.enabled = true;
                    Vector3 attackDir = (mTarget.position - transform.position).normalized;
                    m_weapon.transform.rotation.SetLookRotation(attackDir);
                    m_animator.SetTrigger("attack");
                    yield return new WaitForSeconds(m_attackInterval);
                    m_WeaponCollider.enabled = false;
                }
                else // no target so do nothing
                {
                    yield return null;
                }

            }
        }
    }
}
