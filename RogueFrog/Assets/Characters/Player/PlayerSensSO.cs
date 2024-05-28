using UnityEngine;

namespace RogueFrog.Characters.Player
{
    [CreateAssetMenu(fileName = "", menuName = "PlayerSensitivity")]
    public class PlayerSensSO : ScriptableObject
    {
        public float lookSensitivity;
        public float aimSensitivity;
    }
}
