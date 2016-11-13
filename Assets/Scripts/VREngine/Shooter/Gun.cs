using UnityEngine;
using System.Collections;
using VREngine.Core;

namespace VREngine.Shooter
{
    public class Gun : MonoBehaviour
    {
        [SerializeField]
        private float m_DefaultLineLength = 70f;         // How far the line renderer will reach if a target isn't hit.

        [SerializeField]
        private float m_Damping = 0.5f;                  // The damping with which this gameobject follows the camera.

        [SerializeField]
        private float m_GunContainerSmoothing = 10f;     // How fast the gun arm follows the reticle.

        [SerializeField]
        private float m_GunFlareVisibleSeconds = 0.07f;  // How long the line renederer and flare are visible for each shot.

        [SerializeField]
        private VRReticle m_Reticle;                     // What the gun arm should be aiming at.

        [SerializeField]
        private Transform m_CameraTransform;             // Used as a reference to move this gameobject towards.

        [SerializeField]
        private Transform m_GunContainer;

        [SerializeField]
        private Transform m_GunEnd;

        [SerializeField]
        private LineRenderer m_GunFlare;

        [SerializeField]
        private VRInput m_VRInput;

        private const float k_DampingCoef = -20f;       // This is the coefficient used to ensure smooth damping of this gameobject.

        private void OnEnable()
        {
            m_VRInput.OnDown += HandleDown;
        }

        private void OnDisable()
        {
            m_VRInput.OnDown -= HandleDown;
        }

        private void HandleDown()
        {
            StartCoroutine(Fire(null));
        }

        void Update()
        {
            // Smoothly interpolate this gameobject's roattion towards that of the user/camera
            transform.rotation = Quaternion.Slerp(transform.rotation, m_CameraTransform.rotation,
                m_Damping * (1 - Mathf.Exp(k_DampingCoef * Time.deltaTime)));

            // Move this gameobject with the camera.
            transform.position = m_CameraTransform.position;

            // Find a rotation for the gun to be pointed at the reticle;
            Quaternion lookAtRotation = Quaternion.LookRotation(m_Reticle.ReticleTransform.position - m_GunContainer.position);

            // Smoothly interpolate the gun's rotation towards the desired rotation.
            m_GunContainer.rotation = Quaternion.Slerp(m_GunContainer.rotation, lookAtRotation,
                m_GunContainerSmoothing * Time.deltaTime);
        }

        private IEnumerator Fire(Transform target)
        {
            // Set the length of the line renderer to the default value.
            float lineLength = m_DefaultLineLength;

            // If there is a target, the line renderer's length is instead the distance from
            // the gun to the target.
            if (target)
            {
                lineLength = Vector3.Distance(m_GunEnd.position, target.position);
            }

            // Turn the line renderer on.
            m_GunFlare.enabled = true;

            yield return StartCoroutine(MoveLineRenderer(lineLength));

            // Turn the line renderer off.
            m_GunFlare.enabled = false;
        }

        private IEnumerator MoveLineRenderer(float lineLength)
        {
            // Create a timer.
            float timer = 0f;

            while (timer < m_GunFlareVisibleSeconds)
            {
                // ... set the line renderer to start at the gun and finish at the target.
                m_GunFlare.SetPosition(0, m_GunEnd.position);
                m_GunFlare.SetPosition(1, m_GunEnd.position + m_GunEnd.forward * lineLength);

                // Wait for next frame.
                yield return null;

                // Increment the timer by the amount of time waited.
                timer += Time.deltaTime;
            }     
        }
    }
}
