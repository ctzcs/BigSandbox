using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using Vector2 = System.Numerics.Vector2;

namespace ComputeShader
{

    public class D2Flocking : MonoBehaviour
    {
        public UnityEngine.ComputeShader shader;

        public float boidSpeed = 1f;
        public float neighbourDistance = 1f;
        public GameObject boidPrefab;
        public int boidsCount;
        public float spawnRadius;
        public Transform target;


        private int _kernelHandle;
        private ComputeBuffer _boidsBuffer;
        private Boid[] _boidsArray;
        private GameObject[] _boids;
        private int _groupSizeX;
        private int _numOfBoids;
        private List<Rigidbody2D> _rbs;
        private AsyncGPUReadbackRequest _readback;
        private bool count;

        void Start()
        {
            _kernelHandle = shader.FindKernel("CSMain");

            uint x;
            // 获取 Compute Shader 中定义的 numthreads
            shader.GetKernelThreadGroupSizes(_kernelHandle, out x, out _, out _);
            _groupSizeX = Mathf.CeilToInt(boidsCount / (float)x);
            // 塞满每个线程组，免得 Compute Shader 中有线程读不到数据，造成读取数据越界
            _numOfBoids = _groupSizeX * (int)x;

            InitBoids();
            InitShader();
        }

        private void InitBoids()
        {
            _boids = new GameObject[_numOfBoids];
            _boidsArray = new Boid[_numOfBoids];
            _rbs = new List<Rigidbody2D>(_numOfBoids); //delete
            for (int i = 0; i < _numOfBoids; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
                pos.z = 0;
                _boidsArray[i] = new Boid(pos);
                _boids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
                _rbs.Add(_boids[i].GetComponent<Rigidbody2D>()); //delete
                _boidsArray[i].direction = _boids[i].transform.up;
            }
        }

        void InitShader()
        {
            // 定义大小，鸟的数量和每个鸟结构的大小，一个 Vector3 就是 3 * sizeof(float)
            // 10000 只鸟，每只占 6 * 4 bytes，总共也就占 0.234mib GPU 显存 
            _boidsBuffer = new ComputeBuffer(_numOfBoids, 6 * sizeof(float));
            _boidsBuffer.SetData(_boidsArray);
            // 设置 buffer 到 Compute Shader，同时设置要调用的计算的函数 Kernel
            shader.SetBuffer(_kernelHandle, "boidsBuffer", _boidsBuffer);
            shader.SetFloat("boidSpeed", boidSpeed);
            shader.SetVector("flockPosition", target.transform.position);
            shader.SetFloat("neighbourDistance", neighbourDistance);
            shader.SetInt("boidsCount", boidsCount);
            //第一次请求的初始化
            _readback = AsyncGPUReadback.Request(_boidsBuffer, _numOfBoids * 6 * sizeof(float), 0);
        }


        void Update()
        {
            //TODO 初始化shader数组里的数据似乎可以每帧更新

            /*Request();*/
            Callback();
        }

        /// <summary>
        /// 直接获取数据的方式
        /// </summary>
        void Request0()
        {
            // 设置每一帧会变的变量
            shader.SetFloat("deltaTime", Time.deltaTime);
            shader.SetVector("flockPosition", target.transform.position);
            // 调用 Compute Shader Kernel 来计算
            shader.Dispatch(_kernelHandle, _groupSizeX, 1, 1);
            // 阻塞等待 Compute Shader 计算结果从 GPU 传回来
            _boidsBuffer.GetData(_boidsArray);
            // 设置鸟的 position 和 rotation
            for (int i = 0; i < _boidsArray.Length; i++)
            {
                _boids[i].transform.localPosition = _boidsArray[i].position;

                if (!_boidsArray[i].direction.Equals(Vector3.zero))
                {
                    _boids[i].transform.rotation = Quaternion.LookRotation(_boidsArray[i].direction);
                }

            }
        }

        /// <summary>
        /// 使用等待的方式
        /// </summary>
        void Request()
        {
            if (_boidsBuffer == null)
            {
                return;
            }

            //如果没有完成就等待
            if (!_readback.done)
            {
                _readback.WaitForCompletion();
            }

            //完成了进入一个新请求
            if (_readback.done && !_readback.hasError)
            {
                _readback.GetData<Boid>().CopyTo(_boidsArray);
                // 设置鸟的 position 和 rotation
                for (int i = 0; i < _boidsArray.Length; i++)
                {
                    _boids[i].transform.localPosition = _boidsArray[i].position;

                    if (!_boidsArray[i].direction.Equals(Vector3.zero))
                    {
                        _boids[i].transform.rotation = Quaternion.LookRotation(_boidsArray[i].direction);
                    }

                }

                // 设置每一帧会变的变量
                shader.SetFloat("deltaTime", Time.deltaTime);
                shader.SetVector("flockPosition", target.transform.position);
                // 调用 Compute Shader Kernel 来计算
                shader.Dispatch(_kernelHandle, _groupSizeX, 1, 1);
                /*// 阻塞等待 Compute Shader 计算结果从 GPU 传回来
                _boidsBuffer.GetData(_boidsArray);*/
                _readback = AsyncGPUReadback.Request(_boidsBuffer, _numOfBoids * 6 * sizeof(float), 0);
            }


        }

        /// <summary>
        /// 使用回调的方式，目前最优的方式，还有的优化空间是Batch
        /// </summary>
        void Callback()
        {
            //先在外面初始化一次，然后每帧判断是否完成
            if (_readback.done)
            {

                for (int i = 0; i < _boidsArray.Length; i++)
                {
                    _boids[i].transform.localPosition = _boidsArray[i].position;
                    /*_rbs[i].MovePosition(_boidsArray[i].position);*/

                    /*if (!_boidsArray[i].direction.Equals(Vector3.zero))
                    {
                        _boids[i].transform.rotation = Quaternion.LookRotation(_boidsArray[i].direction);
                    }*/

                }

                //如果完成了
                // 设置鸟的 position 和 rotation

                //清空数据，填入新的请求，当完成时，将数据拷贝到数组中
                // 设置每一帧会变的变量
                shader.SetFloat("deltaTime", Time.deltaTime);
                shader.SetVector("flockPosition", target.transform.position);
                // 调用 Compute Shader Kernel 来计算
                shader.Dispatch(_kernelHandle, _groupSizeX, 1, 1);
                _readback = AsyncGPUReadback.Request(_boidsBuffer, _numOfBoids * 6 * sizeof(float), 0,
                    (o) => { o.GetData<Boid>().CopyTo(_boidsArray); });
            }

        }

        private RaycastHit2D[] _hit2Ds;

        void KillBoid()
        {
            if (Input.GetMouseButton(0))
            {
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Physics2D.RaycastNonAlloc(pos, Vector3.zero, _hit2Ds);
                if (_hit2Ds != null)
                {
                    var entity = _hit2Ds[0].transform;
                    if (entity.CompareTag("Monster"))
                    {
                        var c = entity.GetComponent<Boid>();
                        for (int i = 0; i < _boidsArray.Length; i++)
                        {
                            /*if (_boidsArray[i] == c)
                            {

                            }*/
                        }

                        Destroy(entity.gameObject);
                        OverrideBuffer();
                    }
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                _hit2Ds = null;
            }
        }

        void OverrideBuffer()
        {

        }
        /*private const int RemoveOnce = 8;
        private Queue<int> _removeBuffer = new Queue<int>(RemoveOnce);
        void Destory(Boid boid)
        {
            if (_removeBuffer.Count == RemoveOnce)
            {
                //执行切换数组

                //刷新容量
                _removeBuffer.Clear();
            }

            for (int i = 0; i < _boidsArray.Length; i++)
            {
                if (boid.GetHashCode() == _boidsArray[i].GetHashCode())
                {
                    _removeBuffer.Enqueue(i);
                }
            }
        }

        void Brush(Queue<int> queue)
        {
            _boidsArray = new Boid[_boidsArray.Length - RemoveOnce];
            for (int i = 0; i < queue.Count; i++)
            {
                int a = queue.Dequeue();
                //怎么可以一次性移除呢
            }
            
            //重新计算_numsOfBoids*/
        void OnDestroy()
        {
            if (_boidsBuffer != null)
            {
                // 用完主动释放 buffer
                _boidsBuffer.Dispose();
            }
        }
    }

    
}
