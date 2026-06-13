using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Configuration for the skiing controller. Create via Assets > Create > SugarRush > Skiing Config.
    /// </summary>
    [CreateAssetMenu(fileName = "SkiingConfig", menuName = "SugarRush/Skiing Config")]
    public class SkiingConfig : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Base downhill acceleration (units/s^2).")]
        public float baseAcceleration = 12f;

        [Tooltip("Maximum downhill speed.")]
        public float maxSpeed = 22f;

        [Tooltip("Friction/decay when no acceleration applied.")]
        public float groundFriction = 2f;

        [Tooltip("Slope angle in degrees (0 = flat, 90 = vertical).")]
        public float slopeAngle = 15f;

        [Header("Air Control")]
        [Tooltip("Upward force for jump.")]
        public float jumpForce = 14f;

        [Tooltip("Gravity scale while airborne.")]
        public float gravityScale = 2.5f;

        [Tooltip("Horizontal air control multiplier (0-1).")]
        public float airControl = 0.4f;

        [Header("Jump Feel")]
        [Tooltip("Time after leaving ground where jump still works.")]
        public float coyoteTime = 0.1f;

        [Tooltip("Time window to buffer a jump before landing.")]
        public float jumpBufferTime = 0.1f;

        [Tooltip("Upward bounce applied on landing to soften impact.")]
        public float landingBounce = 2f;

        [Header("Roll")]
        [Tooltip("Duration of the roll state (invulnerable to obstacles).")]
        public float rollDuration = 0.6f;

        [Tooltip("Cooldown between rolls.")]
        public float rollCooldown = 0.3f;

        [Tooltip("Forward speed boost during roll.")]
        public float rollSpeedBoost = 6f;

        [Tooltip("Instant horizontal kick when roll starts.")]
        public float rollInitialBoost = 3f;

        [Tooltip("Hitbox height multiplier while rolling.")]
        [Range(0.1f, 1f)]
        public float rollHitboxHeightScale = 0.5f;

        [Header("Ground Check")]
        [Tooltip("Layer mask for ground detection.")]
        public LayerMask groundMask;

        [Tooltip("Distance from center to ground check raycast origin.")]
        public float groundCheckOffset = 0.55f;

        [Tooltip("Ground check ray length.")]
        public float groundCheckDistance = 0.1f;

        [Header("Stumble")]
        [Tooltip("Speed reduction factor on stumble.")]
        public float stumbleSpeedFactor = 0.4f;

        [Tooltip("Stumble lockout duration.")]
        public float stumbleDuration = 0.8f;

        public Vector2 SlopeDirection => new Vector2(Mathf.Cos(slopeAngle * Mathf.Deg2Rad), -Mathf.Sin(slopeAngle * Mathf.Deg2Rad));
    }
}
