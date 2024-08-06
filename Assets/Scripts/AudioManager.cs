using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("----- Music Clip -----")]
    public AudioClip titleTheme; //music for title screen
    public AudioClip townTheme; //music for town scenes
    public AudioClip apartmentTheme; //music for apartment scene
    public AudioClip dreamTheme; //music for dream scene
    public AudioClip sombreMoments; //music for story moments that convey sadness
    public AudioClip forgiveness; //music for story moments that convey character growth
    public AudioClip stealth; //music for investigation/stealth sequences
    public AudioClip partyTheme; //music for party scene
    public AudioClip ryanDiscovered; //music for end cutscene (when player finds ryan)

    [Header("----- SFX Clips -----")]
    public AudioClip phoneNotification;
    public AudioClip itemPickup;
    public AudioClip sceneTransition;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footstep3;
    public AudioClip buttonInteraction;
    public AudioClip crow;

    public static AudioManager Instance { get; private set; }

    private Dictionary<string, AudioClip> sceneMusicMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeSceneMusicMap();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeSceneMusicMap()
    {
        sceneMusicMap = new Dictionary<string, AudioClip>
        {
             { "MainMenu", titleTheme },
            { "Day1Town1Scene", townTheme },
            { "Day1Town2Scene", townTheme },
            { "Day2Town1Scene", townTheme },
            { "Day2Town2Scene", townTheme },
            { "Day1ApartmentScene", apartmentTheme },
            { "Day2ApartmentScene", apartmentTheme },
            { "Day1UrbanGoods", apartmentTheme },
            { "DreamTestingScene", dreamTheme },
            { "Day2Dream", dreamTheme },
            { "StealthScene", stealth },
            { "PartyScene", partyTheme },
            { "RyanDiscoveredScene", ryanDiscovered }
        };
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneMusicMap.TryGetValue(scene.name, out AudioClip musicClip))
        {
            PlayMusic(musicClip);
        }
        else
        {
            Debug.LogWarning($"No music assigned for scene: {scene.name}");
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

<<<<<<< HEAD
    public void PlayPhoneNotification()
    {
        SFXSource.PlayOneShot(phoneNotification);
    }

    public void ItemPickupSFX()
=======
    public void ClearSFX()
>>>>>>> parent of d90c93a (Merge branch 'developed' into feature-sprite-overhaul)
    {
        SFXSource.PlayOneShot(itemPickup);
    }

    public void ButtonSFX()
    {
        SFXSource.PlayOneShot(buttonInteraction);
    }

    public void WalkingSFX()
    {
        if (!SFXSource.isPlaying)
        {
            int randomIndex = Random.Range(0, 3); // Generates a random index between 0 and 3
            AudioClip randomFootstep = null;

            switch (randomIndex)
            {
                case 0:
                    randomFootstep = footstep1;
                    break;
                case 1:
                    randomFootstep = footstep2;
                    break;
                case 2:
                    randomFootstep = footstep3;
                    break;
                default:
                    Debug.LogError("Invalid random index for footstep sound effects.");
                    return;
            }

            SFXSource.PlayOneShot(randomFootstep);
        }
    }
}
