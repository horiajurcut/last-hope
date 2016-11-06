using UnityEngine;
using System;

namespace VREngine.Core
{
    public class VREyeRayCaster : MonoBehaviour
    {
        public event Action<RaycastHit> OnRayCastHit;

        [SerializeField]
        private Transform m_Camera;

        [SerializeField]
        private LayerMask m_ExclusionLayers;

        [SerializeField]
        private Reticle m_Reticle; // The reticle, if applicable.

        [SerializeField]
        private VRInput m_VrInput; // Call input based events on the current VRInteractiveItem.

        [SerializeField]
        private bool m_ShowDebugRay; // Show debug ray.

        [SerializeField]
        private float m_DebugRayLength = 5f; // Debug ray length.

        [SerializeField]
        private float m_DebugRayDuration = 1f; // How long the debug ray will remain visible.

        [SerializeField]
        private float m_RayLength = 500f; // How far into the scene the ray is cast.

        private VRInteractiveItem m_CurrentInteractible;
        private VRInteractiveItem m_LastInteractible;

        // Utility for other classes to get the current interactive item.
        public VRInteractiveItem CurrentInteractible
        {
            get { return m_CurrentInteractible; }
        }

        // Called when the object becomes enabled and active
        private void OnEnable()
        {
            m_VrInput.OnClick += HandleClick;
            m_VrInput.OnDoubleClick += HandleDoubleClick;
            m_VrInput.OnUp += HandleUp;
            m_VrInput.OnDown += HandleDown;
        }

        // Called when the object is destroyed
        private void OnDisable()
        {
            m_VrInput.OnClick -= HandleClick;
            m_VrInput.OnDoubleClick -= HandleDoubleClick;
            m_VrInput.OnUp -= HandleUp;
            m_VrInput.OnDown -= HandleDown;
        }

        private void Update()
        {
            EyeRayCast();
        }

        private void EyeRayCast()
        {
            // Draw debug raw if required.
            if (m_ShowDebugRay)
            {
                Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
            }

            // Create a ray that points forward from the camera.
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            RaycastHit hit;

            // Do the raycast forwards to see if we hit an interactible.
            if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers)) {
                // Attempt to retrieve the VRInteractiveItem on the hit object
                VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>();
                m_CurrentInteractible = interactible;

                // If we hit a different interactible than the last time
                if (interactible && interactible != m_LastInteractible) {
                    interactible.Over();
                }

                // Deactivate the last interactible
                if (interactible != m_LastInteractible)
                {
                    DeactivateLastInteractible();
                }

                m_LastInteractible = interactible;

                // Something was hit, set reticle at hit position
                if (m_Reticle)
                {
                    m_Reticle.SetPosition(hit);
                }

                if (OnRayCastHit != null)
                {
                    OnRayCastHit(hit);
                }
            } else {
                // Nothing was hit, deactivate the last interactible item.
                DeactivateLastInteractible();
                m_CurrentInteractible = null;

                // Position the reticle at default distance.
                if (m_Reticle)
                {
                    m_Reticle.SetPosition();
                }
            }
        }

        private void DeactivateLastInteractible()
        {
            if (m_LastInteractible == null)
            {
                return;
            }

            m_LastInteractible.Out();
            m_LastInteractible = null;
        }

        // Events
        private void HandleUp()
        {
            if (m_CurrentInteractible != null)
            {
                m_CurrentInteractible.Up();
            }
        }

        private void HandleDown()
        {
            if (m_CurrentInteractible != null)
            {
                m_CurrentInteractible.Down();
            }
        }

        private void HandleClick()
        {
            if (m_CurrentInteractible != null)
            {
                m_CurrentInteractible.Click();
            }
        }

        private void HandleDoubleClick()
        {
            if (m_CurrentInteractible != null)
            {
                m_CurrentInteractible.DoubleClick();
            }
        }
    }
}