using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace NomadRun.Core
{
    /// <summary>
    /// Unified input handler supporting both the modern Unity Input System
    /// (UnityEngine.InputSystem.Keyboard) and the legacy Input Manager (UnityEngine.Input).
    /// Prevents InvalidOperationException regardless of Player Settings.
    /// </summary>
    public static class NomadInput
    {
        public static bool GetUpPressed()
        {
#if ENABLE_INPUT_SYSTEM
            try
            {
                var kb = Keyboard.current;
                if (kb != null && (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame))
                    return true;
            }
            catch (System.Exception) {}
#endif

            try
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    return true;
            }
            catch (System.Exception) {}

            return false;
        }

        public static bool GetDownPressed()
        {
#if ENABLE_INPUT_SYSTEM
            try
            {
                var kb = Keyboard.current;
                if (kb != null && (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame))
                    return true;
            }
            catch (System.Exception) {}
#endif

            try
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    return true;
            }
            catch (System.Exception) {}

            return false;
        }

        public static bool GetUpHeld()
        {
#if ENABLE_INPUT_SYSTEM
            try
            {
                var kb = Keyboard.current;
                if (kb != null && (kb.wKey.isPressed || kb.upArrowKey.isPressed))
                    return true;
            }
            catch (System.Exception) {}
#endif

            try
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    return true;
            }
            catch (System.Exception) {}

            return false;
        }

        public static bool GetDownHeld()
        {
#if ENABLE_INPUT_SYSTEM
            try
            {
                var kb = Keyboard.current;
                if (kb != null && (kb.sKey.isPressed || kb.downArrowKey.isPressed))
                    return true;
            }
            catch (System.Exception) {}
#endif

            try
            {
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    return true;
            }
            catch (System.Exception) {}

            return false;
        }

        public static bool GetSpitPressed()
        {
#if ENABLE_INPUT_SYSTEM
            try
            {
                var kb = Keyboard.current;
                if (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.leftCtrlKey.wasPressedThisFrame || kb.rightCtrlKey.wasPressedThisFrame))
                    return true;
            }
            catch (System.Exception) {}
#endif

            try
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
                    return true;
            }
            catch (System.Exception) {}

            return false;
        }

        public static bool GetToastPressed()
        {
#if ENABLE_INPUT_SYSTEM
            try
            {
                var kb = Keyboard.current;
                if (kb != null && kb.fKey.wasPressedThisFrame)
                    return true;
            }
            catch (System.Exception) {}
#endif

            try
            {
                if (Input.GetKeyDown(KeyCode.F))
                    return true;
            }
            catch (System.Exception) {}

            return false;
        }

        public static bool GetRestartPressed()
        {
#if ENABLE_INPUT_SYSTEM
            try
            {
                var kb = Keyboard.current;
                if (kb != null && kb.rKey.wasPressedThisFrame)
                    return true;
            }
            catch (System.Exception) {}
#endif

            try
            {
                if (Input.GetKeyDown(KeyCode.R))
                    return true;
            }
            catch (System.Exception) {}

            return false;
        }
    }
}
