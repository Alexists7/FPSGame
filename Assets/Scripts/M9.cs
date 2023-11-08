using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class M9 : Knife
{
    [SerializeField] AudioSource shootingAudioSource;
    [SerializeField] AudioSource playerHitSource;
    [SerializeField] Camera cam;
    public override void Use()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UseKnifeLeftClick();
            }

            if (Input.GetMouseButtonDown(1))
            {
                UseKnifeRightClick();
            }
        }
    }

    void UseKnifeLeftClick()
    {
        Debug.Log("LeftClick");

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        audioConfig.PlayLeftClickClip(shootingAudioSource);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(((KnifeInfo)itemInfo).leftClickDamage);
            hit.collider.gameObject.GetComponent<IDamagable>()?.PlayShotClip(playerHitSource);
        }
    }

    void UseKnifeRightClick()
    {
        Debug.Log("RightClick");

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        audioConfig.PlayLeftClickClip(shootingAudioSource);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);   
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(((KnifeInfo)itemInfo).rightClickDamage);
            hit.collider.gameObject.GetComponent<IDamagable>()?.PlayShotClip(playerHitSource);
        }
    }
}
