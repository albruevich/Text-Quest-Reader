using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PictureNode : MonoBehaviour
{
    [SerializeField] private Image outerPicture;
    [SerializeField] private Image innerPicture;  

    private Animator animator;

    string lastPictureName;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ClearPicturesColor();
    }

    public void InitImages(string pictureName, string questName)
    {
        Sprite sprite = null;

        if (!string.IsNullOrEmpty(pictureName))
        {
            string path = $"Quests/{questName}/Images/{pictureName}";
            sprite = Resources.Load<Sprite>(path);
        }
     
        innerPicture.sprite = outerPicture.sprite = sprite;
    }

    public void ClearPicturesColor()
    {
        outerPicture.color = Color.white;
        innerPicture.color = Color.white;        
    }

    public void SetNewPicture(string pictureName, string questName)
    {
        if (lastPictureName == pictureName)           
            return;
        
        Sprite sprite = null;

        if (!string.IsNullOrEmpty(pictureName))
        {
            string path = $"Quests/{questName}/Images/{pictureName}";
            sprite = Resources.Load<Sprite>(path);
        }

        innerPicture.sprite = sprite;

        if (animator)       
            animator.Play("FadePictures");        

        lastPictureName = pictureName;
    }

    public void Callback()
    {
        outerPicture.sprite = innerPicture.sprite;
    }
}