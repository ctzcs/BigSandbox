
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Windows;
using File = System.IO.File;

namespace ScriptsBox.存档系统
{
    public class SaveMgr
    {
        private static string Path = Application.dataPath + "/../Save/";

        public static SaveData data = new();
        public static void Save()
        {
            string text = JsonConvert.SerializeObject(data,Formatting.Indented);
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            File.WriteAllText(System.IO.Path.Combine(Path,"Save.sav"),text);
        }

        public static void Load()
        {
            string text = File.ReadAllText(System.IO.Path.Combine(Path, "Save.sav"));
            data = JsonConvert.DeserializeObject<SaveData>(text);
        }
    }
}
