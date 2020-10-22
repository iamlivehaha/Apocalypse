using Assets.Scripts.GamePlay.CharacterController.Player;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy.Weapon
{
    public class Catch : IWeapon
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnCollisionEnter(Collision col)
        {
            //spawn point check
            if (col.collider.tag == "Player")
            {
                PlayerMoveController player = col.transform.GetComponent<PlayerMoveController>();
                if (player!=null&& player.m_godenFinger)
                {
                    Debug.Log("you die!");
                    player.ChangeState(PlayerMoveController.PlayerState.Death);
                }
            }
        }
        protected override void ShowAttackEffect()
        {
            throw new System.NotImplementedException();
        }

        public override void Attack(GameObject theTarget)
        {
            throw new System.NotImplementedException();
        }

        public override void Attack(GameObject theTarget, Quaternion rot)
        {
            throw new System.NotImplementedException();
        }
    }
}
