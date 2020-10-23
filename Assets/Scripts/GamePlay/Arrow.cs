using System.Collections;
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
        public float m_shootVelocity;
        public float m_destroyTime = 10f;
        public bool m_isHit = false;


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
                transform.Translate(m_shootVelocity* m_fwdVector);
            }
        }

        public void OnTriggerEnter(Collider col)
        {
            Debug.Log("hit! "+ col.gameObject);
            if (col.transform.tag=="Wall"||col.transform.tag=="Ground")
            {
                m_isHit = true;
                StartCoroutine(HitGround());
            }
            else if(col.transform.tag=="Player")
            {
                StartCoroutine(DestoryArrow());
            }
        }

        IEnumerator HitGround()
        {
            transform.position = transform.position;
            m_animator.SetBool("isHit",m_isHit);
            m_animator.SetTrigger("hit");
            yield return new WaitForSeconds(3f);
            StartCoroutine(DestoryArrow());
        }

        IEnumerator DestoryArrow()
        {
            float destoryAnimPlayTime = 3;//wait until anim play finished
            yield return new WaitForSeconds(m_destroyTime- destoryAnimPlayTime);
            m_animator.SetTrigger("break");
            yield return new WaitForSeconds(destoryAnimPlayTime);
            Destroy(gameObject);
        }
    }
}
