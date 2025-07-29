using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.Play();
    }
}
