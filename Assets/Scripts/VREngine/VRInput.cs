using UnityEngine;
using System;

namespace VREngine.Core
{
    public class VRInput : MonoBehaviour
    {
        public Action OnClick { get; internal set; }
        public Action OnDoubleClick { get; internal set; }
        public Action OnDown { get; internal set; }
        public Action OnUp { get; internal set; }
    }
}
