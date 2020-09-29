using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnim : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void OptionsEnter()
    {
        anim.SetBool("Options", true);
    }
    public void OptionsExit()
    {
        anim.SetBool("Options", false);
    }

}
