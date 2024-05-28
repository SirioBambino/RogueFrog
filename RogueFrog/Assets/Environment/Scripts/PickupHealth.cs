using UnityEngine;
using RogueFrog.Characters.Scripts;

namespace RogueFrog.Environment
{
    public class PickupHealth : PickupObject
    {
        protected override void OnPickup(Collider other)
        {
            other.GetComponent<PlayerInfo>().Health += 50;
        }
    }
}
