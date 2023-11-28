using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager main;
    public AudioClip trainsound;
    public AudioClip ItemPickup;
    public AudioClip ItemEnd;
    public AudioClip CoinEat;
    public AudioClip _music;
    public AudioClip catchPlayer;
    public AudioClip GuardStartRun;
    public AudioClip stumbleGuard;
    public AudioClip crash;
    public AudioClip shakeCan;
    public AudioClip CanSpray;
    public AudioClip Click;
    public AudioClip SaveDeath;
    public AudioClip HighScore;
    public AudioClip MysteryBox;
    public AudioClip MissionComp;
    public AudioSource Effectsource;
    public AudioSource UISource;
    void Awake()
    {
        main = this;
    }
    private void OnEnable()
    {
        main = this;
    }
    private void Start()
    {
        GetComponent<AudioSource>().clip = _music;
        GetComponent<AudioSource>().Pause();
    }
    public void PlaySimpleClip(AudioClip clip)
    {
        Effectsource.PlayOneShot(clip);
        
    }
    public void PauseSimpleClip()
    {
        Effectsource.Pause();
        
    }
    public void ClickButton()
    {
        UISource.PlayOneShot(Click);
    }
    public void CompleteMission()
    {
        UISource.PlayOneShot(MissionComp);
    }
    public void Music(bool p)
    {
        if (p)
            GetComponent<AudioSource>().UnPause();
        if (!p)
            GetComponent<AudioSource>().Pause();
    }
}
