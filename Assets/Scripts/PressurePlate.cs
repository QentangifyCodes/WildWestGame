using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool open = false;

    public Animator anim;


    void OnCollisionEnter(Collision collision)
    {
        anim.SetTrigger("Open");
        open = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        anim.SetTrigger("Close");
        open = false;
    }
}
