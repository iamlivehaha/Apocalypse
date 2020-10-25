using System.Collections;
using Assets.Scripts.GamePlay.CharacterController.Player;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.GamePlay
{
    public class Arrow : MonoBehaviour
    {
        public Transform m_head;
        public Transform m_fwd;
        public Vector3 m_fwdVector;

        [Header("Components")]
        public BoxCollider m_headCol;
        public BoxCollider m_arrowCol;
        public Rigidbody m_rigidbody;
        public Animator m_animator;

        [Header("Physic Property")]
        public bool m_bDestory;
        public float m_shootVelocity;
        public float m_destroyTime = 10f;
        public bool m_isHit = false;
        public float destoryAnimPlayTime;//wait until anim play finished
        public Vector3 m_arrowJumpVelocity;


        void Awake()
        {
            m_head = transform.Find("Head");
            m_fwd = transform.Find("Fwd");
            m_fwdVector = (m_fwd.position - transform.position).normalized;
            m_headCol = m_head.GetComponent<BoxCollider>();
            m_arrowCol = GetComponent<BoxCollider>();
            m_rigidbody = GetComponent<Rigidbody>();
            m_animator = GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (m_isHit)
            {
                m_fwdVector = Vector3.zero;
                m_rigidbody.Sleep();
            }
            if (!m_isHit)
            {
                m_fwdVector = (m_fwd.position - transform.position).normalized;
                transform.Translate(m_shootVelocity* m_fwdVector,Space.World);
            }
            //set weapon collider
            m_headCol.enabled = m_animator.GetCurrentAnimatorStateInfo(0).IsName("idle");
        }

        public void OnTriggerEnter(Collider col)
        {
            //Debug.Log("hit! "+ col.gameObject);
            
            if (col.transform.tag=="Wall"||col.transform.tag=="Ground")
            {
                m_isHit = true;
                StartCoroutine(HitGround());
            }
            else if(col.transform.tag=="Player"&&m_isHit==false)
            {
                m_isHit = true;
                Debug.Log("hit! Player in " + col.transform.position.y + "Arrow in " + transform.position.y);
                //if (Mathf.Abs(col.transform.position.y-m_head.position.y)<=3f&&
                //    Mathf.Abs(col.transform.position.x - m_head.position.x) <= 1.5f)
                //{
                    PlayerManager.Instance().m_moveCtrl.ChangeState(PlayerMoveController.PlayerState.Death);
                //}
                //else if (col.transform.position.y >= transform.position.y)
                //{
                //    PlayerManager.Instance().m_moveCtrl.velocity = m_arrowJumpVelocity;
                //}
                StartCoroutine(DestoryArrow(destoryAnimPlayTime));
            }
            else if(col.transform.tag == "Player" && m_isHit)
            {
                PlayerManager.Instance().m_moveCtrl.velocity = m_arrowJumpVelocity;
                m_animator.SetTrigger("hit");
                StartCoroutine(DestoryArrow(destoryAnimPlayTime));
            }
        }

        IEnumerator HitGround()
        {
            transform.position = transform.position;
            m_animator.SetBool("isHit",true);
            m_animator.SetTrigger("hit");
            yield return new WaitForSeconds(2f);
            m_animator.SetBool("isHit", false);
            StartCoroutine(DestoryArrow(m_destroyTime));
        }

        IEnumerator DestoryArrow(float destoryTime)
        {
            if (m_bDestory)
            {
                m_arrowCol.enabled = false;
                yield return new WaitForSeconds(destoryTime - destoryAnimPlayTime);
                m_animator.SetTrigger("break");
                yield return new WaitForSeconds(destoryAnimPlayTime);
                Destroy(gameObject);
            }
        }
    }
}
