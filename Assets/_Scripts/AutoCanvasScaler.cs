using UnityEngine;
using UnityEngine.UI;

public class AutoCanvasScaler : MonoBehaviour
{
    [HideInInspector]
    public float scaleFactor = 1f;

    private void Awake()
    {
        //DoScale();
    }  

    public void DoScale()
    {
        float devider = 2270f;

        //if (ReaderUtils.IsPad())
        //    devider = 1700f;
        //if (ReaderUtils.IsPhoneX() || ReaderUtils.IsAndroidTall())
        //    devider = 2622f;

        scaleFactor = Screen.height / devider;
        GetComponent<CanvasScaler>().scaleFactor = scaleFactor;              
    }
}
