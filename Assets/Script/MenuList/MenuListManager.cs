using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuListManager :MonoBehaviour
{
    private static MenuListManager _instance;

    private const float MenuItemHeight = 80;
    
    private List<MenuListItem> _menuItems;
    private Stack<MenuListItem> _menuItemPool;

    private ScrollRect _scrollRect;
    private RectTransform _contents;
    
    public static MenuListManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var canvas = GameObject.Find("Canvas");
                canvas.transform.Find("PopUp/MenuList").gameObject.AddComponent<MenuListManager>();
            }
            return _instance;
        }
    }

    public void SetView(bool active)
    {
        gameObject.SetActive(active);
    }

    public void LoadSaveData(string fileName)
    {
        var saveData = FileManagement.LoadFile(fileName);
        for (var i = 0; i < saveData.Length; i++)
        {
            if (i < _menuItems.Count)
                _menuItems[i].Text = saveData[i];
            else
                AddMenuItem(saveData[i]);
        }

        while (DeleteMenuItem(saveData.Length));
    }

    public void SaveData(string fileName)
    {
        FileManagement.SaveFile(_menuItems, fileName);
    }

    private bool DeleteMenuItem(int index)
    {
        if (index >= _menuItems.Count) return false;
        var menuItem = _menuItems[index];
        menuItem.gameObject.SetActive(false);
        PoolObject(menuItem);
        _menuItems.RemoveAt(index);
        for (var i = index; i < _menuItems.Count; i++)
        {
            var position = _menuItems[i].transform.localPosition;
            position.y += MenuItemHeight;
            _menuItems[i].transform.localPosition = position;
        }

        UpdateContentsSize();

        return true;
    }
    
    public void DeleteMenuItem(MenuListItem item)
    {
        item.gameObject.SetActive(false);
        PoolObject(item);
        if (_menuItems.Contains(item))
        {
            var index = _menuItems.IndexOf(item);
            _menuItems.Remove(item);
            for (var i = index; i < _menuItems.Count; i++)
            {
                var position = _menuItems[i].transform.localPosition;
                position.y += MenuItemHeight;
                _menuItems[i].transform.localPosition = position;
            }
        }

        UpdateContentsSize();
    }

    private void AddMenuItem(string name = "")
    {
        var item = GetObject();
        if (_menuItems.Count > 0)
        {
            var position = _menuItems[_menuItems.Count - 1].transform.localPosition;
            position.y -= MenuItemHeight;
            item.transform.localPosition = position;
        }
        else
        {
            var position = item.transform.localPosition;
            position.y = 0;
            item.transform.localPosition = position;
        }
        
        item.gameObject.SetActive(true);
        item.Text = name;
        _menuItems.Add(item);

        UpdateContentsSize();
    }

    private void UpdateContentsSize()
    {
        _contents.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _menuItems.Count * MenuItemHeight);
    }

    private void UpdateSpinObject()
    {
        MainSceneManager.Instance.NOfMenu = _menuItems.Count;
        for (var i = 0; i < _menuItems.Count; i++)
        {
            if(_menuItems[i].Text == "") 
                MainSceneManager.Instance.SetMenuItem(i, "꽝");
            else
                MainSceneManager.Instance.SetMenuItem(i, _menuItems[i].Text);
        }
    }

    #region <Button Evnet>
    
    private void OnClickCloseButton()
    {
        UpdateSpinObject();
        FileManagement.SaveFile(_menuItems);
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
        gameObject.SetActive(false);
    }

    private void OnClickAddButton()
    {
        AddMenuItem();
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }

    private void OnClickSaveButton()
    {
        SetView(false);
        SaveUIManager.Instance.SetView(true);
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }
    
    #endregion

    
    private void Awake()
    {
        _instance = this;
        
        var closeButton = transform.Find("CloseButton/Button").GetComponent<Button>();
        closeButton.onClick.AddListener(OnClickCloseButton);
        var addButton = transform.Find("AddButton/Button").GetComponent<Button>();
        addButton.onClick.AddListener(OnClickAddButton);
        var saveButton = transform.Find("SaveButton/Button").GetComponent<Button>();
        saveButton.onClick.AddListener(OnClickSaveButton);

        _contents = transform.Find("Viewport/Content").GetComponent<RectTransform>();
        
        _menuItems = new List<MenuListItem>();
        _menuItemPool = new Stack<MenuListItem>();
        
        var menus = FileManagement.LoadFile()??MainSceneManager.Instance.DefaultMenu;
        foreach (var menu in menus)
        {
            AddMenuItem(menu);
        }
        
        UpdateSpinObject();
    }
    
    #region <Pooling>

    private MenuListItem MakeObject()
    {
        var obj = Resources.Load("MenuItem") as GameObject;
        var menuListItem = Instantiate(obj, _contents.transform).AddComponent<MenuListItem>();

        return menuListItem;
    }

    public void PoolObject(MenuListItem obj)
    {
        obj.gameObject.SetActive(false);
        _menuItemPool.Push(obj);
    }

    private MenuListItem GetObject()
    {
        if (_menuItemPool.Count == 0)
        {
            return MakeObject();
        }
        else
        {
            return _menuItemPool.Pop();
        }
    }

    #endregion
}
