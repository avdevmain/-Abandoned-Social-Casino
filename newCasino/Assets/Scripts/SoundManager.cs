using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private float mainVolume;
    private float musicVolume;
    private float sfxVolume;

    public AudioSource buttonSound;
    public AudioSource backgroundMusic;
    public AudioSource slotBackgroundMusic;
    public AudioSource winSound;
    public AudioSource rollSound;


 //  public AudioClip bttnPressClip;
 //   public AudioClip mainBackClip;
    public AudioClip slotBackClip;
    public AudioClip winClip;
    public AudioClip megaWinClip;
    public AudioClip rollClip;
    public List<AudioClip> specialClips = new List<AudioClip>(); //Перечень звуков для особых режимов слота (колесо, скелет или другие)

    public static SoundManager instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            //Подгрузка главных звуков (кнопка, бэк в лобби)
            buttonSound = transform.parent.GetChild(6).GetComponent<AudioSource>();
            backgroundMusic = transform.parent.GetChild(5).GetComponent<AudioSource>();
            slotBackgroundMusic = transform.parent.GetChild(ButtonHandler.slotPatternIndex).GetChild(3).GetComponent<AudioSource>();
            rollSound = transform.parent.GetChild(ButtonHandler.slotPatternIndex).GetChild(3).GetComponent<AudioSource>();
            winSound =  transform.parent.GetChild(ButtonHandler.slotPatternIndex).GetChild(4).GetComponent<AudioSource>();

           // bttnPressClip = Resources.Load<AudioClip>("Sound/buttonPressed");
           // mainBackClip = Resources.Load<AudioClip>("Sound/start_background");



        }
        else
        if (instance == this)
            Destroy(gameObject);
    }



    public float GetMainVolume()
    {
        return mainVolume;
    }

    public void SetMainVolume(float value)
    {
        mainVolume = value;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
    }
    
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
    }



}
