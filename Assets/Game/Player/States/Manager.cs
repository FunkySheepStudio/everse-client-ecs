using UnityEngine;

namespace Game.Player.States
{
    [AddComponentMenu("Game/Player/States Manager")]
    public class Manager : MonoBehaviour
    {
        BaseState currentState;
        public WalkingState walkingState = new WalkingState();

        private void Start()
        {
            currentState = walkingState;
            currentState.EnterState(this);
        }

        private void Update()
        {
            if (currentState != null)
                currentState.UpdateState(this);
        }

        public void SwitchState(BaseState state)
        {
            currentState = state;
            state.EnterState(this);
        }
    }
}
