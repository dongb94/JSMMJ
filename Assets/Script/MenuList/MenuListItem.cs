
using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuListItem : MonoBehaviour
{
    private InputField _field;

    public string Text
    {
        get => _field.text;
        set
        {
            if (value == null) return;
            _field.text = value;
        }
    }

    private void Awake()
    {
        _field = transform.Find("InputField").GetComponent<InputField>();
        transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(OnClickDeleteButton);
    }

    private void OnClickDeleteButton()
    {
        _field.text = "";
        MenuListManager.Instance.DeleteMenuItem(this);
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }
}