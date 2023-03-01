using UnityEngine;
using UnityEngine.UI;

public class WarningPanel : MonoBehaviour
{
    public Text warningText;

    public void ActionClose()
    {
        Destroy(gameObject);
    }
}
