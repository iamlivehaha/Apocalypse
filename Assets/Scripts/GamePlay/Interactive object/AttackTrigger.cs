using Assets.Scripts.GamePlay.CharacterController.Enemy;
using UnityEngine;

namespace Assets.Scripts.GamePlay.Interactive_object
{
    public class AttackTrigger : MonoBehaviour
    {
        public GameObject m_attacker;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public void OnTriggerEnter(Collider col)
        {
            //spawn point check
            if (col.transform.tag == "Player")
            {
                m_attacker.GetComponent<Hunter>().AttackImmediately();
            }
        }
        public void OnTriggerExit(Collider col)
        {
            //spawn point check
            //if (col.transform.tag == "Player")
            //{
            //m_attacker.GetComponent<Hunter>().AttackImmediately();
            //}
        }
    }
}

