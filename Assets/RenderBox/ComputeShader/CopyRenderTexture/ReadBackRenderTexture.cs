
using UnityEngine;
using UnityEngine.Rendering;

public class ReadBackRenderTexture : MonoBehaviour
{
    public UnityEngine.ComputeShader cs;
    public Renderer useForRenderTexture;
    private RenderTexture _rt;
    
    private int _groupSize = 8;
    private int _size = 64;
    private int _pixelCount;
    private int _rtBufferSize;
    private int _csKernel;
    private ComputeBuffer _renderTextureBuffer;

    private static readonly int Result = Shader.PropertyToID("Result");
    private static readonly int Time1 = Shader.PropertyToID("Time");

    // Start is called before the first frame update
    void Start()
    {
        _pixelCount = _size * _size;
        _rtBufferSize = 4 * sizeof(float);
        _csKernel = cs.FindKernel("CSMain");
        _rt = new RenderTexture(_size,_size,0,RenderTextureFormat.ARGB32)
        {
            enableRandomWrite = true,
            filterMode = FilterMode.Point
        };
        
        _rt.Create();
        _renderTextureBuffer = new ComputeBuffer(_pixelCount,_rtBufferSize);
        useForRenderTexture.material.mainTexture = _rt;
        
    }

    private void OnDestroy()
    {
        _renderTextureBuffer.Dispose();
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        AsyncGPUCompleteReadback();
    }

    void AsyncGPUCompleteReadback()
    {
        cs.SetTexture(_csKernel,Result,_rt);
        cs.SetFloat(Time1,Time.time);
        cs.Dispatch(_csKernel,_pixelCount/_groupSize,_pixelCount/_groupSize,1);
        //AsyncGPUReadback.Request(_renderTextureBuffer, _pixelCount * _rtBufferSize, 0, OnCompleteReadback_RenderTexture);
    }
    
    //renderTexture无需回读
    void OnCompleteReadback_RenderTexture(AsyncGPUReadbackRequest request)
    {
        if(request.hasError )return;
        //useForRenderTexture.material.mainTexture = _rt;
        //TODO 可以读Texture但是对线程组有要求，不能是一维线程组
        // var data = request.GetData<Color32>();//request.GetData<Color32>();
        // //MyLog.Log($"{num}");
        // Graphics.CopyTexture(_rt,_texture2D);
        // _texture2D.Apply();
        //rd.material.SetTexture(MainTex,_texture2D);
    }
}
