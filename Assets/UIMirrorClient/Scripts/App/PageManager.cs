using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    public enum Page
    {
        Connect,
        Canvas,
        Settings
    }

    [Serializable]
    public struct PageMapping
    {
        public Page Page;
        public GameObject PageObject;
    }

    [SerializeField]
    protected GameObject gearPage;

    public Page DefaultPage;
    public PageMapping[] Pages;

    public static PageManager Instance;

    private Dictionary<Page, GameObject> pageMap;

    private Page currentPage;

    public Page previousPage; 

    protected void Awake()
    {
        pageMap = Pages.ToDictionary(p => p.Page, p => p.PageObject);
        CurrentPage = DefaultPage;
        Instance = this;
    }

    public Page CurrentPage
    {
        get
        {
            return currentPage;
        }

        set
        {
            SetPage(value.ToString());
        }
    }

    public void SetPage(string page)
    {
        previousPage = currentPage;
        currentPage = (Page)Enum.Parse(typeof(Page), page);
        UpdateCurrentPage();
    }

    public void Back()
    {
        CurrentPage = previousPage;
    }

    private void UpdateCurrentPage()
    {
       foreach (var kvp in pageMap)
        {
            kvp.Value.SetActive(kvp.Key == currentPage);
        }

        gearPage.SetActive(currentPage != Page.Settings);
    }
}
