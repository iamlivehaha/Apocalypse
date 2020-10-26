using UnityEngine;

namespace Assets.Scripts.GamePlay.CameraService
{
    public class CameraTrigger : MonoBehaviour
    {
        [SerializeField]
        public CameraSetting m_triggerCameraSetting;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        public void OnTriggerStay(Collider col)
        {
            //spawn point check
            if (col.transform.tag == "Player")
            {
                CameraManager.Instance().SetCameraSetting(m_triggerCameraSetting);
            }
            //else
            //{
            //    CameraManager.Instance().ReturnDefaultSetting();
            //}


        }
    }
}
