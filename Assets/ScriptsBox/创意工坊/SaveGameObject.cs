
using System;
using System.IO;
using ScriptsBox.创意工坊;
using Sirenix.Serialization;
using UnityEngine;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

public class SaveGameObject : MonoBehaviour
{
    private GameObject _go;
    
    private static string Path = Application.dataPath + "/../Mod/";

    // Update is called once per frame
    void Update()
    {
        if (IsSavePressed())
        {
            SaveGo();
        }else if (IsLoadPressed())
        {
            LoadGo();
        }
    }

    bool IsSavePressed()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            return true;
        }

        return false;
    }

    bool IsLoadPressed()
    {
        if (Input.GetKeyUp(KeyCode.L))
        {
            return true;
        }

        return false;
    }

    void SaveGo()
    {
        
        byte[] json = SerializationUtility.SerializeValue(_go,DataFormat.JSON);
        
        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
        }
        
        File.WriteAllBytes(Path + "go.data",json);
    }

    void LoadGo()
    {
        if (_go != null)
        {
            Destroy(_go);
        }
        if (File.Exists(Path + "go.data"))
        {
            byte[] json = File.ReadAllBytes(Path + "go.data");
            _go = SerializationUtility.DeserializeValue<GameObject>(json,DataFormat.JSON);
        }
        else
        {
            _go = new GameObject();
            _go.AddComponent<ComEntity>().SetGuid(Guid.NewGuid());
            _go.AddComponent<ComPos>();
        }
        
    }
}
