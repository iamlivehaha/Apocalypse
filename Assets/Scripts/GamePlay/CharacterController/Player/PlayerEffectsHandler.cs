using Assets.Scripts.GamePlay.CharacterController.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GamePlay.CharacterController
{
    public class PlayerEffectsHandler : MonoBehaviour
    {
        public PlayerMoveController eventSource;
        public UnityEvent OnJump, OnLand, OnHardLand;

        public void Awake()
        {
            if (eventSource == null)
                return;

            eventSource.OnLand += OnLand.Invoke;
            eventSource.OnJump += OnJump.Invoke;
            eventSource.OnHardLand += OnHardLand.Invoke;
        }
    }
}
