using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;
using Fergicide;
using System;

public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    //Variables Linked to Movement
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float speed = 12f, gravity = -9.81f, jumpHeight = 2f, groundDistance = 0.4f, mouseSensitivity = 4f;

    [SerializeField] CharacterController controller;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    Vector3 velocity;
    bool grounded;
    float verticalLookRotation;

    //Variables Linked to Gun and More
    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    //Variables needed for Photon
    Rigidbody rb;
    PhotonView PV;

    //Constants
    const float maxHealth = 150f;

    float currentHealth = maxHealth;
    PlayerManager playerManager;

    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] TMP_Text ammoCount;
    [SerializeField] TMP_Text slash;
    [SerializeField] TMP_Text totalCount;
    [SerializeField] Camera[] gunCameras;

    [SerializeField] float fireRate = 0.25f;
    [SerializeField] float pistolFireRate = 1f;

    float nextFireTime;
    float pistolNextFireTime;
    string myName;

    [SerializeField] PlayAudioEffect hitEffect;
    [SerializeField] Image[] icons;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        nextFireTime = Time.time;
        pistolNextFireTime = Time.time;

        if (PV.IsMine)
        {
            EquipItem(0);
            myName = PV.Owner.NickName;
            Debug.Log("My name: " + myName);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);

            foreach(Camera cam in gunCameras)
            {
                cam.gameObject.SetActive(false);
            }
        }
    }


    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        MoveAndJump();
        Look();

        EquipItemWithKeypad();
        EquipItemWithScrollWheel();

        CheckClick();
        CheckReload();
    }

    void CheckReload()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                if (items[itemIndex] is Gun gun)
                {
                    gun.Reload();
                }
            }
        }
    }

    void CheckClick()
    {
        if (items[itemIndex].gameObject.CompareTag("Knife"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                items[itemIndex].Use();
            }

            if (Input.GetMouseButtonDown(1))
            {
                items[itemIndex].Use();
            }
        }

        if (items[itemIndex].gameObject.CompareTag("SingleShotGun"))
        {
            if (Time.time > pistolNextFireTime)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (((GunInfo)items[itemIndex].itemInfo).ammoCount > 0)
                    {
                        items[itemIndex].Use();

                        pistolNextFireTime = Time.time + pistolFireRate;
                    }
                }

                if (Input.GetMouseButtonUp(0) && ((GunInfo)items[itemIndex].itemInfo).ammoCount == 0)
                {
                    if (items[itemIndex] is Gun gun)
                    {
                        gun.OutOfAmmo();
                    }
                }
            }   
        }


        if (items[itemIndex].gameObject.CompareTag("SprayGun"))
        {
            if (Time.time > nextFireTime)
            {
                if (Input.GetMouseButton(0))
                {
                    if (((GunInfo)items[itemIndex].itemInfo).ammoCount > 0)
                    {
                        items[itemIndex].Use();
                        Debug.Log("Spraying");

                        nextFireTime = Time.time + fireRate;
                    }
                }

                if(Input.GetMouseButtonDown(0) && ((GunInfo)items[itemIndex].itemInfo).ammoCount == 0)
                {
                    if (items[itemIndex] is Gun gun)
                    {
                        gun.OutOfAmmo();
                    }
                }
            }
        }      
    }


    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void MoveAndJump()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void EquipItemWithScrollWheel()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }
    }

    void EquipItemWithKeypad()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;
        }

        itemIndex = _index;

        if (PV.IsMine)
        {
            if (itemIndex >= 0 && itemIndex < icons.Length)
            {
                // Deactivate all icons
                for (int i = 0; i < icons.Length; i++)
                {
                    icons[i].gameObject.SetActive(false);
                }

                // Activate the selected icon
                icons[itemIndex].gameObject.SetActive(true);
            }
        }

        if (itemIndex == 0 || itemIndex == 1)
        {
            //change ammo count if gun is switched
            ammoCount.text = ((GunInfo)items[itemIndex].itemInfo).ammoCount.ToString();
            totalCount.text = ((GunInfo)items[itemIndex].itemInfo).totalCount.ToString();
            slash.text = "/";
        }
        else
        {
            ammoCount.text = "";
            totalCount.text = "";
            slash.text = "";
        }
        

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    public void PlayShotClip(AudioSource audioSource)
    {
        hitEffect.PlayShotClip(audioSource);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            int gunUsedIndex = (int)info.Sender.CustomProperties["itemIndex"];
            //you die sending the killers name with you
            Die(gunUsedIndex,info.Sender.NickName);
            //the killer gets a kill, sending who they killed
            PlayerManager.Find(info.Sender).GetKill(myName, gunUsedIndex);
        }
    }

    void Die(int index, string killer)
    {
        playerManager.Die(index, killer);
    }

    public float GetMouseSensitivity()
    {
        return mouseSensitivity;
    }

    public void SetMouseSensitivity(float mouseSens)
    {
        mouseSensitivity = mouseSens;
    }
}
