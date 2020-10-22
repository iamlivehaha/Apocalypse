using System.Collections;
using Spine.Unity;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Hunter : IEnemy
    {
        public float Attackoffset = 5;
        private Quaternion lookRot;

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
            }

            // property setting
            m_animator.SetBool("bpatrol", bPatrol);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            LookTarget(m_target.transform);
        }

        private void LookTarget(Transform mTarget)
        {
            Vector3 attackDir = (mTarget.position - transform.position).normalized;
            float angle = Vector3.Angle(new Vector3(attackDir.x > 0 ? 1 : -1, 0, 0), attackDir);
            float dir = (mTarget.position - transform.position).normalized.x;
            lookRot = Quaternion.AngleAxis(dir > 0 ? -angle - Attackoffset : -angle + Attackoffset, Vector3.forward);
            m_weapon.transform.rotation = Quaternion.Slerp(m_weapon.transform.rotation, lookRot, Time.deltaTime);
            //m_weapon.transform.rotation = rot;
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
                    m_animator.SetTrigger("attack");
                    //todo arrow attack 
                    m_weapon.Attack(m_target.gameObject,lookRot);
                    yield return new WaitForSeconds(m_attackInterval);
                }
                else // no target so do nothing
                {
                    yield return null;
                }

            }
        }
        public override void Attack(GameObject o)
        {
            StartCoroutine(StartAttack(o.transform));
        }

        public override void UnderAttack(GameObject o)
        {

        }
    }
}
