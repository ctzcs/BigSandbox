using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ScriptsBox.序列化.Yaml
{
    public class Yaml : MonoBehaviour
    {
        string path = Application.dataPath + "/ScriptsBox/序列化/Yaml/";
        private string fileName = "save.yaml";
        
        [SerializeField] private PlayerData _data;
        

        [Button("序列化")]
        void Serialize()
        {
            var serialize = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            string text = serialize.Serialize(_data);
            /*using (StreamWriter writer = new StreamWriter(path + fileName, false, Encoding.UTF8))
            {
                writer.Write(text);
            }*/
            
            File.WriteAllText(Application.dataPath+path,text);
        }

        [Button("反序列化")]
        bool Deserialize()
        {
            if (File.Exists(path+fileName))
            {
                string text = File.ReadAllText(path+fileName);
                Debug.Log(text);
                _data = new Deserializer().Deserialize<PlayerData>(text);
                return true;
            }

            return false;
        }
    }

    [Serializable,YamlSerializable]
    class PlayerData
    {
        public string name;
        public List<SubClass> subClasses;
    }

    [Serializable,YamlSerializable]
    class SubClass
    {
        [YamlMember,SerializeField]
        public string name;

        public SubClass(){}
        public SubClass(string n)
        {
            name = n;
        }
    }
}
