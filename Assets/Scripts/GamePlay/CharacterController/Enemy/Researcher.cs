using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Researcher : IEnemy
    {
        // Start is called before the first frame update
        public override void Init()
        {
            base.Init();
            m_controller = GetComponent<UnityEngine.CharacterController>();
            m_animator = transform.Find("Visuals/Researcher").GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void Attack(GameObject o)
        {
            throw new System.NotImplementedException();
        }

        public override void UnderAttack(GameObject o)
        {
            throw new System.NotImplementedException();
        }
    }
}
