using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KdTree
{
    public class Entity:MonoBehaviour
    {
        private float _elapsedTime = 0;
        private float _delay = 0;
        private Material _material;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            this.transform.position += new Vector3(0.01f*Mathf.Sin(_elapsedTime),0,0.01f* Mathf.Sin(_elapsedTime));
        }
        
        public void ChangeColor(float delay)
        {
            //如果还有时间，让上一个携程延长时间
            if (_delay > 0 && _material.color == Color.red)
            {
                _delay += delay;
                return;
            }
            StartCoroutine(IEChangeColor(delay));
        }
        /// <summary>
        /// 核心是通过改变_delay来计算延时
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        IEnumerator IEChangeColor(float delay)
        {
            _delay += delay;
            var c = _material.color;
            _material.color = Color.red;
            while (_delay > 0)
            {
                yield return new WaitForSeconds(0.1f);
                _delay -= 0.2f;
            }
            _material.color = c;
            _delay = 0;
        }
    }
}