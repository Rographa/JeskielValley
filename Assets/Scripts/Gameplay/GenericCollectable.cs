using System;
using System.Collections;
using DG.Tweening;
using Interfaces;
using Items;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class GenericCollectable : MonoBehaviour, ICollectable
    {
        [SerializeField] private Vector2 jumpPowerRange;
        [SerializeField] private Vector2Int numberOfJumpsRange;
        [SerializeField] private float jumpDuration = 2f;
        [SerializeField] private float magnetForce;
        [SerializeField] private Ease spawnAnimationEasing;
        [SerializeField] private float spawnAnimationDuration;
        [SerializeField] private Ease disappearAnimationEasing;
        [SerializeField] private float disappearAnimationDuration;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private ItemData _currentItem;
        private bool _wasCollected;
        private bool _canCollect;

        private Transform _magnetTarget;

        private event Action<ItemData> OnCollect;

        public void Setup(Action<ItemData> onCollect, ItemData itemData)
        {
            _currentItem = itemData;
            OnCollect = onCollect;
            spriteRenderer.sprite = itemData.itemVisuals.icon;
            SetActive(true);
        }

        private void JumpToRandomPosition()
        {
            var pos = (Vector2)transform.position + Vector2.one * (Random.insideUnitCircle * 2);
            var jumpPower = Random.Range(jumpPowerRange.x, jumpPowerRange.y);
            var numberOfJumps = Random.Range(numberOfJumpsRange.x, numberOfJumpsRange.y);
            transform.DOJump(pos, jumpPower, numberOfJumps, jumpDuration).OnComplete(EnableCollect);
        }

        public void OnMagnetRangeEnter(Transform origin)
        {
            _magnetTarget = origin;
            StartCoroutine(EnableMagnet());
        }

        private IEnumerator EnableMagnet()
        {
            var fixedUpdate = new WaitForFixedUpdate();
            yield return new WaitUntil(() => _canCollect);
            while (_magnetTarget != null && !_wasCollected)
            {
                var dir = (_magnetTarget.position - transform.position).normalized;
                var force = dir * (magnetForce * Time.fixedDeltaTime);
                transform.Translate(force);
                yield return fixedUpdate;
            }
        }

        public void OnMagnetRangeExit(Transform origin)
        {
            _magnetTarget = null;
        }

        public void Collect()
        {
            if (_wasCollected || !_canCollect) return;
            _wasCollected = true;
            OnCollect?.Invoke(_currentItem);
            SetActive(false);
        }

        private void SetActive(bool value)
        {
            Ease easing;
            float duration;
            float endValue;
            switch (value)
            {
                case true:
                    duration = spawnAnimationDuration;
                    easing = spawnAnimationEasing;
                    endValue = 1;
                    break;
                case false:
                    duration = disappearAnimationDuration;
                    easing = disappearAnimationEasing;
                    endValue = 0;
                    break;
            }

            var tween = transform.DOScale(endValue, duration).SetEase(easing);
            if (!value)
            {
                tween.OnComplete(Destroy);
            }
            else
            {
                tween.OnComplete(JumpToRandomPosition);
            }
        }

        private void EnableCollect()
        {
            _canCollect = true;
        }

        private void Destroy()
        {
            transform.DOKill();
            Destroy(this.gameObject);
        }
    }
}
