using UnityEngine;
using UnityEngine.InputSystem;

// This class was adapted and changed from the official unity starter asset third person controller https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526
// Around 20% of code is new, the rest is the same as the original
namespace RogueFrog.Characters.Scripts
{
	public class PlayerInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool aim;
		public bool shoot;
		public bool pause;
		public bool insertCard;

		private PlayerInfo playerInfo;

		private void Start()
		{
			playerInfo = gameObject.GetComponent<PlayerInfo>();
		}

		public void OnMove(InputValue value)
		{
			if (playerInfo.Health > 0 && !playerInfo.HasWon)
				MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			LookInput(value.Get<Vector2>());
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnShoot(InputValue value)
		{
			if (aim)
				ShootInput(value.isPressed);
		}

		public void OnPause(InputValue value)
		{
			PauseInput(value.isPressed);
		}

		public void OnInsertCard(InputValue value)
		{
			InsertCardInput(value.isPressed);
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}

		public void PauseInput(bool newPauseState)
		{
			pause = newPauseState;
		}

		public void InsertCardInput(bool newInsertCardState)
		{
			insertCard = newInsertCardState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
}
