using System.Collections;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Miner : IEnemy
    {
        public float offset = 5;
        public bool isBlock = false;

        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            m_animator = transform.Find("Visuals/Miner").GetComponent<Animator>();
            m_boxCollider = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_skeletonComponent = transform.Find("Visuals/Miner").GetComponent<ISkeletonComponent>();

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
                    if (!isBlock)
                    {
                        LookTarget(mTarget.transform);
                    }
                    m_animator.SetTrigger("attack");
                    yield return new WaitForSeconds(m_attackInterval);
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
            Debug.Log("ttk " + angle + "t " + Time.time);
            Quaternion rot = Quaternion.AngleAxis(-angle + offset, Vector3.forward);
            m_weapon.transform.rotation = rot;
        }
    }
}
