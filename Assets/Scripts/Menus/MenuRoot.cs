using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuRoot : MonoBehaviour
{
    public List<CanvasGroup> Menus = new List<CanvasGroup>();

    protected Transform menusRoot;
    protected Transform uiRoot;

    protected CanvasGroup currentMenu;
    protected CanvasGroup previousMenu;

    public bool isEnabled = true;

    void Start()
    {
        GetMenus();
    }

    protected void GetMenus()
    {
        uiRoot = transform.GetChild(0);
        menusRoot = uiRoot.Find("Menus");

        if (menusRoot)
        {
            for (int i = 0; i < menusRoot.childCount; i++)
            {
                CanvasGroup menu = menusRoot.GetChild(i).GetComponent<CanvasGroup>();

                if (menu)
                {
                    Menus.Add(menu);
                }
            }
        }

        if (Menus.Count > 0)
        {
            currentMenu = Menus[0];
        }

        ToggleThisMenu(isEnabled);
    }

    public void OpenMenu(int id)
    {
        if (id >= 0 && id < Menus.Count)
        {
            CanvasGroup menu = Menus[id];

            if (menu)
            {
                if (currentMenu)
                {
                    ToggleMenu(currentMenu, false);
                }

                ToggleMenu(menu, true);
            }
        }
    }

    public void CloseMenu(int id)
    {
        if (id >= 0 && id < Menus.Count)
        {
            CanvasGroup menu = Menus[id];

            if (menu)
            {
                ToggleMenu(menu, false);
            }
        }
    }

    public void PreviousMenu()
    {
        if (previousMenu)
        {
            CanvasGroup prev = previousMenu;

            ToggleMenu(currentMenu, false);
            ToggleMenu(prev, true);
        }
    }

    protected void ToggleMenu(CanvasGroup m, bool b)
    {
        m.interactable = b;
        m.blocksRaycasts = b;
        m.alpha = b ? 1 : 0;

        if (b)
        {
            currentMenu = m;
        }
        else
        {
            previousMenu = m;
        }
    }

    public void ToggleThisMenu(bool b)
    {
        uiRoot.gameObject.SetActive(b);
    }
}
