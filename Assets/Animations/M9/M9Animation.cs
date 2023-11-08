using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M9Animation : MonoBehaviour
{
    Animator animator;
    [SerializeField] PhotonView PV;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (PV.IsMine)
            {
                if (Input.GetMouseButton(0))
                {
                    animator.SetTrigger("LeftClick");
                }

                if (Input.GetKeyDown(KeyCode.Y))
                {
                    animator.SetTrigger("Inspect");
                }


                if (Input.GetMouseButton(1))
                {
                    animator.SetTrigger("RightClick");
                }
            }
        }
    }
}
