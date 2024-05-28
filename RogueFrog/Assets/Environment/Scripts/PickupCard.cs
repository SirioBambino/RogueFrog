using UnityEngine;
using RogueFrog.Characters.Scripts;

namespace RogueFrog.Environment
{
    public class PickupCard : PickupObject
    {
        protected override void OnPickup(Collider other)
        {
            other.GetComponent<PlayerInfo>().HasCard = true;
        }
    }
}
