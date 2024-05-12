using System;

namespace ScriptsBox.存档系统
{
    [Serializable]
    public class SaveData
    {
        public int dataVersion = 1;

        public ASerializedObject aObj = new ASerializedObject()
        {
            id = 1,
            name = "Mike",
        };
    }
}