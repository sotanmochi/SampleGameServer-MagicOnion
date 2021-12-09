using UnityEngine;

namespace SampleGame.Utility
{
    public static class KeyCodeExtension
    {
        public static bool IsPressing(this KeyCode self)
        {
            return KeyInput.GetKey(self);
        }

        public static bool IsPressed(this KeyCode self)
        {
            return KeyInput.GetKeyDown(self);
        }
    }

    public static class KeyInput
    {
        public static bool _enable = true;

        public static void SetEnable(bool enable)
        {
            _enable = enable;
        }

        public static bool AnyKey()
        {
            return _enable && Input.anyKey;
        }

        public static bool AnyKeyDown()
        {
            return _enable && Input.anyKeyDown;
        }

        public static bool GetKey(KeyCode key)
        {
            return _enable && Input.GetKey(key);
        }

        public static bool GetKeyDown(KeyCode key)
        {
            return _enable && Input.GetKeyDown(key);
        }
    }
}