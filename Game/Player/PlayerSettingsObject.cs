using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player {
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Objects/PlayerSettings")]
    public class PlayerSettingsObject : ScriptableObject {

        [Header("General")]
        [Space(4)]
        public float Gravity;
        public float Mass;
        public float GravityTerminalVelocity;
        public float JumpVelocity;
        public float JumpHoldTime;
        [Range(0, 1)]
        public float HeadBounce;

        public float HorizontalSpeed;

        public float CoyoteTime;
        public float AccelTime;
        public AnimationCurve MovementStartAccel;
        public float DecelTime;
        public AnimationCurve MovementEndAccel;


        [Range(0, 1)]
        public float CrouchPercent;

        [Header("Crouching")]
        [Space(4)]
        public float CrouchGravity;
        public float CrouchJumpVelocity;
        public float CrouchMovementSpeed;

        [Header("Sliding")]
        [Space(4)]
        public float slidingVelocity;
        public float slidingVelocityDrag;
        public float slidingAirDrag;
        public float slidingMinVelocity;

    }
}
