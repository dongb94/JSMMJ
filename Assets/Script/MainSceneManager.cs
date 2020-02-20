
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainSceneManager : MonoBehaviour
{
    private static MainSceneManager _instance;

    private bool _isSpin;
    
    private int _nOfMenu;
    private int _currentMenu;
    private List<SpinObject> _menuList;

    private Stack<SpinObject> _objectPool;

    private static GameObject canvas;
    private Button _menuButton;
    private Button _selectButton;

    private ResultPopUp _result;

    public readonly string[] DefaultMenu = {"라면", "덮밥", "국밥", "중식", "분식", "냉면", "돈까스", "햄버거"};
    
    public static MainSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                canvas = GameObject.Find("Canvas");
                canvas.transform.Find("Circle/SpinObjects").gameObject.AddComponent<MainSceneManager>();
            }
            return _instance;
        }
    }

    public int NOfMenu
    {
        get => _nOfMenu;
        set
        {
            if (_nOfMenu == value) return;
            _nOfMenu = value <= 0 ? 1 : value;
            
            // 메뉴가 현재보다 적어지면 여분은 Object Pull에 저장 
            if (_nOfMenu < _menuList.Count)
            {
                for (var i = _nOfMenu; i < _menuList.Count; i++)
                {
                    PoolObject(_menuList[i]);
                }
                _menuList.RemoveRange(_nOfMenu, _menuList.Count - _nOfMenu);
            }

            var angle = 360f / _nOfMenu;
            var fillAmount = 1f / _nOfMenu;
            
            // UI 세팅
            for (var i = 0; i < _nOfMenu; i++)
            {
                if (i < _menuList.Count)
                {
                    _menuList[i].InitWithFillAmountAndAngle(fillAmount, angle * i);
                }
                else
                {
                    var spinObj = GetObject();
                    spinObj.InitWithFillAmountAndAngle(fillAmount, angle * i);
                    _menuList.Add(spinObj);
                }
            }
            
        }
    }

    #region <Method>

    public void SetMenuItem(int index, string text)
    {
        _menuList[index].Text = text;
    }
    
    private void TossNextMenu()
    {
        _menuList[_currentMenu].UnSelected();
        _currentMenu = (_currentMenu+1) % _nOfMenu;
        _menuList[_currentMenu].Selected();
    }

    private IEnumerator Spin(int count)
    {
        TossNextMenu();
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
        yield return new WaitForSeconds(0.05f);
        count--;
        if (count > 0) StartCoroutine(Spin(count));
        else
        {
            _isSpin = false;
            _result.PopUp(_menuList[_currentMenu].Text);
            SoundManager.PlaySound(SoundManager.SoundList.Result);
        }
    }

    #endregion
    
    #region <Button Evnet>

    private void OnSelectButtonClick()
    {
        if (_isSpin) return;
        _isSpin = true;
        var random = Random.Range(_nOfMenu * 5, _nOfMenu * 10);
        StartCoroutine(Spin(random));
    }

    private void OnMenuButtonClick()
    {
        if(_isSpin) return;
        MenuListManager.Instance.SetView(true);
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }

    #endregion
    
    private void Awake()
    {
        _instance = this;
        _isSpin = false;
        _menuButton = canvas.transform.Find("Circle/MenuButton").GetComponent<Button>();
        _menuButton.onClick.AddListener(OnMenuButtonClick);
        _selectButton = canvas.transform.Find("SelectButton").GetComponent<Button>();
        _selectButton.onClick.AddListener(OnSelectButtonClick);
        _result = canvas.transform.Find("Result").gameObject.AddComponent<ResultPopUp>();
        _menuList = new List<SpinObject>();
        _objectPool = new Stack<SpinObject>();
        _nOfMenu = 0;
        _currentMenu = 0;
        NOfMenu = DefaultMenu.Length; // default
    }

    #region <Pooling>

    private SpinObject MakeObject()
    {
        var obj = Resources.Load("SpinObject") as GameObject;
        var spinObj = Instantiate(obj, transform).AddComponent<SpinObject>();

        return spinObj;
    }

    private void PoolObject(SpinObject obj)
    {
        obj.gameObject.SetActive(false);
        _objectPool.Push(obj);
    }

    private SpinObject GetObject()
    {
        if (_objectPool.Count == 0)
        {
            return MakeObject();
        }
        else
        {
            return _objectPool.Pop();
        }
    }

    #endregion
}