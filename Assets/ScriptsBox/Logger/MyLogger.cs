using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MyLogger : MonoBehaviour
{
    private void Start()
    {
        Main();
    }

    void Logger(params string[] message)
    {
#if DEBUG
        Debug.Log(message.Length);
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < message.Length; i++)
        {
            builder.Append(message[i]);
        }
        Debug.Log(builder);//拼接移到了这里
#endif
    }

    void Main(){
        Logger("1,2");
    }
}
