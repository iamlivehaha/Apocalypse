using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController.Enemy.Weapon;
using UnityEngine;

namespace Assets.Scripts.GamePlay.CharacterController.Enemy
{
    public abstract class IEnemy : ICharacter
    {
        public enum EnemyState
        {
            None,
            Idle,
            Patrol,
            Crouch,
            Chasing,
            Confusing,
            Rise,
            Fall,
            Attack
        }
        [Header("Components")]
        public IWeapon m_weapon = null;

        [Header("Patrol Line")]
        public bool bPatrol = false;
        public List<Transform> m_patrolLine;
        public Transform m_currentDestination = null;
        public Transform m_nextDestination = null;
        public Vector2 m_patrolDirection = new Vector2(1, 0);


        [Header("Behavior Property")]
        public float m_confusingTime = 1.5f;
        public float m_attackInterval = 1.0f;
        public float m_viewDistance = 5.0f;
        public float[] m_viewAngle = new float[2] { 30, 160 };
        public Transform m_target = null;

        [SerializeField]
        public EnemyState previousState, currentState;
        private float m_gravityScale = 9.8f;

        private bool bTargetInView = false;
        private bool bTargetInAttackRange = false;

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
                Debug.LogError("The Patrol Line of "+gameObject+" hasn't been set!");
            }
            if (bPatrol && m_patrolLine.Count >= 2)
            {
                m_currentDestination = m_patrolLine[0];
                m_nextDestination = m_patrolLine[1];
            }

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
                m_patrolDirection = new Vector2(m_currentDestination.position.x - transform.position.x, 0);
            }
            //Determine next destination in normal line
            if (bPatrol && bTargetInView == false)
            {
                if (Mathf.Abs(m_currentDestination.position.x - transform.position.x) < 0.5f)
                {
                    m_currentDestination = m_nextDestination;
                    for (int i = 0; i < m_patrolLine.Count; i++)
                    {
                        if (m_patrolLine[i]==m_currentDestination&&i!= m_patrolLine.Count-1)
                        {
                            m_nextDestination = m_patrolLine[i + 1];
                        }
                        else
                        {
                            m_nextDestination = m_patrolLine[0];
                        }
                    }
                }
            }
            //check target is in view
            if (true)
            {
                Vector3 fwd = transform.TransformDirection(m_patrolDirection);
                Ray ray = new Ray(transform.position + transform.forward, transform.forward);
                Debug.DrawLine(transform.position,transform.position+fwd*m_viewDistance,Color.red);
                bool bHit = Physics.Raycast(ray, out var hit, m_viewDistance);
                if (bHit&&hit.transform.tag=="Player")
                 {
                     m_target = hit.transform;
                     bTargetInView = true;
                 }
                 else
                 {
                     bTargetInView = false;
                 }
            }
            //check target is in attack range
            if (bTargetInView)
            {
                bTargetInAttackRange =
                    Mathf.Abs(m_target.position.x - transform.position.x) < m_weapon.m_range;
            }

            //Movement
            //patrol routine or move to target
            if (bPatrol)
            {
                Vector3 diretion = (m_currentDestination.position - transform.position).normalized;
                Vector3 gravityDeltaVelocity = Physics.gravity * m_gravityScale * Time.fixedDeltaTime;
                if (!m_controller.isGrounded)
                {
                    diretion += gravityDeltaVelocity;
                }
                else
                {
                    diretion.y = 0;
                }
                m_controller.Move(diretion * Time.fixedDeltaTime);
            }
            else if (bTargetInView)
            {
                //Vector3 diretion = new Vector3(m_target.position.x - transform.position.x,0,0).normalized;
                //transform.Translate(diretion * Time.fixedDeltaTime, Space.World);
            }



            if (previousState == EnemyState.Chasing)
            {
                currentState = bTargetInView ? EnemyState.Chasing : EnemyState.Confusing;
            }
            else if (bPatrol)
            {
                currentState = EnemyState.Patrol;
            }
            else
            {
                currentState = EnemyState.Idle;
            }


            bool stateChanged = previousState != currentState;//semaphore

            previousState = currentState;
            if (stateChanged)
            {
                HandleStateChanged();
            }
        }
        // Determine and store character state
        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.tag == "Player" && collision.transform.position.y > transform.position.y)
            {
                if (currentState==EnemyState.Patrol||currentState==EnemyState.Idle)
                {
                    currentState = EnemyState.Confusing;
                }else if (currentState==EnemyState.Confusing)
                {
                    currentState = EnemyState.Chasing;
                }
            }
            HandleStateChanged();
        }

        void HandleStateChanged()
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
                    m_animator.SetTrigger(stateName);
                    break;
                case EnemyState.Chasing:
                    stateName = "chasing";
                    m_animator.SetTrigger("angry");
                    if (bTargetInView)
                    {
                        m_animator.SetBool("run",true);
                    }
                    break;
                case EnemyState.Confusing:
                    stateName = "Confusing";
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
                if (bTargetInView==false)
                {
                    yield break;;
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
