using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PictureNode : MonoBehaviour
{
    public Image outerPicture, innerPicture;
    public Sprite startSprite;

    Animator animator;

    private void Start()
    {      
        animator = GetComponent<Animator>();

        StartPictures();
    }

    public void StartPictures()
    {
        outerPicture.color = Color.white;
        innerPicture.color = Color.white;       

        outerPicture.sprite = innerPicture.sprite = startSprite;
    }

    public void SetNewPicture(string pictureName)
    {       
        if (innerPicture.sprite && innerPicture.sprite.name == pictureName)
            return;

        innerPicture.sprite = Resources.Load<Sprite>(pictureName);
      
        animator.Play("FadePictures");
    }   

    public void Callback()
    {
        outerPicture.sprite = innerPicture.sprite;        
    }
}
