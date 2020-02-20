
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveUIManager :MonoBehaviour
{
    private static SaveUIManager _instance;

    private const float SaveDataHeight = 80f;
    
    private GameObject _contents;
    private InputField _saveNameField;

    private List<SaveDataUI> _saveDatas;
    private Stack<SaveDataUI> _saveDataPool;

    public static SaveUIManager Instance
    {
        get
        {
            if (!_instance)
            {
                var canvas = GameObject.Find("Canvas");
                canvas.transform.Find("PopUp/MenuSave").gameObject.AddComponent<SaveUIManager>();
            }
            
            return _instance;
        }
    }

    public void SetView(bool active)
    {
        gameObject.SetActive(active);
    }

    public void AddSaveDataUI(string name)
    {
        var data = GetObject();
        data.SetText(name);

        if (_saveDatas.Count == 0)
        {
            data.transform.localPosition = Vector3.zero;
        }
        else
        {
            var prePosition = _saveDatas[_saveDatas.Count - 1].transform.localPosition;
            prePosition.y -= SaveDataHeight;
            data.transform.localPosition = prePosition;    
        }

        UpdateContentsSize();
        
        _saveDatas.Add(data);
        
        data.gameObject.SetActive(true);
    }

    public void DeleteSaveDataUI(SaveDataUI data)
    {
        if (!_saveDatas.Contains(data)) return;
        var index = _saveDatas.IndexOf(data);
        _saveDatas.Remove(data);
        PoolObject(data);

        for (var i = index; i < _saveDatas.Count; i++)
        {
            var position = _saveDatas[i].transform.localPosition;
            position.y += SaveDataHeight;
            _saveDatas[i].transform.localPosition = position;
        }
        
        UpdateContentsSize();
    }

    public void ReloadSaveDataUI()
    {
        var list = FileManagement.LoadSaveDataList();
        for (var i = 0; i < list.Length; i++)
        {
            if(_saveDatas.Count > i)
                _saveDatas[i].SetText(list[i]);
            else
                AddSaveDataUI(list[i]);
        }
        for (var i = _saveDatas.Count; i > list.Length; i--)
        {
            DeleteSaveDataUI(_saveDatas[i-1]);
        }
    }
    
    private void UpdateContentsSize()
    {
        _contents.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _saveDatas.Count * SaveDataHeight);
    }

    private void OnClickSaveButton()
    {
        MenuListManager.Instance.SaveData(_saveNameField.text);
        ReloadSaveDataUI();
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }

    private void OnClickCloseButton()
    {
        SetView(false);
        MenuListManager.Instance.SetView(true);
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }

    private void Awake()
    {
        _instance = this;
        
        _contents = transform.Find("Scroll View/Viewport/Content").gameObject;
        _saveNameField = transform.Find("Input/InputField").GetComponent<InputField>();
        transform.Find("Input/SaveButton").GetComponent<Button>().onClick.AddListener(OnClickSaveButton);
        transform.Find("CloseButton/Button").GetComponent<Button>().onClick.AddListener(OnClickCloseButton);

        _saveDatas = new List<SaveDataUI>();
        _saveDataPool = new Stack<SaveDataUI>();

        ReloadSaveDataUI();
    }

    #region <Pooling>

    private SaveDataUI MakeObject()
    {
        var obj = Resources.Load("SaveData") as GameObject;
        var saveDataUi = Instantiate(obj, _contents.transform).AddComponent<SaveDataUI>();

        return saveDataUi;
    }

    private void PoolObject(SaveDataUI obj)
    {
        obj.gameObject.SetActive(false);
        _saveDataPool.Push(obj);
    }

    private SaveDataUI GetObject()
    {
        if (_saveDataPool.Count == 0)
        {
            return MakeObject();
        }
        else
        {
            return _saveDataPool.Pop();
        }
    }

    #endregion
    
}
