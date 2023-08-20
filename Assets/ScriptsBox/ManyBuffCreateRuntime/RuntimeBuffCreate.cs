using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeBuffCreate : MonoBehaviour
{
    public List<FakeBuff> _fakeBuffs;

    public int count;
    public int removeCount;
    public int maxCount;
    // Start is called before the first frame update
    void Start()
    {
        _fakeBuffs = new List<FakeBuff>(10000);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < count; i++)
            {
                var buff = new FakeBuff(1,2,3.2f);
                _fakeBuffs.Add(buff);
            }
        }

        if (Input.GetMouseButton(1))
        {
            for (int i = removeCount; i >= 0; i--)
            {
                RemoveBuff(i);
            }
            
        }
    }

    void RemoveBuff(int id)
    {
        if (id < _fakeBuffs.Count)
        {
            _fakeBuffs.RemoveAt(id);
        }
    }
}

[System.Serializable]
public class FakeBuff
{
    public int a;
    public int b;
    public float c;

    public FakeBuff(int a,int b,float c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
}
