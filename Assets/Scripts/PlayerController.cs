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

public class PlayerController : MonoBehaviourPunCallbacks, IDamagable
{
    //Variables Linked to Movement
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float speed = 12f, gravity = -9.81f, jumpHeight = 2f, groundDistance = 0.4f, mouseSensitivity = 100f;

    [SerializeField] CharacterController controller;

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;

    Vector3 velocity;
    bool grounded;
    float verticalLookRotation;

    //Variables Linked to Gun and More
    [SerializeField] Gun[] items;

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
    [SerializeField] TMP_Text totalCount;
    [SerializeField] Camera[] gunCameras;

    [SerializeField]float fireRate = 0.25f;
    float nextFireTime;

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

        if (PV.IsMine)
        {
            EquipItem(0);
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
            items[itemIndex].Reload();
        }
    }

    void CheckClick()
    {
        if (items[itemIndex].gameObject.CompareTag("SingleShotGun"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (((GunInfo)items[itemIndex].itemInfo).ammoCount != 0)
                {
                    items[itemIndex].Use();
                    Debug.Log("Mouse Button Up");
                }
            }
        }


        if (items[itemIndex].gameObject.CompareTag("SprayGun"))
        {
            if (Time.time > nextFireTime)
            {
                if (Input.GetMouseButton(0))
                {
                    if (((GunInfo)items[itemIndex].itemInfo).ammoCount != 0)
                    {
                        items[itemIndex].Use();
                        Debug.Log("Spraying");

                        nextFireTime = Time.time + fireRate;
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

        Camera camera = items[itemIndex].itemGameObject.GetComponentInChildren<Camera>();

        ammoCount.text = ((GunInfo)items[itemIndex].itemInfo).ammoCount.ToString();
        totalCount.text = ((GunInfo)items[itemIndex].itemInfo).totalCount.ToString();

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
        Debug.Log(PV.Owner.NickName + "Took damage: " + damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
