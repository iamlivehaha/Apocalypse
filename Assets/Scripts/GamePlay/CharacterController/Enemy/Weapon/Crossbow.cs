using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy.Weapon
{
    public class Crossbow : IWeapon
    {
        public Transform m_shootPos;
        public Crossbow() 
        {

        }
        // Start is called before the first frame update
        void Start()
        {
            m_shootPos = transform.Find("shootPosition");
        }

        // Update is called once per frame
        void Update()
        {
        
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
            Debug.Log("shoot");
            GameObject arrow = Resources.Load<GameObject>("Prefabs/Prefabs_Characters_Arrow");
            if (arrow==null)
            {
                Debug.Log("can not find arrow prefab in Prefabs/Prefabs_Characters_Arrow");
            }
            else
            {
                Instantiate(arrow, m_shootPos.position, rot);
            }

        }

    }
}
