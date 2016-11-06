using UnityEngine;
using System;

namespace VREngine.Core
{
    public class VRInteractiveItem : MonoBehaviour
    {
        public event Action OnOver;        // Called when the gaze moves over this object.
        public event Action OnOut;         // Called when the gaze leaves this object.
        public event Action OnClick;       // Called when click input is detected while the gaze is over this object.
        public event Action OnDoubleClick; // Called when double click input is detected while the gaze is over the object.
        public event Action OnUp;          // Called when Fire1 is released whilst the gaze is over this object.
        public event Action OnDown;        // Called when Fire1 is pressed whilst the gaze is over this object.

        protected bool m_IsOver;

        public bool IsOver
        {
            get { return m_IsOver; } // Is the gaze currently over this object?
        }

        // The below methods are called by VREyeRayCaster when the appropriate input is detected.
        public void Out()
        {
            m_IsOver = false;

            if (OnOut != null)
            {
                OnOut();
            }
        }

        public void Down()
        {
           if (OnDown != null)
            {
                OnDown();
            }
        }

        public void Up()
        {
            if (OnUp != null)
            {
                OnUp();
            }
        }

        public void Click()
        {
            if (OnClick != null)
            {
                OnClick();
            }
        }

        public void DoubleClick()
        {
            if (OnDoubleClick != null)
            {
                OnDoubleClick();
            }
        }

        public void Over()
        {
            m_IsOver = true;

            if (OnOver != null)
            {
                OnOver();
            }
        }
    }
}
