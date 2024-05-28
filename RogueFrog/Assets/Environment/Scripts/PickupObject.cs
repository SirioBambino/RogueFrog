using UnityEngine;
using DG.Tweening;
using RogueFrog.Characters.Scripts;

namespace RogueFrog.Environment
{
    public abstract class PickupObject : MonoBehaviour
    {
        [SerializeField] protected float BobbingDuration = 1.5f;
        [SerializeField] protected float RotationDuration = 7.0f;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            transform.position = new Vector3(transform.position.x, 0.75f, transform.position.z);
            transform.DOMoveY(1.25f, BobbingDuration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            transform.DORotate(new Vector3(0, 360, 0), RotationDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerInfo>()) return;
            
            OnPickup(other);
            _audioSource.Play();
            
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            transform.DOKill();
            Destroy(gameObject, _audioSource.clip.length);
        }

        protected abstract void OnPickup(Collider other);
    }
}
