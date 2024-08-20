using System.Collections.Generic;
using UnityEngine;
using System.Collections;
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
    public AudioClip deptTheme; //music for Urban Goods scene
    public AudioClip dreamTheme; //music for dream scene
    public AudioClip sombreMoments; //music for story moments that convey sadness
    public AudioClip forgiveness; //music for story moments that convey character growth
    public AudioClip stealth; //music for investigation/stealth sequences
    public AudioClip partyTheme; //music for party scene
    public AudioClip ryanDiscovered; //music for end cutscene (when player finds ryan)

    [Header("----- SFX Clips -----")]
    public AudioClip phoneNotification;
    public AudioClip phoneUnlock;
    public AudioClip itemPickup;
    public AudioClip sceneTransition;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footstep3;
    public AudioClip footstep4;
    public AudioClip crow;

    [Header("----- Music Fade Settings -----")]
    [SerializeField] private float fadeDuration = 0.5f;



    public static AudioManager Instance { get; private set; }

    private Dictionary<string, AudioClip> sceneMusicMap;
    private Dictionary<SFXContext, AudioClip> SFXMap;

    private Queue<AudioClip> sfxQueue = new Queue<AudioClip>();
    private bool isPlayingSFX = false;
    [SerializeField] private float sfxQueueTimeout = 2.0f; // Timeout duration in seconds
    private float lastSFXRequestTime = -1f;


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
        InitializeSFXMap();
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
            { "Day3Town1Scene", townTheme },
            { "Day3Town2Scene", townTheme },
            { "Day1ApartmentScene", apartmentTheme },
            { "Day2ApartmentScene", apartmentTheme },
            { "Day3ApartmentScene", apartmentTheme },
            { "Day1UrbanGoodsScene", deptTheme },
            { "Day2UrbanGoodsScene", deptTheme },
            { "Day3UrbanGoodsScene", deptTheme },
            { "Day1Dream", dreamTheme },
            { "Day2Dream", dreamTheme },
            { "Day3Dream", dreamTheme },
            { "MaplewoodParkScene", partyTheme },
        };
    }

    private void InitializeSFXMap()
    {
        SFXMap = new Dictionary<SFXContext, AudioClip>
        {
            { SFXContext.PhoneNotification, phoneNotification },
            { SFXContext.ItemPickup, itemPickup },
            { SFXContext.SceneTransition, sceneTransition },
            { SFXContext.PhoneUnlock, phoneUnlock },
            { SFXContext.Crow, crow }
        };
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if a specific context needs to override the scene music
        if (CurrentContext != null)
        {
            PlayMusicByContext(CurrentContext);
        }
        else if (sceneMusicMap.TryGetValue(scene.name, out AudioClip musicClip))
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
            StartCoroutine(TransitionMusic(clip));
        }
    }

    public void PlayMusicByContext(string context)
    {
        AudioClip clip = null;

        // Determine the appropriate music clip based on the context
        switch (context)
        {
            case "Stealth":
                clip = stealth;
                break;
            case "Ryan Discovered":
                clip = ryanDiscovered;
                break;
            default:
                Debug.LogWarning($"No music assigned for context: {context}");
                return;
        }

        if (clip != null)
        {
            PlayMusic(clip);
        }
    }


    private IEnumerator TransitionMusic(AudioClip newClip)
    {
        if (musicSource.isPlaying)
        {
            yield return StartCoroutine(FadeOutMusic(musicSource));
        }

        yield return StartCoroutine(FadeInMusic(musicSource, newClip));
    }


    public void PlaySFX(SFXContext context)
    {
        if (SFXMap.TryGetValue(context, out AudioClip clip))
        {
            // Enqueue the SFX and update the last request time
            sfxQueue.Enqueue(clip);
            lastSFXRequestTime = Time.time;

            if (!isPlayingSFX)
            {
                StartCoroutine(ProcessSFXQueue());
            }
        }
        else
        {
            Debug.LogWarning($"No SFX assigned for context: {context}");
        }
    }


    private IEnumerator ProcessSFXQueue()
    {
        isPlayingSFX = true;

        while (sfxQueue.Count > 0)
        {
            // Check for timeout
            if (Time.time - lastSFXRequestTime > sfxQueueTimeout)
            {
                Debug.Log("SFX queue timeout reached. Clearing remaining items.");
                sfxQueue.Clear(); // Clear the remaining items in the queue
                break; // Exit the loop
            }

            AudioClip clip = sfxQueue.Dequeue();
            SFXSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length); // Wait for the clip to finish before continuing
        }

        isPlayingSFX = false;
    }

    // Assuming you have a method to get the player's volume setting
    private float GetPlayerVolume()
    {
        // Replace this with your actual volume retrieval logic
        return PlayerPrefs.GetFloat("OverallVolume", 1.0f); // Default to 1.0 if not set
    }

    private IEnumerator FadeOutMusic(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;
        float targetVolume = 0;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / fadeDuration);
            audioSource.volume = volume * GetPlayerVolume();
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
    }

    private IEnumerator FadeInMusic(AudioSource audioSource, AudioClip newClip)
    {
        audioSource.clip = newClip;
        audioSource.volume = 0;
        audioSource.Play();

        float timeElapsed = 0f;
        float targetVolume = GetPlayerVolume(); // Use player's volume setting

        while (timeElapsed < fadeDuration)
        {
            float volume = Mathf.Lerp(0, targetVolume, timeElapsed / fadeDuration);
            audioSource.volume = volume;
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    public void ClearSFX()
    {
        SFXSource.Stop();
    }

    private string CurrentContext { get; set; }

    public void SetContext(string context)
    {
        CurrentContext = context;
        PlayMusicByContext(context);
    }

    public void ClearContext()
    {
        CurrentContext = null;
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

public enum SFXContext
{
    PhoneNotification,
    ItemPickup,
    SceneTransition,
    PhoneUnlock,
    Crow
}
