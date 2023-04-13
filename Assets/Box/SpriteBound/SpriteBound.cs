using System;
using UnityEngine;

public class SpriteBound : MonoBehaviour
{
    private Sprite _sprite;
    private Vector2 _cameraMin;
    private Vector2 _cameraMax;
    public float paddingX;
    public float paddingY;
    public float speed;
    private float _inputX, _inputY;
    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
        //这里rect.size的256,bounds.extents是0.5
        Debug.Log($"border:{_sprite.border}\n" +
                  $"bounds:{_sprite.bounds.extents}\n" +
                  $"rect:{_sprite.rect.size}");

        _cameraMin = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        _cameraMax = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        var scale = transform.localScale;
        //关键问题是如何计算一个texture在世界空间中的真实大小。
        //Unity是以米为单位，比如这里默认256pixelsPerUnit，就是256像素是一米，所以这张图片的真实长宽是1*1Unit
        paddingX = _sprite.texture.width  * 0.5f * scale.x/_sprite.pixelsPerUnit;
        paddingY = _sprite.texture.height  * 0.5f * scale.y/_sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void Update()
    {
        _inputY = Input.GetAxisRaw("Vertical");
        _inputX = Input.GetAxisRaw("Horizontal");
        if (_inputX != 0 || _inputY != 0)
        {
            
            var pos = transform.position;
            pos.x = Math.Clamp(pos.x+_inputX*Time.deltaTime*speed, _cameraMin.x + paddingX, _cameraMax.x - paddingX); 
            pos.y = Math.Clamp(pos.y+_inputY*Time.deltaTime*speed, _cameraMin.y + paddingY, _cameraMax.y - paddingY);
            transform.position = pos;
        }
    }
}
