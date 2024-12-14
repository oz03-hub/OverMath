using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdversarialAudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip BaeClip;
    private float lastBae;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastBae = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastBae >= 8f)
        {
            audioSource.PlayOneShot(BaeClip);

            lastBae = Time.time;
        }
    }
}
