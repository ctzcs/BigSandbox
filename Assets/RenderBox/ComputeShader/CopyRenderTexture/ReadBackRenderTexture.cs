
using UnityEngine;
using UnityEngine.Rendering;

public class ReadBackRenderTexture : MonoBehaviour
{
    public UnityEngine.ComputeShader cs;
    public Renderer useForRenderTexture;
    public Renderer useForTexture2d;
    private RenderTexture _rt;

    private Texture2D _texture2D;
    private int _groupSize = 8;
    private int _size = 64;
    private int _pixelCount;
    private int _rtBufferSize;
    private int _csKernel;
    private ComputeBuffer _renderTextureBuffer;
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
        _texture2D = new Texture2D(_size, _size, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point
        };
        _renderTextureBuffer = new ComputeBuffer(_pixelCount,_rtBufferSize);
        
    }

    private void OnDestroy()
    {
        _renderTextureBuffer.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AsyncGPUCompleteReadback()
    {
        cs.Dispatch(_csKernel,_pixelCount/_groupSize,_pixelCount/_groupSize,1);
        AsyncGPUReadback.Request(_renderTextureBuffer, _pixelCount * _rtBufferSize, 0, OnCompleteReadback_RenderTexture);
    }
    
    
    void OnCompleteReadback_RenderTexture(AsyncGPUReadbackRequest request)
    {
        if(request.hasError || _texture2D == null)return;
        //TODO 可以读Texture但是对线程组有要求，不能是一维线程组
        var data = request.GetData<Color32>();//request.GetData<Color32>();
        //MyLog.Log($"{num}");
        Graphics.CopyTexture(_rt,_texture2D);
        _texture2D.Apply();
        //rd.material.SetTexture(MainTex,_texture2D);
    }
}
