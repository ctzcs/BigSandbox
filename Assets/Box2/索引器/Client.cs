using System.Collections;
using System.Collections.Generic;
using Box2.索引器;
using Unity.VisualScripting;
using UnityEngine;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //这表明，普通的数组就已经可以访问，其实满足了大部分编写的条件
        //这里估计就是收集了数组里面的指针，然后通过+来获取下一个地址的位置。
        People[] peoples = new People[5]
        {
            new People(0),new People(1),new People(2),new People(3),new People(4),
        };
        Debug.Log(peoples[0]);
        
        //这种方式就是将开辟的内存进行封装，提供一些更方便的访问方式，也可能有回收方式。不过感觉不如list对象池
        PeopleArray peopleArray = new PeopleArray(2);
        peopleArray[0] = 0;
        Debug.Log(peopleArray[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
