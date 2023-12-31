using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AkAnimation : MonoBehaviour
{
    Animator animator;
    [SerializeField] ItemInfo itemInfo;
    [SerializeField] PhotonView PV;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            if (PV.IsMine)
            {
                if (((GunInfo)itemInfo).ammoCount > 0)
                {
                    if (Input.GetMouseButton(0))
                    {
                        animator.SetBool("Shoot", true);
                        Invoke("SetBoolBack", 0.2f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Y))
                {
                    animator.SetTrigger("Inspect");
                }

                if ((((GunInfo)itemInfo).ammoCount != ((GunInfo)itemInfo).magCapacity) &&
                    Input.GetKeyDown(KeyCode.R) && ((GunInfo)itemInfo).totalCount != 0)
                {
                    animator.SetBool("Reload", true);
                    Invoke("SetRelBoolBack", 2f);
                }
            }
        }
    }

    void SetBoolBack()
    {
        animator.SetBool("Shoot", false);
    }

    void SetRelBoolBack()
    {
        animator.SetBool("Reload", false);
    }
}
