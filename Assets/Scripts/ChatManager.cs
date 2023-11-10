using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] TMP_Text chatContent;
    [SerializeField] GameObject chatMenu;
    [SerializeField] RectTransform chatMenuRectTransform;

    PhotonView PV;

    List<string> messages = new List<string>();

    float buildDelay = 0f;
    int maxMessages = 14;
    string username;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    [PunRPC]
    void RPC_AddNewMessage(string msg)
    {
        messages.Add(msg);
    }

    public void SendChat(string msg)
    {
        username = "<#C9C0FF>" + PhotonNetwork.NickName + "</color>";
        string message = username + ": " + msg;
        PV.RPC(nameof(RPC_AddNewMessage), RpcTarget.All, message);
    }

    public void SubmitChat()
    {
        string blankCheck = chatInput.text;
        blankCheck = Regex.Replace(blankCheck, @"\s", "");

        if(blankCheck == "")
        {
            chatInput.ActivateInputField();
            chatInput.text = "";
            return;
        }

        SendChat(chatInput.text);
        chatInput.ActivateInputField();
        chatInput.text = "";
    }

    void BuildChatContent()
    {
        string builtUpMessage = "";

        foreach(string s in messages)
        {
            builtUpMessage += s + "\n";
        }

        chatContent.text = builtUpMessage;
    }

    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (!chatMenu.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Cursor.lockState = CursorLockMode.None;
                    chatMenu.SetActive(true);
                }
            }

            if (chatMenu.activeSelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePosition = Input.mousePosition;

                    // Check if the mouse click is inside the ChatMenu RectTransform
                    if (RectTransformUtility.RectangleContainsScreenPoint(chatMenuRectTransform, mousePosition))
                    {
                        // Clicked inside the ChatMenu, do nothing
                        return;
                    }
                    else
                    {
                        // Clicked outside the ChatMenu, lock the cursor
                        Cursor.lockState = CursorLockMode.Locked;
                        chatMenu.SetActive(false);
                    }
                }
            }

            if (messages.Count > maxMessages)
            {
                messages.RemoveAt(0);
            }

            if (buildDelay < Time.time)
            {
                BuildChatContent();
                buildDelay = Time.time + 0.25f;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitChat();
            }
        }
        else if(messages.Count > 0)
        {
            messages.Clear();
            chatContent.text = "";
        }
    }
}
