﻿using UnityEngine;
using System;

namespace VREngine.Core
{
    public class VRInput : MonoBehaviour
    {
        public enum SwipeDirection
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT
        };

        public event Action<SwipeDirection> OnSwipe;
        public event Action OnClick;                 // Called when Fire1 is released and it's not a double click.
        public event Action OnDoubleClick;           // Called when a double click is detected.
        public event Action OnDown;                  // Called when Fire1 is pressed.
        public event Action OnUp;                    // Called when Fire1 is released.
        public event Action OnCancel;                // Called when Cancel is pressed.

        [SerializeField]
        private float m_DoubleClickTime = 0.3f;      // The max time allowed between double clicks.
        [SerializeField]
        private float m_SwipeWidth = 0.3f;           // The width of a swipe.

        private Vector2 m_MouseDownPosition;         // The screen position of the mouse when Fire1 is pressed.
        private Vector2 m_MouseUpPosition;           // The screen position of the mouse when Fire1 is released.
        private float m_LastMouseUpTime;             // The time when Fire1 was last released. 
        private float m_LastHorizontalValue;         // The previous value of the horizontal axis used to detect keyboard swipes.
        private float m_LastVerticalValue;           // The previous value of the vertical axis used to detect keyboard swipes.

        public float DoubleClickTime { get { return m_DoubleClickTime; } }

        private void Update()
        {
            CheckInput();
        }

        private void CheckInput()
        {
            // Set the default swipe to be NONE.
            SwipeDirection swipe = SwipeDirection.NONE;

            if (Input.GetButtonDown("Fire1"))
            {
                // When Fire1 is pressed record the position of the mouse.
                m_MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                // If anything is subscribed to OnDown call it.
                if (OnDown != null)
                {
                    OnDown();
                }
            }

            // Gather information about the mouse when the button is up.
            if (Input.GetButtonUp("Fire1"))
            {
                // When Fire1 is released record the position of the mouse.
                m_MouseUpPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                // Detect the direction between mouse position when Fire1 is pressed and released.
                swipe = DetectSwipe();
            }

            // If there was no swipe from the mouse (this frame), check for a keyboard swipe.
            if (swipe == SwipeDirection.NONE)
            {
                swipe = DetectKeyboardEmulatedSwipe();
            }

            // If there are any subscribers to OnSwipe call it passing in the detected swipe.
            if (OnSwipe != null)
            {
                OnSwipe(swipe);
            }

            // This if statement is to trigger events based on the information gathered before.
            if (Input.GetButtonUp("Fire1"))
            {
                // If anything has subscribed to OnUp call it.
                if (OnUp != null)
                {
                    OnUp();
                }

                // If the time between the last release of Fire1 and now is less than
                // the allowed double click time then it's a double click.
                if (Time.time - m_LastMouseUpTime < m_DoubleClickTime)
                {
                    // If anything has subscribed to OnDoubleClick call it.
                    if (OnDoubleClick != null)
                    {
                        OnDoubleClick();
                    }
                } else {
                    // If it's not a double click, it's a single click.
                    // If anything has subscribed to OnClick call it.
                    if (OnClick != null)
                    {
                        OnClick();
                    }
                }

                // Record the time when Fire1 is released.
                m_LastMouseUpTime = Time.time;
            }

            // If the Cancel button is pressed.
            if (Input.GetButtonDown("Cancel"))
            {
                // If anything has subscribed to OnCancel call it.
                if (OnCancel != null)
                {
                    OnCancel();
                }
            }
        }

        private SwipeDirection DetectSwipe()
        {
            // Get direction from mouse when Fire1 is pressed to when it's released.
            Vector2 swipeData = (m_MouseUpPosition - m_MouseDownPosition).normalized;

            // If the direction of the swipe has a small width it's vertical.
            bool swipeIsVertical = Mathf.Abs(swipeData.x) < m_SwipeWidth;

            // If the direction of the swipe has a small height it's horizontal.
            bool swipeIsHorizontal = Mathf.Abs(swipeData.y) < m_SwipeWidth;

            // If the swipe has a positive y component and is vertical the swipe is UP.
            if (swipeData.y > 0f && swipeIsVertical)
            {
                return SwipeDirection.UP;
            }

            // If the swipe has a negative y component and is vertical the swipe is DOWN.
            if (swipeData.y < 0f && swipeIsVertical)
            {
                return SwipeDirection.DOWN;
            }

            // If the swipe has a positive x component and is horizontal the swipe is RIGHT.
            if (swipeData.x > 0f && swipeIsHorizontal)
            {
                return SwipeDirection.RIGHT;
            }

            // If the swipe has a negative x component and is horizontal the swipe is LEFT.
            if (swipeData.x < 0f && swipeIsHorizontal)
            {
                return SwipeDirection.LEFT;
            }

            // There is no swipe.
            return SwipeDirection.NONE;
        }

        private SwipeDirection DetectKeyboardEmulatedSwipe()
        {
            // Store the values for Horizontal and Vertical axes.
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Store whether there was horizontal or vertical input before.
            bool noHorizontalInputPreviously = Mathf.Abs(m_LastHorizontalValue) < float.Epsilon;
            bool noVerticalInputPreviously = Mathf.Abs(m_LastVerticalValue) < float.Epsilon;

            // The last horizontal values are now the current ones.
            m_LastHorizontalValue = horizontal;
            m_LastVerticalValue = vertical;

            // If there is positive vertical input now and previously there wasn't the swipe is UP.
            if (vertical > 0f && noVerticalInputPreviously)
            {
                return SwipeDirection.UP;
            }

            // If there is negative vertical input now and previously there wasn't the swipe is DOWN.
            if (vertical < 0f && noVerticalInputPreviously)
            {
                return SwipeDirection.DOWN;
            }

            // If there is positive horizontal input now and previously there wasn't the swipe is RIGHT.
            if (horizontal > 0f && noHorizontalInputPreviously)
            {
                return SwipeDirection.RIGHT;
            }

            // If there is negative horizontal input now and previously there wasn't the swipe is LEFT.
            if (horizontal < 0f && noHorizontalInputPreviously)
            {
                return SwipeDirection.LEFT;
            }

            // There is no swipe.
            return SwipeDirection.NONE;
        }

        private void OnDestroy()
        {
            // Ensure that all events are unsubscribed when this is destroyed.
            OnSwipe = null;
            OnClick = null;
            OnDoubleClick = null;
            OnDown = null;
            OnUp = null;
        }
    }
}
