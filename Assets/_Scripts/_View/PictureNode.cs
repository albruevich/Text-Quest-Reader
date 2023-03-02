using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PictureNode : MonoBehaviour
{
    public Image outerPicture, innerPicture;

    Animator animator;

    private void Start()
    {
        outerPicture.color = Color.black;
        innerPicture.color = Color.black;

        animator = GetComponent<Animator>();       
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
