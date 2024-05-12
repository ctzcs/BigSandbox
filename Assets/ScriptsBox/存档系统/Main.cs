using UnityEngine;

namespace ScriptsBox.存档系统
{
    public class Main:MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveMgr.Save();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                SaveMgr.Load();
            }
        }
    }
}