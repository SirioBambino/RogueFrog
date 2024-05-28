using UnityEngine;

// Subclass that holds additional information about enemies
namespace RogueFrog.Characters.Scripts
{
    class EnemyInfo : CharacterInfo
    {
        [Header("Ranges")]
        public float PatrolRange = 10.0f;
        public float SightRange = 15.0f;
        public float AttackRange = 10.0f;
    }
}
