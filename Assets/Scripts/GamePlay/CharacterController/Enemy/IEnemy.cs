using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController.Enemy.Weapon;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    [RequireComponent(typeof(ISkeletonAnimation))]
    public abstract class IEnemy : ICharacter
    {
        public enum EnemyState
        {
            None,
            Idle,
            Patrol,
            Chasing,
            Confusing,
            Rise,
            Fall,
            Attack
        }
        public IWeapon m_weapon = null;
        [Header("Components")]
        public BoxCollider m_boxCollider;
        public Rigidbody m_rigidbody;
        public ISkeletonComponent m_skeletonComponent;

        [Header("Patrol Line Setting")]
        public bool bPatrol = false;
        public List<Transform> m_patrolLine;
        public Transform m_currentDestination = null;
        public Transform m_nextDestination = null;
        public Vector3 m_patrolDirection = new Vector3(1, 0, 0);


        [Header("Behavior Property")]
        public float m_patrolSpeed = 1.0f;
        public float m_chasingSpeed = 2.0f;
        public float m_confusingTime = 1.5f;
        public float m_attackInterval = 1.0f;
        public float m_viewDistance = 5.0f;
        public float[] m_viewAngle = new float[2] { 30, 160 };
        public Transform m_target = null;

        [SerializeField]
        public EnemyState previousState, currentState;
        private float m_gravityScale = 9.8f;
        private GameObject m_visuals = null;

        public bool bTargetInView = false;
        public bool bTargetInAttackRange = false;
        private bool bGrounded = true;
        private float moveSpeed = 1.0f;

        protected IEnemy() { }

        public void SetWeapon(IWeapon weapon)
        {
            if (m_weapon != null)
            {
                m_weapon.Release();
            }

            m_weapon = weapon;
            m_weapon.SetOwner(this);
        }

        public IWeapon GetWeapon()
        {
            if (m_weapon == null)
            {
                Debug.LogError("The weapon of " + transform.gameObject + " is missing");
                return null;
            }
            return m_weapon;
        }

        public abstract void Attack(GameObject character);
        public abstract void UnderAttack(GameObject character);


        public override void Init()
        {
            if (bPatrol && m_patrolLine.Count == 0)
            {
                bPatrol = false;
                Debug.LogError("The Patrol Line of " + gameObject + " hasn't been set!");
            }
            if (bPatrol && m_patrolLine.Count >= 2)
            {
                m_currentDestination = m_patrolLine[0];
                m_nextDestination = m_patrolLine[1];
            }

            m_visuals = transform.Find("Visuals").gameObject;
        }

        private void FixedUpdate()
        {
            //determine line when return to patrol 
            if (bPatrol && previousState == EnemyState.Confusing)
            {
                float minmunDistance = 5.0f;
                foreach (var transform1 in m_patrolLine)
                {
                    if (transform1.position.x - transform.position.x < minmunDistance)
                    {
                        minmunDistance = transform1.position.x - transform.position.x;
                        m_currentDestination = transform1;
                    }
                }
            }
            //Determine next destination in normal line
            if (bPatrol && bTargetInView == false)
            {
                if (Mathf.Abs(m_currentDestination.position.x - transform.position.x) < 0.5f)
                {
                    m_currentDestination = m_nextDestination;
                    for (int i = 0; i < m_patrolLine.Count; i++)
                    {
                        if (m_patrolLine[i] == m_currentDestination)
                        {
                            m_nextDestination = i != m_patrolLine.Count - 1 ? m_patrolLine[i + 1] : m_patrolLine[0];
                        }
                    }
                }
                m_patrolDirection = new Vector3(m_currentDestination.position.x - transform.position.x, 0, 0);
            }
            //check target is in view
            if (true)
            {
                Vector3 fwd = m_patrolDirection.normalized;
                Vector3 offset = new Vector3(0, 2, 0);
                Vector3 offset_up = new Vector3(0, 6, 0);
                Vector3 offset_down = new Vector3(0, 1, 0);
                Ray ray = new Ray(transform.position + offset, fwd);
                Ray ray_up = new Ray(transform.position + offset_up, fwd);
                Ray ray_down = new Ray(transform.position + offset_down, fwd);
                Debug.DrawLine(transform.position + offset, transform.position + offset + fwd * m_viewDistance, Color.red);
                Debug.DrawLine(transform.position + offset, transform.position + offset_up + fwd * m_viewDistance, Color.red);
                Debug.DrawLine(transform.position + offset, transform.position + offset_down + fwd * m_viewDistance, Color.red);
                bool bHit = Physics.Raycast(ray, out var hit, m_viewDistance);
                bool bHit_up = Physics.Raycast(ray_up, out var hit_up, m_viewDistance);
                bool bHit_down = Physics.Raycast(ray_down, out var hit_down, m_viewDistance);
                //if (bHit)
                //{
                //    Debug.Log(hit.collider.name);
                //}
                if (bHit || bHit_up || bHit_down)
                {
                    m_target = bHit ? hit.transform : (bHit_up ? hit_up.transform : hit_down.transform);
                    bTargetInView = m_target.tag == "Player";
                }
                else
                {
                    bTargetInView = false;
                }
                m_animator.SetBool("targetinview", bTargetInView);
            }
            //check target is in attack range
            if (bTargetInView)
            {
                bTargetInAttackRange =
                    Mathf.Abs(m_target.position.x - transform.position.x) < m_weapon.m_range;
            }

            //Movement

            #region patrol routine or move to target
            if (bPatrol && bTargetInView == false)
            {
                Vector3 diretion = (m_currentDestination.position - transform.position).normalized;
                FlipXCharacter(diretion.x);
                //m_visuals.transform.localScale = new Vector3(diretion.x > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
                diretion += Gravity();
                transform.Translate(diretion * moveSpeed * Time.fixedDeltaTime);
            }
            else if (bTargetInView)
            {
                Vector3 diretion;
                if (Mathf.Abs(m_target.position.x - transform.position.x) > m_weapon.m_range)
                {
                    diretion = new Vector3(m_target.position.x - transform.position.x, 0, 0).normalized;
                }
                else
                {
                    diretion = Vector3.zero;
                }
                FlipXCharacter(m_target.position.x - transform.position.x);
                //m_visuals.transform.localScale = new Vector3(diretion.x > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
                transform.Translate(diretion * moveSpeed * Time.fixedDeltaTime, Space.World);
            }
            #endregion



            //determine next state

            if (bTargetInView)
            {
                currentState = bTargetInAttackRange ? EnemyState.Attack : EnemyState.Chasing;
            }
            else if (bPatrol && bTargetInView == false)
            {
                currentState = (previousState == EnemyState.Chasing || previousState == EnemyState.Attack)
                    ? EnemyState.Confusing : EnemyState.Patrol;
            }
            else if (bPatrol == false)
            {
                currentState = EnemyState.Idle;
            }


            bool stateChanged = previousState != currentState;//semaphore
            if (stateChanged)
                Debug.Log(previousState + " change to " + currentState);
            previousState = currentState;
            if (stateChanged)
            {
                HandleStateChanged();
            }
        }

        private void FlipXCharacter(float x_value)
        {
            var skeleton = m_skeletonComponent.Skeleton;
            if (skeleton != null)
            {
                skeleton.ScaleX = x_value > 0 ? 1 : -1;
            }
        }
        private Vector3 Gravity()
        {
            Vector3 gravityDeltaVelocity = Physics.gravity * m_gravityScale * Time.fixedDeltaTime;
            if (GroundCheck())
            {
                return gravityDeltaVelocity;
            }
            else
            {
                return Vector3.zero;
            }
        }
        private bool GroundCheck()
        {
            //Physics.Raycast(射线发出位置，射线方向，射线长度，射线碰撞检测Layer）
            bGrounded = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 2.50f);

            return bGrounded;
        }

        // Determine and store character state
        public void OnTriggerEnter(Collider collider)
        {
            Debug.Log("hurt!");
            if (collider.gameObject.tag == "Player" && collider.transform.position.y > transform.position.y)
            {
                if (currentState == EnemyState.Patrol || currentState == EnemyState.Idle)
                {
                    previousState = currentState;
                    currentState = EnemyState.Confusing;
                }
                else if (currentState == EnemyState.Confusing)
                {
                    previousState = currentState;
                    currentState = EnemyState.Chasing;
                }
            }
            HandleStateChanged();
        }

        private void HandleStateChanged()
        {
            // When the state changes, notify the animation handle of the new state.
            string stateName = null;
            switch (currentState)
            {
                case EnemyState.Idle:
                    stateName = "idle";
                    break;
                case EnemyState.Patrol:
                    stateName = "patrol";
                    moveSpeed = m_patrolSpeed;
                    m_animator.SetTrigger(stateName);
                    break;
                case EnemyState.Chasing:
                    stateName = "chasing";
                    moveSpeed = m_chasingSpeed;
                    m_animator.SetTrigger("angry");
                    break;
                case EnemyState.Confusing:
                    stateName = "confusing";
                    m_animator.SetTrigger(stateName);
                    break;
                case EnemyState.Attack:
                    stateName = "attack";
                    StartCoroutine(StartAttack(m_target));
                    break;
                //case EnemyState.Crouch:
                //    stateName = "crouch";
                //    m_animator.SetBool(stateName, true);
                //    break;
                //case EnemyState.Rise:
                //    stateName = "rise";
                //    m_animator.SetBool(stateName, true);
                //    m_animator.SetBool("fall", false);
                //    break;
                //case EnemyState.Fall:
                //    stateName = "fall";
                //    m_animator.SetBool(stateName, true);
                //    m_animator.SetBool("rise", false);
                //    break;

                default:
                    break;
            }
        }

        IEnumerator StartAttack(Transform mTarget)
        {
            while (true)
            {
                if (bTargetInView == false)
                {
                    yield break; ;
                }

                if (bTargetInAttackRange)//attack and rest for a interval
                {
                    Attack(mTarget.gameObject);
                    yield return new WaitForSeconds(m_attackInterval);
                }
                else // no target so do nothing
                {
                    yield return null;
                }

            }

        }
    }
}
