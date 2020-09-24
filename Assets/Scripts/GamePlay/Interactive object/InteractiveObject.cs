using StarPlatinum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public class InteractiveObject : MonoBehaviour
    {
        public static readonly string INTERACTABLE_TAG = "Interactive";

        public string m_objectName = "";

        public void Start()
        {
            if (tag != INTERACTABLE_TAG)
            {
                tag = INTERACTABLE_TAG;
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

        public void Interact()
        {
            GameObject controller = GameObject.Find("ControllerManager");
            if (controller == null)
            {
                return;
            }

        }
    }
}
