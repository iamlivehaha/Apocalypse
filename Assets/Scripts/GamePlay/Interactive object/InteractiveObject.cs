using StarPlatinum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public class InteractiveObject : MonoBehaviour
    {
        [SerializeField]
        private int INTERACTABLE_Layer;

        public int m_objID = 1;
        public string m_objectName = "";

        void Awake()
        {
            INTERACTABLE_Layer = LayerMask.GetMask("Interactable");
        }
        public void Start()
        {

            if ( gameObject.layer!= INTERACTABLE_Layer)
            {
                gameObject.layer = INTERACTABLE_Layer;
            }

            if (m_objectName != null || m_objectName != "")
            {
                //bool result = SingletonGlobalDataContainer.Instance.RegisterNewObject(m_objectName);
                //if (result == false) {
                //	Debug.LogError ("Global data container alraedy contain " + m_objectName);
                //}
            }
            else
            {
                Debug.LogError("Interactive object Doesn`t have name");
            }
        }

    }
}
