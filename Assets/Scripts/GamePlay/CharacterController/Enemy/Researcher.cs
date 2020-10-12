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
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void Attack(GameObject o)
        {
            m_animator.SetTrigger("attack");

            //var bone = m_skeletonComponent.Skeleton.FindBone("bone01");

        }

        public override void UnderAttack(GameObject o)
        {

        }
    }
}
