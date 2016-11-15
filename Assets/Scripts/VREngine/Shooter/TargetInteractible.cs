using UnityEngine;
using System.Collections;
using VREngine.Core;

namespace VREngine.Shooter
{
    public class TargetInteractible : MonoBehaviour
    {
        [SerializeField]
        private VRInteractiveItem m_InteractiveItem;

        [SerializeField]
        private Renderer m_Renderer;

        [SerializeField]
        private Animator enemyAnimator;

        private float movementSpeed = 1.3f;

        private void OnEnable()
        {
            m_InteractiveItem.OnClick += ChangeColor;
        }


        private void OnDisable()
        {
            m_InteractiveItem.OnClick -= ChangeColor;
        }

        private void Update()
        {
            if (enemyAnimator && !enemyAnimator.GetBool("isShot"))
            {
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            
        }

        private void ChangeColor()
        {
            if (m_InteractiveItem.IsOver) {
                if (m_Renderer)
                {
                    m_Renderer.material.color = new Color(
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f),
                        Random.Range(0f, 1f)
                    );
                }
                if (enemyAnimator)
                {
                    enemyAnimator.SetBool("isShot", true);
                }
            }
        }
    }
}

