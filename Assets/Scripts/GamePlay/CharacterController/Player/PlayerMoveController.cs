using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.GamePlay.CharacterController.Enemy;
using Assets.Scripts.Managers;
using GamePlay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GamePlay.CharacterController.Player
{
    [RequireComponent(typeof(UnityEngine.CharacterController))]
    public class PlayerMoveController : ICharacter
    {
        public enum PlayerState
        {
            None,
            Idle,
            Walk,
            Run,
            Crouch,
            Rise,
            Fall,
            Death
        }
        [Header("Components")]
        public UnityEngine.CharacterController m_controller;
        public ISkeletonComponent m_skeletonComponent;

        [Header("DebugTool;")]
        public bool m_godenFinger = false;

        [Header("Public, Physics Property")]
        public float m_walkSpeed = 5f;
        public float m_runSpeed = 10f;
        public float m_gravityScale = 6.6f;


        [Header("Jumping")]
        public float m_jumpSpeed = 25;
        public float m_minimumJumpDuration = 0.5f;
        public float m_jumpInterruptFactor = 0.5f;
        public float m_forceCrouchVelocity = 40;
        public float m_forceCrouchDuration = 0.5f;
        public float m_airControl = 0.8f;
        public float m_crouchControl = 0.5f;

        [Header("Wall Slide and Jump")]
        //public bool m_istouchingFront = false;
        //private bool m_iswallSliding = true;
        //private float m_wallSlidingSpeed = 15;
        public float m_wallJumpUpSpeed = 30;
        public float m_wallJumpBounceVelocity = 10;
        public float m_wallJumpMaskInputXTime;
        public float m_forceWallJumpVelocity = 6f;
        private float m_wallJumpMaskInputXTempTime;
        private bool m_isWallJump = false;
        public float m_shovelPulloutVelocity;

        [Header("Public, Interactive Property")]
        //public float m_showInteractiveUIRadius = 1.0f;
        public float m_interactableRadius = 0.5f;
        //public float m_interactableRaycastAngle = 90;
        //public float m_interactableRaycastAngleInterval = 10;
        public LayerMask m_interactableLayer;

        [Header("Public, Re spawn Property")]
        public float m_deathTime = 3;
        public Transform m_currentSP;
        public List<int> m_collectBro = new List<int>();
        //public bool isdying = false;
        //Temp variate
        // Events
        public event UnityAction OnJump, OnLand, OnHardLand;

        private bool m_isMove = true;
        private bool m_isInteractByUI = false;

        private Vector2 input = default(Vector2);
        public Vector3 velocity = default(Vector3);
        float minimumJumpEndTime = 0;
        float forceCrouchEndTime;

        //Mutex control
        //bool isGrounded = false;
        bool wasGrounded = false;
        public bool isWallJump = false;
        public bool inputJumpStop;
        public bool inputJumpStart;

        public PlayerState previousState, currentState;


        //private collider detection
        static int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        public List<Collider> coliders = new List<Collider>();

        Collider interactCollider = null;



        public override void Init()
        {
            m_controller = GetComponent<UnityEngine.CharacterController>();
            m_animator = transform.Find("Visuals/Creature").GetComponent<Animator>();
            m_skeletonComponent = transform.Find("Visuals/Creature").GetComponent<ISkeletonComponent>();
            m_wallJumpMaskInputXTempTime = m_wallJumpMaskInputXTime;
        }
        // Update is called once per frame
        void Update()
        {
            #region Check Interactable Collision
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, m_interactableRadius, hitColliders, m_interactableLayer.value);
            //Debug.Log("Num of Collisions: " + numColliders);
            //bro check
            for (int i = 0; i < numColliders; i++)
            {
                if (hitColliders[i].CompareTag("Bro"))
                {
                    if (coliders.Contains(hitColliders[i]))
                    {
                        continue;
                    }
                    InteractiveObject hitObj = hitColliders[i].gameObject.GetComponent<InteractiveObject>();
                    if (hitObj.tag=="Bro")
                    {
                        Debug.Log("collect bro!" + hitObj.m_objID);
                        m_collectBro.Add(hitObj.m_objID);
                        hitObj.gameObject.SetActive(false);
                    }
                    coliders.Add(hitColliders[i]);
                }
            }



            #endregion

            #region wall slide and jump
            //m_istouchingFront = Physics.OverlapSphere(,);


            #endregion
            //jump check
            inputJumpStop = Input.GetButtonUp("Jump");
            inputJumpStart = Input.GetButtonDown("Jump");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 显示UI
            }

            #region Move MoveLogic to update func

            if (!m_isMove)// 不能进行移动
            {
                return;
            }

            //float dt = Time.fixedDeltaTime;
            float dt = Time.deltaTime;

            bool isGrounded = m_controller.isGrounded;
            bool landed = !wasGrounded && isGrounded;//Mutex control

            // Dummy input.
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
            bool doCrouch = (isGrounded && input.y < -0.5f) || (forceCrouchEndTime > Time.time);
            bool doJumpInterrupt = false;
            bool doJump = false;
            bool hardLand = false;

            //do Crouch
            if (landed)
            {
                if (-velocity.y > m_forceCrouchVelocity)
                {
                    hardLand = true;
                    doCrouch = true;
                    forceCrouchEndTime = Time.time + m_forceCrouchDuration;
                }
            }
            //do jump 
            if (!doCrouch)//check the mutex
            {
                if (isGrounded)// do jump in ground
                {
                    if (inputJumpStart)
                    {
                        doJump = true;
                    }
                }
                else//stop jump in the air
                {
                    doJumpInterrupt = inputJumpStop && Time.time < minimumJumpEndTime;
                }

            }
            // Dummy physics and controller using UnityEngine.CharacterController.
            //gravity ~ -9.8N/kg => gravity velocity ~ -9.8m/s
            Vector3 gravityDeltaVelocity = Physics.gravity * m_gravityScale * dt;

            if (doJump)// add jump velocity
            {
                velocity.y = m_jumpSpeed;
                minimumJumpEndTime = Time.time + m_minimumJumpDuration;
            }
            else if (doJumpInterrupt)// jump velocity regresses
            {
                if (velocity.y > 0)
                    velocity.y *= m_jumpInterruptFactor;
            }

            //Move Horizontal and set the air control 
            if (m_isWallJump)//mask the input X
            {
                if (Mathf.Abs(m_wallJumpMaskInputXTempTime) <= 0.1f)
                {
                    m_isWallJump = false;
                    m_wallJumpMaskInputXTempTime = m_wallJumpMaskInputXTime;
                }

                m_wallJumpMaskInputXTempTime -= dt;
            }
            else
            {
                velocity.x = 0;
                if (!doCrouch)
                {
                    if (Math.Abs(input.x) > 0.01f)
                    {
                        velocity.x = (isGrounded ? 1 : m_airControl) * Mathf.Abs(input.x) > 0.6f ? m_runSpeed : m_walkSpeed;
                        velocity.x *= Mathf.Sign(input.x);//return the +- symbol
                    }
                }
                else if (doCrouch)
                {
                    if (Math.Abs(input.x) > 0.01f)
                    {
                        velocity.x = m_crouchControl * m_walkSpeed;
                        velocity.x *= Mathf.Sign(input.x);//return the +- symbol
                    }
                }
            }


            //Update the Velocity.Y and move
            if (!isGrounded)
            {
                if (wasGrounded)//clear the velocity if player was grounded right now
                {
                    if (velocity.y < 0)
                        velocity.y = 0;
                }
                else // add the gravityDeltaVelcity
                {
                    velocity += gravityDeltaVelocity;
                }
            }
            m_animator.SetFloat("input.x", Math.Abs(input.x));
            m_animator.SetFloat("velocity_y", Math.Abs(velocity.y));
            velocity.z = 0;
            m_controller.Move(velocity * dt);
            wasGrounded = isGrounded;

            // Determine and store character state
            if (currentState != PlayerState.Death)
            {
                if (isGrounded)
                {
                    if (doCrouch)
                    {
                        currentState = PlayerState.Crouch;
                    }
                    else
                    {
                        if (Math.Abs(input.x) < 0.02f)
                            currentState = PlayerState.Idle;
                        else
                            currentState = Mathf.Abs(input.x) > 0.6f ? PlayerState.Run : PlayerState.Walk;
                    }
                }
                else
                {
                    currentState = velocity.y > 0 ? PlayerState.Rise : PlayerState.Fall;
                }
            }


            bool stateChanged = previousState != currentState;//semaphore

            //Debug.log for state change
            //if (stateChanged)
            //{
            //    Debug.Log(previousState + "transfer to" + currentState);
            //}

            previousState = currentState;

            // Animation
            // Do not modify character parameters or state in this phase. Just read them.
            // Detect changes in state, and communicate with animation handle if it changes.
            if (stateChanged)
            {
                HandleStateChanged();
            }


            //Filp X by velocity
            if (Math.Abs(velocity.x) > 0.02f)
            {
                var skeleton = m_skeletonComponent.Skeleton;
                if (skeleton != null)
                {
                    skeleton.ScaleX = velocity.x > 0 ? -1 : 1;
                }
            }

            // Fire events.
            if (doJump)
            {
                OnJump.Invoke();
            }
            if (landed)
            {
                if (hardLand)
                {
                    OnHardLand.Invoke();
                }
                else
                {
                    OnLand.Invoke();
                }
            }

            #endregion
        }


        void FixedUpdate()
        {
            LockZAxis();
        }

        void HandleStateChanged()
        {
            // When the state changes, notify the animation handle of the new state.
            string stateName = null;
            switch (currentState)
            {
                case PlayerState.Idle:
                    stateName = "idle";
                    m_animator.SetBool("crouch", false);
                    m_animator.SetBool("fall", false);
                    break;
                case PlayerState.Walk:
                    stateName = "walk";
                    m_animator.SetBool("crouch", false);
                    m_animator.SetBool("fall", false);
                    break;
                case PlayerState.Run:
                    stateName = "run";
                    m_animator.SetBool("crouch", false);
                    m_animator.SetBool("fall", false);
                    break;
                case PlayerState.Crouch:
                    stateName = "crouch";
                    m_animator.SetBool(stateName, true);
                    break;
                case PlayerState.Rise:
                    stateName = "rise";
                    m_animator.SetBool(stateName, true);
                    m_animator.SetBool("fall", false);
                    break;
                case PlayerState.Fall:
                    stateName = "fall";
                    m_animator.SetBool(stateName, true);
                    m_animator.SetBool("rise", false);
                    break;
                case PlayerState.Death:
                    stateName = "death";
                    m_animator.SetTrigger(stateName);
                    StartCoroutine(StartDeath());
                    break;
                default:
                    break;
            }
        }
        private void LockZAxis()
        {
            Vector3 currentPosition = transform.position;
            currentPosition.z = 0;
            transform.position = currentPosition;
        }
        //wall jump
        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //wall jump checks
            if (hit.collider.tag == "Wall" || hit.collider.tag == "Enemy")
            {
                if (inputJumpStart && Mathf.Abs(velocity.x) > m_forceWallJumpVelocity && !m_controller.isGrounded)
                {
                    //vertical jump
                    velocity.y = 0;
                    velocity.y = Mathf.Clamp(velocity.y + m_wallJumpUpSpeed, velocity.y, m_wallJumpUpSpeed);
                    //horizontal bounce
                    velocity.x = 0;
                    float wallJumpVelocity = -hit.moveDirection.normalized.x * m_wallJumpBounceVelocity;
                    if (wallJumpVelocity > 0)
                    {
                        velocity.x = Mathf.Clamp(velocity.x + wallJumpVelocity, velocity.x, wallJumpVelocity);
                    }
                    else
                    {
                        velocity.x = Mathf.Clamp(velocity.x + wallJumpVelocity, wallJumpVelocity, velocity.x);
                    }

                    m_isWallJump = true;
                }
            }
            ////shovel jump check
            //if (hit.collider.tag == "Shovel")
            //{
            //    Debug.Log("shovel jump!");
            //    velocity.y = m_shovelPulloutVelocity;
            //}
            //trap check and underAttack check
            if (hit.collider.tag == "Trap" || hit.collider.tag == "Weapon")
            {
                if (!m_godenFinger)
                {
                    Debug.Log("you die!");
                    ChangeState(PlayerState.Death);
                }

            }

        }

        public void OnTriggerEnter(Collider col)
        {
            //spawn point check
            if (col.transform.tag == "SpawnPoint")
            {
                if (col.transform != m_currentSP)
                {
                    Debug.Log("Update spawn point!");
                    m_currentSP = col.transform;
                }
            }
        }
        IEnumerator StartDeath()
        {
            SetMoveEnable(false);
            Time.timeScale = 0.3f;
            yield return new WaitForSeconds(m_deathTime * Time.timeScale);
            PlayerManager playerManager = PlayerManager.Instance();
            playerManager.SpawnPlayer(m_currentSP);
            Time.timeScale = 1;

        }

        public void ChangeState(PlayerState state)
        {
            previousState = currentState;
            currentState = state;
            bool stateChanged = previousState != currentState;//semaphore
            if (stateChanged)
                Debug.Log("Player: " + previousState + " change to " + currentState);
            if (stateChanged)
            {
                HandleStateChanged();
            }
        }
        public void SetMoveEnable(bool isEnable)
        {
            m_isMove = isEnable;
        }


        public void AwakenPlayer()
        {
            Debug.Log("respawn");
            m_animator.SetTrigger("awake");
        }
    }
};