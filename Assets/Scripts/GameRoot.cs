using StarPlatinum.Base;

namespace Assets.Scripts
{
    public class GameRoot : MonoSingleton<GameRoot>
    {
        public override void SingletonInit()
        {
            throw new System.NotImplementedException();
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
}
