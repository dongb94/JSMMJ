
using System;
using UnityEngine;

public class SoundManager :MonoBehaviour
{
    private static AudioSource _source;

    public enum SoundList
    {
        Start,
        SlotMachine,
        Result
    }
    private void Awake()
    {
        _source = transform.GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundList sound)
    {
        var clip = Resources.Load<AudioClip>(sound.ToString());
        _source.PlayOneShot(clip);
    }
}