using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void BrownToGreen()
    {
        anim.SetBool("Options", true);
    }
    public void GreenToBrown()
    {
        anim.SetBool("Options", false);
    }

}
