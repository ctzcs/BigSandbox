
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace ComputeShader
{

    public class D2Flocking : MonoBehaviour
    {
        
        public UnityEngine.ComputeShader shader;

        public float boidSpeed = 1f;
        public float neighbourDistance = 2f;
        public GameObject boidPrefab;
        public int boidsCount;
        public float spawnRadius;
        public Transform target;
        public List<Transform> boidsList;//用来存放boids的Transform
        public List<Vector2> minSpawnPositions;
        public List<Vector2> maxSpawnPositions;
        
        public int bufferLength = 256;
        private int _kernelHandle;
        private ComputeBuffer _boidsBuffer;
        private Boid[] _boidsArray;
        /*private GameObject[] _boids;*/
        private int _groupSizeX;//这个值应该填的是Compute Shader中的groupSize
        public int initCount;
        private int _numOfBoids = 0;
        
        
        /*private List<Rigidbody2D> _rbs;*/
        private AsyncGPUReadbackRequest _readback;
        private bool count;

        void Start()
        {
            _kernelHandle = shader.FindKernel("CSMain");

            uint x;
            // 获取 Compute Shader 中定义的 numthreads
            shader.GetKernelThreadGroupSizes(_kernelHandle, out x, out _, out _);
            /*_groupSizeX = Mathf.CeilToInt(boidsCount / (float)x);*/
            // 塞满每个线程组，免得 Compute Shader 中有线程读不到数据，造成读取数据越界
            /*_numOfBoids = _groupSizeX * (int)x;*/
            _groupSizeX = (int)x;

            InitBoids();
            InitShader();
        }

        private void InitBoids()
        {
            /*_boids = new GameObject[numOfBoids];*/
            /*_boidsArray = new Boid[_numOfBoids];*/
            boidsList = new List<Transform>(bufferLength);
            _boidsArray = new Boid[bufferLength];//改成一个固定的极大值的数组
            /*_rbs = new List<Rigidbody2D>(_numOfBoids); //delete*/
            
            //之前的做法
            /*for (int i = 0; i < _boidsArray.Length; i++)
            {
                Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
                pos.z = 0;
                _boidsArray[i] = new Boid(pos);
                _boids[i] = Instantiate(boidPrefab, pos, Quaternion.identity);
                /*_rbs.Add(_boids[i].GetComponent<Rigidbody2D>()); //delete#1#
                _boidsArray[i].direction = _boids[i].transform.up;
            }*/

            for (int i = 0; i < initCount; i++)
            {
                var j = i % 4;
                Vector3 pos = transform.position + new Vector3(Random.Range(minSpawnPositions[j].x,maxSpawnPositions[j].x), Random.Range(minSpawnPositions[j].y,maxSpawnPositions[j].y),0);/*Random.insideUnitSphere * spawnRadius;*/
                pos.z = 0;
                var obj = Instantiate(boidPrefab, pos, Quaternion.identity);
                AddMonster(obj.transform,false);//这里因为是遍历，所以不要让每次都更新
            }
            Debug.Log("ok");

            for (int i = 0; i < boidsList.Count; i++)
            {
                _boidsArray[i] = new Boid(boidsList[i].position);
                _boidsArray[i].direction = boidsList[i].up;
            }
            Vector3 v = new Vector3(-1000, -1000, -1000);
            for (int i = boidsList.Count; i < _boidsArray.Length; i++)
            {
                _boidsArray[i] = new Boid(v);
                _boidsArray[i].direction = Vector3.zero;
            }
            
        }

        void InitShader()
        {
            // 定义大小，鸟的数量和每个鸟结构的大小，一个 Vector3 就是 3 * sizeof(float)
            // 10000 只鸟，每只占 6 * 4 bytes，总共也就占 0.234mib GPU 显存 
            _boidsBuffer = new ComputeBuffer(bufferLength, 6 * sizeof(float));
            _boidsBuffer.SetData(_boidsArray);
            // 设置 buffer 到 Compute Shader，同时设置要调用的计算的函数 Kernel
            shader.SetBuffer(_kernelHandle, "boidsBuffer", _boidsBuffer);
            shader.SetFloat("boidSpeed", boidSpeed);
            shader.SetVector("flockPosition", target.transform.position);
            shader.SetFloat("neighbourDistance", neighbourDistance);
            shader.SetInt("boidsCount", boidsCount);
            shader.Dispatch(_kernelHandle, _groupSizeX, 1, 1);
            //第一次请求的初始化,注意这里的buffer大小是buffer的长度
            _readback = AsyncGPUReadback.Request(_boidsBuffer, bufferLength * 6 * sizeof(float), 0, GetGPUData);
        }
        

        private RaycastHit2D[] _results = new RaycastHit2D[10];
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RemoveTest();
            }
            /*Request();*/
            Callback();
        }

        void RemoveTest()
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Physics2D.CircleCastNonAlloc(pos, 1, Vector2.up,_results);
            if (_results == null)
                return; 
            for (int i = 0; i < _results.Length; i++)
            {
                var t = _results[i].transform;
                if (t.gameObject.CompareTag("Monster"))
                {
                    RemoveMonster(t,false);
                }
                
                
                    
            }
            UpdateArray();
        }

        /*/// <summary>
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
                /#1#/ 阻塞等待 Compute Shader 计算结果从 GPU 传回来
                _boidsBuffer.GetData(_boidsArray);#1#
                _readback = AsyncGPUReadback.Request(_boidsBuffer, _numOfBoids * 6 * sizeof(float), 0);
            }


        }*/

        /// <summary>
        /// 使用回调的方式，目前最优的方式，还有的优化空间是Batch
        /// </summary>
        void Callback()
        {
            //先在外面初始化一次，然后每帧判断是否完成
            if (_readback.done)
            {

                for (int i = 0; i < boidsList.Count; i++)
                {
                    //用list去承载buffer里的内容
                    boidsList[i].position = _boidsArray[i].position;
                    //这句是刚体的移动
                    /*
                     * _rbs[i].MovePosition(_boidsArray[i].position);
                     */
                    //这句是转向，先不要
                    /*if (!_boidsArray[i].direction.Equals(Vector3.zero))
                    {
                        _boids[i].transform.rotation = Quaternion.LookRotation(_boidsArray[i].direction);
                    }*/

                }
                UpdateArray();
                //如果完成了
                // 设置鸟的 position 和 rotation

                //清空数据，填入新的请求，当完成时，将数据拷贝到数组中
                // 设置每一帧会变的变量
                //注意改变数组之后要重新设置buffer，因为改变了buffer里的数组后，需要设置到gpu中才会改变里面的值
                shader.SetBuffer(_kernelHandle, "boidsBuffer", _boidsBuffer);
                shader.SetFloat("deltaTime", Time.deltaTime);
                shader.SetVector("flockPosition", target.transform.position);
                // 调用 Compute Shader Kernel 来计算
                shader.Dispatch(_kernelHandle, _groupSizeX, 1, 1);
                _readback = AsyncGPUReadback.Request(_boidsBuffer, bufferLength * 6 * sizeof(float), 0,GetGPUData);
                
            }

        }
        /// <summary>
        /// 获取数据的回调函数
        /// </summary>
        /// <param name="readback"></param>
        void GetGPUData(AsyncGPUReadbackRequest readback)
        {
            readback.GetData<Boid>().CopyTo(_boidsArray);
        }

    void AddMonster(Transform target,bool needUpdate = true)
        {
            _numOfBoids++;
            boidsList.Add(target);
            if(needUpdate)
                UpdateArray();
        }

        void RemoveMonster(Transform target,bool needUpdate = true) 
        {
            _numOfBoids--;
            target.position = -1000 * Vector2.one;
            boidsList.Remove(target);
            if(needUpdate)
                UpdateArray();
        }


        void UpdateArray()
        {
            Vector3 v = new Vector3(-1000, -1000, -1000);
            for (int i = 0; i < boidsList.Count; i++)
            {
                _boidsArray[i].position = boidsList[i].position;
                _boidsArray[i].direction = boidsList[i].up;
            }

            for (int i = boidsList.Count; i < _boidsArray.Length; i++)
            {
                _boidsArray[i].position = v;
                _boidsArray[i].direction = Vector3.zero;
            }
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
