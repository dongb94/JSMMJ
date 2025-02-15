
using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveDataUI :MonoBehaviour
{
    private Text _text;

    private void Awake()
    {
        _text = transform.Find("Text").GetComponent<Text>();
        transform.Find("Load/Button").GetComponent<Button>().onClick.AddListener(OnClickLoadButton);
        transform.Find("Delete/Button").GetComponent<Button>().onClick.AddListener(OnClickDeleteButton);
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    private void OnClickLoadButton()
    {
        MenuListManager.Instance.SetView(true);
        SaveUIManager.Instance.SetView(false);
        MenuListManager.Instance.LoadSaveData(_text.text);
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }

    private void OnClickDeleteButton()
    {
        FileManagement.DeleteFile(_text.text);
        SaveUIManager.Instance.DeleteSaveDataUI(this);
        SoundManager.PlaySound(SoundManager.SoundList.SlotMachine);
    }
}