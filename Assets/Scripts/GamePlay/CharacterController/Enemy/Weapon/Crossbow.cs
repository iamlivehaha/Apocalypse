using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy.Weapon
{
    public class Crossbow : IWeapon
    {
        public Transform m_shootLeftPos;
        public Transform m_shootRightPos;
        public Crossbow()
        {

        }
        // Start is called before the first frame update
        void Start()
        {
            if (!m_shootLeftPos|| !m_shootRightPos)
            {
                Debug.Log("can not find arrow shoot pos");
            }
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
            if (arrow == null)
            {
                Debug.Log("can not find arrow prefab in Prefabs/Prefabs_Characters_Arrow");
            }
            else
            {
                bool isleft = !(theTarget.transform.position.x - transform.position.x > 0);
                Instantiate(arrow, isleft ? m_shootLeftPos.position : m_shootRightPos.position, isleft ? m_shootLeftPos.rotation : m_shootRightPos.rotation);
            }

        }

    }
}
