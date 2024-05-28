using Cinemachine;
using RogueFrog.Environment.Scripts.Generation;
using RogueFrog.UI.Scripts;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

// This class was adapted and changed from this video tutorial https://www.youtube.com/watch?v=FbM4CkqtOuA
// Which is further adapted from the official unity starter asset third person controller https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526
// Around 65% of code is new, the rest is the same as the original

namespace RogueFrog.Characters.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerInfo))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private Rig aimRig;
        [SerializeField] private Transform aimTarget;
        [SerializeField] private LayerMask aimColliderMask;

        [SerializeField] private Transform projectilePrefab;
        [SerializeField] private Transform spawnProjectilePosition;

        [Header("Cinemachine")]
        [SerializeField] private GameObject cinemachineCameraTarget;
        [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

        [Header("Audio")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [SerializeField] private AudioClip shotAudioClip;

        private float aimRigWeight;
        private bool isAiming = false;

        // cinemachine
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;

        // player
        private float speed;
        private float animationBlend;
        private float targetRotation = 0.0f;
        private float rotationVelocity;

        // animation IDs
        private int animIDSpeed;
        private int animIDMotionSpeed;

        private Animator animator;
        private CharacterController controller;
        private PlayerInputs input;
        private PlayerInput playerInput;
        private PlayerInfo playerInfo;
        private bool hasMoved = false;
        public bool handmade = false;

        private bool IsCurrentDeviceMouse
        {
            get { return playerInput.currentControlScheme == "KeyboardMouse"; }
        }

        private void Start()
        {
            cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            input = GetComponent<PlayerInputs>();
            playerInput = GetComponent<PlayerInput>();
            playerInfo = GetComponent<PlayerInfo>();

            AssignAnimationIDs();

            playerInfo.Health = playerInfo.MaxHealth;
            playerInfo.Ammo = playerInfo.MaxAmmoInClip;
            Time.timeScale = 1;
        }

        private void Update()
        {
            if (playerInfo.Health > 0)
            {
                // If game is not paused player is able to move and aim
                if (!PauseMenu.isPaused && !playerInfo.HasWon)
                {
                    Move();
                    Aim();
                    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                }
            }
            else
            {
                animator.SetBool("IsDead", true);
            }

            if (input.move != Vector2.zero) hasMoved = true;
            if (!hasMoved) transform.position = (handmade) ? transform.position : LevelGenerationManager.spawnPoint;
        }

        private void LateUpdate()
        {
            // If game is not paused, player is alive and player hasn't won, player is able to turn camera
            if (!PauseMenu.isPaused && playerInfo.Health > 0 && !playerInfo.HasWon)
                CameraRotation();
        }

        private void CameraRotation()
        {
            // Check if there is an input
            if (input.look.sqrMagnitude >= 0.01f)
            {
                // Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // Change look sensitivity if player is aiming or not
                float lookSensitivity = isAiming ? playerInfo.Sensitivity.aimSensitivity : playerInfo.Sensitivity.lookSensitivity;

                cinemachineTargetYaw += input.look.x * deltaTimeMultiplier * lookSensitivity;
                cinemachineTargetPitch += input.look.y * deltaTimeMultiplier * lookSensitivity;
            }

            // Clamp rotations so values are limited 360 degrees
            cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, -25.0f, 30.0f);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // Set target speed based on move speed, if there is no input, set the target speed to 0
            float targetSpeed = (input.move == Vector2.zero) ? 0.0f : playerInfo.MovementSpeed;

            // A reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = 1f;

            // Accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // Creates curved result rather than a linear one giving a more organic speed change
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * 10.0f);
            }
            else
            {
                speed = targetSpeed;
            }

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * 10.0f);
            if (animationBlend < 0.01f) animationBlend = 0f;

            // Normalise input direction
            Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

            // If there is a move input rotate player when the player is moving
            if (input.move != Vector2.zero)
            {
                targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, playerInfo.RotationSmoothTime);

                // Rotate to face input direction relative to camera position
                if (!isAiming)
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            // Move the player
            controller.Move(targetDirection.normalized * (speed * Time.deltaTime));

            // Update animator
            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, inputMagnitude);
        }

        private void Aim()
        {
            Vector3 mouseWorldPosition = Vector3.zero;

            // Cast a ray into the world starting from the center of the screen 
            Vector2 screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999.0f, aimColliderMask))
            {
                mouseWorldPosition = raycastHit.point;
                aimTarget.position = raycastHit.point;
            }

            if (input.aim)
            {
                // Change camera and animation
                aimVirtualCamera.gameObject.SetActive(true);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1.0f, Time.deltaTime * 10.0f));
                isAiming = true;

                // Get the direction for aiming and rotate player towards that direction
                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20.0f);

                aimRigWeight = 1.0f;

                if (input.shoot)
                {
                    Shoot(mouseWorldPosition);
                    playerInfo.Ammo--;
                }
            }
            else
            {
                // Change camera and animation
                aimVirtualCamera.gameObject.SetActive(false);
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0.0f, Time.deltaTime * 10.0f));
                isAiming = false;

                aimRigWeight = 0.0f;
            }

            aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20.0f);
        }

        private void Shoot(Vector3 mouseWorldPosition)
        {
            // Get the direction of shooting and instantiate a projectile towards that direction
            Vector3 aimDirection = (mouseWorldPosition - spawnProjectilePosition.position).normalized;
            Transform projectile = Instantiate(projectilePrefab, spawnProjectilePosition.position, Quaternion.LookRotation(aimDirection, Vector3.up));
            projectile.gameObject.GetComponent<Projectile>().Damage = playerInfo.AttackDamage;
            input.shoot = false;

            // Play shot audio clip
            mixer.GetFloat("MainVolume", out float volume);
            AudioSource.PlayClipAtPoint(shotAudioClip, transform.TransformPoint(controller.center), Mathf.Pow(10, (volume / 20.0f)) * 0.75f);
        }

        private void AssignAnimationIDs()
        {
            animIDSpeed = Animator.StringToHash("Speed");
            animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        // Play random footstep audio clip
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (footstepAudioClips.Length > 0)
                {
                    mixer.GetFloat("MainVolume", out float volume);
                    var index = Random.Range(0, footstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(controller.center), Mathf.Pow(10, (volume / 20.0f)) * 0.2f);
                }
            }
        }
    }
}