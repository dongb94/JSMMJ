using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopUp : MonoBehaviour
{
    private const float FloatTime = 1.2f;
    private Text _text;
    private GameObject _image;
    private float _time;
    private void Awake()
    {
        _text = transform.Find("Image/Text").GetComponent<Text>();
        _image = transform.Find("Image").gameObject;
        transform.GetComponent<Button>().onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            GoogleAdMob.Instance.PlayInterstitialAd();
        });
        gameObject.SetActive(false);
    }

    public void PopUp(string text)
    {
        _text.text = text;
        _image.transform.localScale = Vector3.zero;
        _time = 0;
        gameObject.SetActive(true);
        StartCoroutine(ScaleUp());
    }

    private IEnumerator ScaleUp()
    {
        _time += Time.deltaTime;
        _image.transform.localScale = Vector3.one * (_time / FloatTime);
        yield return new WaitForFixedUpdate();
        if (_time < FloatTime) StartCoroutine(ScaleUp());
    }
}
