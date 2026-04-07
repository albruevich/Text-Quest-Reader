using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PictureNode : MonoBehaviour
{
    [SerializeField] private Image outerPicture;
    [SerializeField] private Image innerPicture;
    //[SerializeField] private Sprite startSprite;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        InitializePictures();
    }

    public void InitializePictures()
    {
        outerPicture.color = Color.white;
        innerPicture.color = Color.white;

        //outerPicture.sprite = startSprite;
        //innerPicture.sprite = startSprite;   
    }

    public void SetNewPicture(string pictureName)
    {
        if (innerPicture.sprite != null && innerPicture.sprite.name == pictureName)
            return;

        string path = $"Quests/{SaveLoadManager.QuestFolderName}/Images/{pictureName}";

        var sprite = Resources.Load<Sprite>(path);

        if (sprite != null)        
            innerPicture.sprite = sprite;                  
        else
            Debug.LogWarning($"Sprite not found: {path}");        

        if(animator)
            animator.Play("FadePictures");
    }

    public void Callback()
    {
        outerPicture.sprite = innerPicture.sprite;
    }
}