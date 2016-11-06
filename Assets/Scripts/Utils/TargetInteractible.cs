using UnityEngine;
using System.Collections;
using VREngine.Core;

namespace VREngine.Utils
{
    public class TargetInteractible : MonoBehaviour
    {
        [SerializeField]
        private VRInteractiveItem m_InteractiveItem;
        [SerializeField]
        private Renderer m_Renderer;

        private void OnEnable()
        {
            m_InteractiveItem.OnClick += ChangeColor;
        }


        private void OnDisable()
        {
            m_InteractiveItem.OnClick -= ChangeColor;
        }

        private void ChangeColor()
        {
            if (m_InteractiveItem.IsOver) {
                m_Renderer.material.color = new Color(
                   Random.Range(0f, 1f),
                   Random.Range(0f, 1f),
                   Random.Range(0f, 1f)
               );
            }
        }
    }
}

