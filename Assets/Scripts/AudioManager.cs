using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public AudioSource audioSource;     
    public AudioClip introClip;        
    public AudioClip normalClip;      
    private bool hasSwitched = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = introClip;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSwitched && (audioSource.time >= 3f || !audioSource.isPlaying))
        {
            hasSwitched = true;
            audioSource.clip = normalClip;
            audioSource.loop = true;   
            audioSource.Play();
        }
    }
}
