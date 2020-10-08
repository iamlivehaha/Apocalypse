using System;
using System.Collections.Generic;
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
            Attack
        }
        [Header("Components")]
        public UnityEngine.CharacterController m_controller;
        public ISkeletonComponent m_skeletonComponent;

        [Header("Public, Physics Property")]
        public float m_walkSpeed = 5f;
        public float m_runSpeed = 7f;
        public float m_gravityScale = 6.6f;
        public float m_rayDistance = 2f;

        [Header("Jumping")]
        public float m_jumpSpeed = 25;
        public float m_minimumJumpDuration = 0.5f;
        public float m_jumpInterruptFactor = 0.5f;
        public float m_forceCrouchVelocity = 40;
        public float m_forceCrouchDuration = 0.5f;
        public float m_airControl = 0.8f;
        public float m_crouchControl = 0.5f;


        [Header("Public, Interactive Property")]
        public float m_showInteractiveUIRadius = 1.0f;
        public float m_interactableRadius = 0.5f;
        public float m_interactableRaycastAngle = 90;
        public float m_interactableRaycastAngleInterval = 10;
        public LayerMask m_interactableLayer;

        //Temp variate
        // Events
        public event UnityAction OnJump, OnLand, OnHardLand;

        private bool m_isMove = true;
        private bool m_isInteractByUI = false;

        public Vector2 input = default(Vector2);
        public Vector3 velocity = default(Vector3);
        float minimumJumpEndTime = 0;
        float forceCrouchEndTime;

        //Mutex control
        //bool isGrounded = false;
        bool wasGrounded = false;

        public PlayerState previousState, currentState;


        //private collider detection
        static int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        Dictionary<Collider, float> colliders = new Dictionary<Collider, float>();

        Collider interactCollider = null;
        float smallestLength = 10000;


        public override void Init()
        {
            m_controller = GetComponent<UnityEngine.CharacterController>();
            m_animator = transform.Find("Visuals/Creature").GetComponent<Animator>();
            m_skeletonComponent = transform.Find("Visuals/Creature").GetComponent<ISkeletonComponent>();
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

            float dt = Time.fixedDeltaTime;
            bool isGrounded = m_controller.isGrounded;
            bool landed = !wasGrounded && isGrounded;//Mutex control

            // Dummy input.
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
            bool inputJumpStop = Input.GetButtonUp("Jump");
            bool inputJumpStart = Input.GetButtonDown("Jump");
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
            //m_animator.SetBool("dojumpInterupt",doJumpInterrupt);
            m_controller.Move(velocity * dt);
            wasGrounded = isGrounded;

            // Determine and store character state
            if (isGrounded)
            {
                if (doCrouch)
                {
                    currentState = PlayerState.Crouch;
                }
                else
                {
                    if (input.x == 0)
                        currentState = PlayerState.Idle;
                    else
                        currentState = Mathf.Abs(input.x) > 0.6f ? PlayerState.Run : PlayerState.Walk;
                }
            }
            else
            {
                currentState = velocity.y > 0 ? PlayerState.Rise : PlayerState.Fall;
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


            //Filp X by rotation
            if (Math.Abs(input.x) > 0.02f)
            {
                var skeleton = m_skeletonComponent.Skeleton;
                if (skeleton != null)
                {
                    skeleton.ScaleX = input.x > 0 ? -1 : 1;
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
                case PlayerState.Attack:
                    stateName = "attack";
                    m_animator.SetTrigger(stateName);
                    break;
                default:
                    break;
            }
        }

        public void SetMoveEnable(bool isEnable)
        {
            m_isMove = isEnable;
        }


        public void SetInteract()
        {
            m_isInteractByUI = true;
        }
    }
};