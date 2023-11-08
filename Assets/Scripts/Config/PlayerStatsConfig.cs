using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Config/Player Stats", fileName = "New Player Stats")]
    public class PlayerStatsConfig: ScriptableObject
    {
        public float moveSpeed = 3f;
        public float diagonalMovementMultiplier = 0.75f;
        public float sprintSpeedMultiplier = 1.25f;
        public float maxStamina = 100f;
        public float staminaRegenPerSecond = 5f;
        public float sprintStaminaCostPerSecond = 10f;
        public float tiredStatusDuration = 2f;
        public float jumpForce = 10f;
    }
}