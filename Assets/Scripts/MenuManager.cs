using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] Menu[] menus;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        //Debug.Log("Running first function");
        for (int i = 0; i < menus.Length; i++)
        {
            //Debug.Log("entering for loop: " + i);
            if (menus[i].menuName == menuName)
            {
                //Debug.Log("same menu name: " + menuName);
                menus[i].Open();
            } 
            else if (menus[i].open) 
            {
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        } 
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
