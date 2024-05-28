using RogueFrog.Characters.Player;
using UnityEngine;

// Subclass that holds additional information about the player
namespace RogueFrog.Characters.Scripts
{
    class PlayerInfo : CharacterInfo
    {
        [Header("Sensitivity")]
        //public float NormalLookSensitivity = 1.0f;
        //public float AimLookSensitivity = 0.5f;
        public PlayerSensSO Sensitivity;

        [Space(10)]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        public bool HasCard;
        public bool HasWon;
    }
}
