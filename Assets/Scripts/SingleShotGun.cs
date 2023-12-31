using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    [SerializeField] TMP_Text ammoCount;
    [SerializeField] TMP_Text totalCount;
    [SerializeField] AudioSource shootingAudioSource;
    [SerializeField] ParticleSystem system;
    [SerializeField] AudioSource playerHitSource;

    int ammoCounter;
    int totalCounter;
    int changeCounter;

    void Awake()
    {
        PV = GetComponent<PhotonView>();

        ((GunInfo)itemInfo).totalCount = ((GunInfo)itemInfo).totalCapacity;
        ((GunInfo)itemInfo).ammoCount = ((GunInfo)itemInfo).magCapacity;

        ammoCount.text = ((GunInfo)itemInfo).ammoCount.ToString();
        totalCount.text = ((GunInfo)itemInfo).totalCount.ToString();
    }

    public override void Use()
    {
        if(Cursor.lockState == CursorLockMode.Locked)
        {
            Shoot();
        }
    }

    public override void Reload()
    {
        if(((GunInfo)itemInfo).ammoCount == ((GunInfo)itemInfo).magCapacity)
        {
            return;
        }

        if(((GunInfo)itemInfo).totalCount == 0)
        {
            return;
        }

        audioConfig.PlayReloadClip(shootingAudioSource);
        Invoke("ReloadGun", 2f);
    }

    void ReloadGun()
    {
        int.TryParse(ammoCount.text, out ammoCounter);
        int.TryParse(totalCount.text, out totalCounter);

        if (ammoCounter > 0)
        {        
            //if totalCount is less than magCapacity, then we cannot reload fully, so reload what we have left
            if (totalCounter < ((GunInfo)itemInfo).magCapacity)
            {
                ((GunInfo)itemInfo).ammoCount = totalCounter;
                ammoCount.text = totalCounter.ToString();

                //totalCounter gets set to 0
                totalCounter = 0;
                ((GunInfo)itemInfo).totalCount = totalCounter;
                totalCount.text = totalCounter.ToString();
                return;
            }
            else
            {
                //to correctly decrease the totalCount ammo based on what we have left
                changeCounter = ((GunInfo)itemInfo).magCapacity - ammoCounter;
                totalCounter -= changeCounter;
                ((GunInfo)itemInfo).totalCount = totalCounter;
                totalCount.text = totalCounter.ToString();

                //if totalCount is greater than magCapacity, let ammoCount be equal to magCapacity
                ((GunInfo)itemInfo).ammoCount = ((GunInfo)itemInfo).magCapacity;
                ammoCounter = ((GunInfo)itemInfo).magCapacity;
                ammoCount.text = ammoCounter.ToString();
            }
        }
        else if (ammoCounter == 0)
        {
            //if totalCount is less than magCapacity, then we cannot reload fully, so reload what we have left
            if (totalCounter < ((GunInfo)itemInfo).magCapacity)
            {
                ((GunInfo)itemInfo).ammoCount = totalCounter;
                ammoCount.text = totalCounter.ToString();

                //totalCounter gets set to 0
                totalCounter = 0;
                ((GunInfo)itemInfo).totalCount = totalCounter;
                totalCount.text = totalCounter.ToString();
                return;
            }
            else
            {
                //decrease totalCounter by one magCapacity and display
                totalCounter -= ((GunInfo)itemInfo).magCapacity;
                ((GunInfo)itemInfo).totalCount = totalCounter;
                totalCount.text = totalCounter.ToString();

                //if totalCount is greater than magCapacity, let ammoCount be equal to magCapacity
                ((GunInfo)itemInfo).ammoCount = ((GunInfo)itemInfo).magCapacity;
                ammoCounter = ((GunInfo)itemInfo).magCapacity;
                ammoCount.text = ammoCounter.ToString();
            }
        }
    }

    public override void OutOfAmmo()
    {
        audioConfig.PlayOutOfAmmoClip(shootingAudioSource);
    }

    void Shoot()
    {
        int.TryParse(ammoCount.text, out ammoCounter);
        int.TryParse(totalCount.text, out totalCounter);


        //if you have ammo, then shoot
        if (ammoCounter > 0)
        {
            ammoCounter = ammoCounter - 1;
            ((GunInfo)itemInfo).ammoCount = ammoCounter;
            ammoCount.text = ammoCounter.ToString();
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            audioConfig.PlayShootingClip(shootingAudioSource, ((GunInfo)itemInfo).ammoCount == 1);
            hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(((GunInfo)itemInfo).damage);

            hit.collider.gameObject.GetComponent<IDamagable>()?.PlayShotClip(playerHitSource);

            PV.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);
            system.Play();
        }
    }


    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}
