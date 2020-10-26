using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController.Enemy;
using Assets.Scripts.Managers;
using StarPlatinum.Base;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIManager : MonoSingleton<UIManager>
{
    /// <summary>移动控制</summary>
    [SerializeField] public List<string> m_StartStroy = new List<string>();

    public List<string> m_AwakenStroy = new List<string>();
    public Text m_UIText;
    public Image m_sceneTransition;
    public VideoPlayer videoPlayer;

    public float m_stancePlayTime;
    public float m_crossFadeAlphaTime;
    public float m_trainsitionTime;

    public override void SingletonInit()
    {
        m_UIText = GameObject.FindGameObjectWithTag("Subtitle").GetComponent<Text>();
        m_UIText.CrossFadeAlpha(0, m_crossFadeAlphaTime, false);

        m_StartStroy.Add("好孤独…");
        m_StartStroy.Add("我听说在很久以前");
        m_StartStroy.Add("直立行走的生物蚕食了我的家园");
        m_StartStroy.Add("带走了我的伙伴，夺走了他们的皮毛和身体");
        m_StartStroy.Add("现在，我居住的森林只剩下荒芜");
        m_StartStroy.Add("万物复苏还有希望吗？也许繁荣永远留在了历史里");
        m_StartStroy.Add("又或许，在某个遥远的地方，还有新的伙伴和新的家园");
        m_StartStroy.Add("我一定要找到它");


        m_AwakenStroy.Add("原来是场梦…");
        m_AwakenStroy.Add("与其现在孤身一人，我宁愿做无数个这样的梦");
        m_AwakenStroy.Add("和伙伴们共度最后一刻");
        m_AwakenStroy.Add("一次一次，循环往复…");

    }

    public void PlayTransitionScene(List<string> storytoPlay)
    {
        if (m_sceneTransition.GetComponent<Animation>().Play("SceneTransitionAnimation"))
        {
            PlayerManager.Instance().SetMoveEnable(false);
            StartCoroutine(ClosePanel(m_trainsitionTime));
            StartCoroutine(PlayStory(storytoPlay));
        }
    }

    public void PlayMovie()
    {
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached; ;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        //End Reached
        //todo awke player
    }



    IEnumerator PlayStory(List<string> m_story)
    {
        foreach (string s in m_story)
        {
            m_UIText.text = s;
            m_UIText.CrossFadeAlpha(1, m_crossFadeAlphaTime, false);
            yield return new WaitForSeconds(m_stancePlayTime);
            m_UIText.CrossFadeAlpha(0, m_crossFadeAlphaTime, false);
            yield return new WaitForSeconds(m_crossFadeAlphaTime);
        }
    }

    public IEnumerator ClosePanel(float length)
    {
        float timer = 0;
        while (timer < length)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        PlayerManager.Instance().SetMoveEnable(true);
    }

}
