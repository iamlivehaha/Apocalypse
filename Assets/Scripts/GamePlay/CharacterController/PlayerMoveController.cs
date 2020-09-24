using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using GamePlay;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [Header("Public, Physics Property")]
    public float m_moveSpeed = 5f;
    public float m_jumpForce = 20f;
    public float m_jumpTime = 1.2f;
    public float m_rayDistance = 2f;

    [Header("Public, Interactive Property")]
    public float m_showInteractiveUIRadius = 1.0f;
    public float m_interactableRadius = 0.5f;
    public float m_interactableRaycastAngle = 90;
    public float m_interactableRaycastAngleInterval = 10;
    public LayerMask m_interactableLayer;


    [Header("Private, Physics Data")]
    [SerializeField]
    private bool m_isInitFaceRight = false;
    [SerializeField]
    private bool m_isFaceRight = false;
    [SerializeField]
    private Animator m_animator;



    //private Rigidbody2D m_rigidbody2D;
    [SerializeField]
    private BoxCollider2D m_boxCollider2D;

    private bool m_isMove = true;
    private bool m_isInteractByUI = false;

    private int count = 0;// 测试计数 Delete in future

    //private collider detection
    static int maxColliders = 10;
    Collider[] hitColliders = new Collider[maxColliders];
    Dictionary<Collider, float> colliders = new Dictionary<Collider, float>();

    Collider interactCollider = null;
    float smallestLength = 10000;

    void Start()
    {
        //m_rigidbody2D = transform.GetComponent<Rigidbody2D> ();
        m_boxCollider2D = transform.GetComponent<BoxCollider2D>();
        if (m_boxCollider2D == null)
        {
            Debug.LogError("Player Collision Is Lost.");
        }
        m_animator = transform.GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.LogError("Player Animator Is Lost.");
        }
        else
        {
            m_animator.SetBool("IsFaceLeft", !m_isFaceRight);
        }

        PlayerManager.Instance().SetMonoMoveController(this);
        //CameraService.Instance.SetTarget(gameObject);
        count = 0;

    }
    // Update is called once per frame
    void Update()
    {
        #region Check Collision
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, m_interactableRadius, hitColliders, m_interactableLayer.value);
        //Debug.Log ("Num of Collisions: " + numColliders);
        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag(InteractiveObject.INTERACTABLE_TAG))
            {
                colliders.Add(hitColliders[i], Vector3.SqrMagnitude(hitColliders[i].transform.position - transform.position));
            }
        }
        foreach (var pair in colliders)
        {
            if (pair.Value < smallestLength)
            {
                smallestLength = pair.Value;
                interactCollider = pair.Key;
            }
        }

        if (interactCollider != null)
        {

        }
        else
        {

        }

        #endregion

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 显示UI
        }


    }


    void FixedUpdate()
    {
        if (!m_isMove)// 不能进行移动
        {
            return;
        }

        #region Horizontal & Vertical Move
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalStepLength = 0;
        float verticalStepLength = 0;

        if (horizontalAxis > 0.1f || horizontalAxis < -0.1f)
        {
            horizontalStepLength = horizontalAxis * m_moveSpeed;
            transform.localPosition = new Vector3(
                transform.localPosition.x + horizontalStepLength * Time.fixedDeltaTime,
                transform.localPosition.y,
                transform.localPosition.z);
        }
        if (verticalAxis > 0.1f || verticalAxis < -0.1f)
        {
            verticalStepLength = verticalAxis * m_moveSpeed;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z + verticalStepLength * Time.fixedDeltaTime);
        }
        //m_animator.SetFloat("Speed", horizontalStepLength * horizontalStepLength + verticalStepLength * verticalStepLength);

        #endregion


        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            StartCoroutine("JumpRoutine");
        }


        //FlipX
        if (horizontalAxis < -0.1f)
        {
            m_isFaceRight = false;
        }
        if (horizontalAxis > 0.1f)
        {
            m_isFaceRight = true;
        }

        if (m_isFaceRight != m_isInitFaceRight)
        {
            transform.localScale = new Vector3(m_isFaceRight ? 1 : -1, transform.localScale.y, transform.localScale.z);
            m_isInitFaceRight = m_isFaceRight;
        }
    }

    IEnumerator JumpRoutine()
    {
        float half = m_jumpTime * 0.5f;
        for (float t = 0; t < half; t += Time.deltaTime)
        {
            float d = m_jumpForce * (half - t);
            transform.Translate((d * Time.deltaTime) * Vector3.up);
            yield return null;
        }
        //for (float t = 0; t < half*0.8f; t += Time.deltaTime)
        //{
        //    float d = m_jumpForce * t;
        //    transform.Translate((d * Time.deltaTime) * Vector3.down);
        //    yield return null;
        //}
    }
    public void SetMoveEnable(bool isEnable)
    {
        m_isMove = isEnable;
    }

    public void StopPlayerAnimation()
    {
        m_animator.SetFloat("Speed", 0.0f);
    }

    public void SetInteract()
    {
        m_isInteractByUI = true;
    }
}
