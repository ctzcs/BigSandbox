using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MyFlowField.Scripts.Player
{
    public class HitBox:MonoBehaviour
    {
        public int radius;
        private CircleCollider2D _collider2D;
        
        
        [FormerlySerializedAs("e")] public UnityEvent onHitBoxEnter;
        private void Start()
        { 
            _collider2D = GetComponent<CircleCollider2D>();
            _collider2D.radius = radius;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (onHitBoxEnter != null)
            {
                onHitBoxEnter.Invoke();
                SetEnable(false);
                Invoke(nameof(SetOpen),2f);
            }
        }

        void SetEnable(bool enable)
        {
            _collider2D.enabled = enable;
        }

        void SetOpen()
        {
            SetEnable(true);
        }
    }
}