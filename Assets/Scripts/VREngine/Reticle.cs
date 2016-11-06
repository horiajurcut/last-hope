using UnityEngine;
using UnityEngine.UI;

namespace VREngine.Core
{
    public class Reticle : MonoBehaviour
    {
        [SerializeField]
        private float m_DefaultDistance = 5f; // The default distance away from the camera.

        [SerializeField]
        private bool m_UseNormal; // Whether the reticle should be placed parallel to a surface.

        [SerializeField]
        private Image m_Image; // Reference to the image representing the reticle.

        [SerializeField]
        private Transform m_ReticleTransform; // We need to affect the reticle's transform.

        [SerializeField]
        private Transform m_Camera;

        private Vector3 m_OriginalScale; // Since the scale of the reticle can change, the original needs to be stored.
        private Quaternion m_OriginalRotation; // Stores the original rotation of the reticle.
        

        public bool UseNormal
        {
            get { return m_UseNormal; }
            set { m_UseNormal = value; }
        }

        public Transform ReticleTransform { get { return m_ReticleTransform; } }

        private void Awake()
        {
            // Store the original scale and rotation.
            m_OriginalScale = m_ReticleTransform.localScale;
            m_OriginalRotation = m_ReticleTransform.localRotation;
        }

        // Show reticle image on screen.
        public void Show()
        {
            m_Image.enabled = true;
        }

        // Hide reticle image on screen.
        public void Hide()
        {
            m_Image.enabled = false;
        }

        public void SetPosition(RaycastHit hit)
        {
            m_ReticleTransform.position = hit.point;
            m_ReticleTransform.localScale = m_OriginalScale * hit.distance;

            // If the reticle should use the normal of hit.
            if (m_UseNormal)
            {
                // Set rotation based on forward vector facing along the normal.
                m_ReticleTransform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            } else {
                // If not using the normal it's local rotation should be as it was originally.
                m_ReticleTransform.localRotation = m_OriginalRotation;
            }
        }

        // Used when the VREyeRayCaster hasn't hit anything.
        public void SetPosition()
        {
            // Set the position of the reticle to the default distance in front of the camera.
            m_ReticleTransform.position = m_Camera.position + m_Camera.forward * m_DefaultDistance;

            // Set the scale based on the original and the distance from the camera.
            m_ReticleTransform.localScale = m_OriginalScale * m_DefaultDistance;

            // The rotation should just be the default.
            m_ReticleTransform.localRotation = m_OriginalRotation;
        }
    }
}
