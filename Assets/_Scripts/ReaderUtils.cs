using UnityEngine;

public class ReaderUtils
{
    public static bool IsPad()
    {
        float res = (float)Screen.width / Screen.height;

        if ((res > 0.69f && res < 0.76f)
#if UNITY_IOS || UNITY_EDITOR
                || UnityEngine.iOS.Device.generation.ToString().Contains("iPad")
#endif
                )
        {
            return true;
        }
        else
            return false;
    }

    public static bool IsSquareTablet()
    {
        float res = (float)Screen.width / Screen.height;

        if (res > 0.76f && res < 0.99f)
        {
            return true;
        }
        else
            return false;
    }

    public static bool IsPhoneX()
    {
#if UNITY_IOS || UNITY_EDITOR       

        if (Screen.width / (float)Screen.height < 0.48f||
             UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX ||
             UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXR ||
             UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXS ||
             UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXSMax)
        {
            return true;
        }
        else
            return false;
#else
        return false;
#endif
    }

    public static bool IsAndroidTall()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        if (Screen.width / (float)Screen.height <= 0.5f)
        {
            //   Debug.Log("IsAndroidTall true");
            return true;
        }
        else
        {
            // Debug.Log("IsAndroidTall false");
            return false;
        }
#else
        return false;
#endif
    }

    public static bool IsPhone()
    {
        return !IsPad() && !IsSquareTablet();
    }

}