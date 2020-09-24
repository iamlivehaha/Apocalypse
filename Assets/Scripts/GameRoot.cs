using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private static GameRoot _instance; //唯一单例

    public static GameRoot instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<GameRoot>();
            }
            return _instance;
        }
    }

    //private AudioSys audioSys;
    //private CameraSys cameraSys;
    //private AISys aiSys;
    //private FurnitureSys furnitureSys;
    //private PcSys pcSys;
    //private TimeSys timeSys;
    //private UISys uiSys;

    void Start()
    {
        //InitManager();

    }

    // Update is called once per frame
    void Update()
    {
        //UpdataManager();
        //if (isEnterPlaying)
        //{
        //    //EnterPlaying();开始游戏
        //    isEnterPlaying = false;
        //}
    }

    private void OnDestroy()
    {
        //DestroyManager();
    }
}
