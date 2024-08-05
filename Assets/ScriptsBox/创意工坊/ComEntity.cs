using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptsBox.创意工坊
{
    public class ComEntity:MonoBehaviour
    {
        [SerializeField]
        private Guid _id;

        public void SetGuid(Guid id)
        {
            _id = id;
        }

        private List<MonoBehaviour> _comList = new();
        
        
        
    }
}