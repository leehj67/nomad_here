using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public TextMeshProUGUI[] menuItems; // 메뉴 항목들의 TMP 배열
    public Color selectedColor = Color.yellow; // 선택된 항목의 색상
    public Color defaultColor = Color.white; // 기본 항목의 색상
    public GameObject[] pages; // 연결된 페이지들

    private int selectedIndex = 0;
    private Stack<int> pageHistory = new Stack<int>();
    private bool isMainPage = true;

    void Start()
    {
        UpdateMenu();
        DeactivateAllPages();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (isMainPage)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                UpdateMenu();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = (selectedIndex + 1) % menuItems.Length;
                UpdateMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                SelectItem();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoToPreviousPage();
            }
        }
    }

    void UpdateMenu()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            if (i == selectedIndex)
            {
                menuItems[i].color = selectedColor;
            }
            else
            {
                menuItems[i].color = defaultColor;
            }
        }
    }

    void SelectItem()
    {
        DeactivateAllPages();
        pages[selectedIndex].SetActive(true);
        pageHistory.Push(selectedIndex);
        isMainPage = false;
    }

    void DeactivateAllPages()
    {
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }
    }

    public void ActivateDefaultState()
    {
        DeactivateAllPages();
        isMainPage = true;
        selectedIndex = 0;
        UpdateMenu();
    }

    public void GoToPreviousPage()
    {
        if (pageHistory.Count > 0)
        {
            DeactivateAllPages();
            pageHistory.Pop();
            if (pageHistory.Count > 0)
            {
                int previousPageIndex = pageHistory.Peek();
                pages[previousPageIndex].SetActive(true);
                isMainPage = false;
            }
            else
            {
                isMainPage = true;
                UpdateMenu();
            }
        }
    }

    public bool IsMainPageActive()
    {
        return isMainPage;
    }
}
