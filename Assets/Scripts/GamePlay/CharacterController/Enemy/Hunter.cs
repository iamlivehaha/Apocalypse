using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public class Hunter : IEnemy
    {
        // Start is called before the first frame update
        public override void Init()
        {
        base.Init();
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
