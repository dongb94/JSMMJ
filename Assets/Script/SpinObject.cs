using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinObject : MonoBehaviour
{
    private Image _circle;
    private Text _text;

    public string Text
    {
        get => _text.text;
        set => _text.text = value;
    }

    public void Selected()
    {
        _circle.color = Color.white;
    }

    public void UnSelected()
    {
        _circle.color = Color.gray;
    }

    public void InitWithFillAmountAndAngle(float fillAmount, float angle)
    {
        _circle.fillAmount = fillAmount;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        gameObject.SetActive(true);
    }

    private void Awake()
    {
        _circle = transform.Find("Circle").GetComponent<Image>();
        _text = transform.Find("Circle/Text").GetComponent<Text>();
        if(!_circle || !_text) Debug.Log("Initialize error");
    }
}
