using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SoundManager : MonoBehaviour {
    public static string AudioVolume = "audio_volume";
    public static string AudioOn = "audio_on";
    public static string SFXVolume = "SFX_volume";
    public static string SFXOn = "sfx_on";


    public enum ImpactSoundType {
        Planet,
        Floater,
        Ship
    }

    [SerializeField] AudioSource mainAudioSource;
    static public float maxMainVolume = 0.2f;
    static public float defaultVolume = 0.5f;


    [Header("Healing")]
    [SerializeField] AudioClip healSound;
    [SerializeField] float healVolume = 1f;

    [SerializeField] AudioClip respawnSound;
    [SerializeField] float respawnVolume = 1f;

    [Header("Shooting / Dmg")]
    [SerializeField] AudioClip[] impactPlanetSounds;
    [SerializeField] AudioClip[] impactFloaterSounds;
    [SerializeField] AudioClip[] impactShipSounds;
    [SerializeField] float impactVolume = 1f;


    [SerializeField] AudioClip shootSound;
    [SerializeField] float shootVolume = 1f;

    [Header("Main Menu Music")]
    [SerializeField] AudioClip menuClip;

    [Header("Game Music")]
    [SerializeField] AudioClip gameClip;
    

    float sfxVolume = 1f;
    public bool mainSoundOn = true;
    public bool sfxOn = true;

    void Awake() {
        ManageSingleton();

        float _mainVolume = PlayerPrefs.GetFloat(SoundManager.AudioVolume, maxMainVolume);
        float _sfxVolume = PlayerPrefs.GetFloat(SoundManager.SFXVolume, maxMainVolume);
        bool _audioOn = Helpers.Int32ToBool(PlayerPrefs.GetInt(SoundManager.AudioOn, 1));
        bool _sfxOn = Helpers.Int32ToBool(PlayerPrefs.GetInt(SoundManager.SFXOn, 1));

        mainAudioSource.volume = _mainVolume;
        sfxVolume = _sfxVolume;
        toggleMainSound(_audioOn);
        toggleSfxSound(_sfxOn);
    }

    void ManageSingleton() {
        int instanceCount = FindObjectsOfType(GetType()).Length;
        if(instanceCount > 1) {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        SceneManager.activeSceneChanged += ActiveSceneChanged;
    }

    private void OnDestroy() {
        SceneManager.activeSceneChanged -= ActiveSceneChanged;
    }

    public float getSFXVolume() {
        return sfxVolume;
    }

    public float getMainAudioVolume() {
        return mainAudioSource.volume / maxMainVolume;
    }

    public void mainAudioVolumeChanged(float sValue) {
        mainAudioSource.volume = maxMainVolume * sValue;
        PlayerPrefs.SetFloat(SoundManager.AudioVolume, mainAudioSource.volume);
    }

    public void sfxVolumeChanged(float sValue) {
        sfxVolume = sValue;
        PlayerPrefs.SetFloat(SoundManager.SFXVolume, sfxVolume);
    }

    public void toggleMainSound(bool toggle) {
        mainSoundOn = toggle;
        PlayerPrefs.SetInt(SoundManager.AudioOn, Helpers.BoolToInt32(toggle));
        if(!mainSoundOn) {
            mainAudioSource.Stop();
        } 
        else {
            mainAudioSource.Play();
        }
    }

    public void toggleSfxSound(bool toggle) {
        sfxOn = toggle;
        PlayerPrefs.SetInt(SoundManager.SFXOn, Helpers.BoolToInt32(toggle));
    }

    #region Sounds

    public void PlayHealSound(Vector3 point) {
        PlaySound(healSound, point, healVolume);
    }

    public void PlayRespawnSound(Vector3 point) {
        PlaySound(respawnSound, point, respawnVolume);
    }

    public void PlayImpactSound(Vector3 point, ImpactSoundType typeOf) {
        if(typeOf == ImpactSoundType.Floater) {
            int index = Random.Range(0, impactFloaterSounds.Length);
            PlaySound(impactFloaterSounds[index], point, impactVolume);
        }
        else if(typeOf == ImpactSoundType.Planet) {
            int index = Random.Range(0, impactPlanetSounds.Length);
            PlaySound(impactPlanetSounds[index], point, impactVolume);
        }
        else if(typeOf == ImpactSoundType.Ship) {
            int index = Random.Range(0, impactShipSounds.Length);
            PlaySound(impactShipSounds[index], point, impactVolume);
        }
    }

    public void PlayShootSound(Vector3 point) {
        PlaySound(shootSound, point, shootVolume);
    }

    #endregion

    void PlaySound(AudioClip sound, Vector3 point, float volume) {
        if(sfxOn) AudioSource.PlayClipAtPoint(sound, point, volume);
    }

    void ActiveSceneChanged(Scene from, Scene to) {
        if(to.buildIndex == 1) StartCoroutine( FadeOutAndInSound(gameClip, 3f) );
        else StartCoroutine( FadeOutAndInSound(menuClip, 3f) );
    }

    IEnumerator FadeOutAndInSound (AudioClip to, float FadeTime) {
        if(!mainSoundOn) {
            mainAudioSource.clip = to;
            yield return null;
        }
        else {
            float startVolume = mainAudioSource.volume;
    
            while (mainAudioSource.volume > 0) {
                mainAudioSource.volume -= startVolume * Time.deltaTime / FadeTime;
    
                yield return null;
            }
    
            mainAudioSource.Stop ();
            mainAudioSource.clip = to;
            mainAudioSource.Play();

            while (mainAudioSource.volume < startVolume) {
                mainAudioSource.volume += startVolume * Time.deltaTime / FadeTime;
                if(mainAudioSource.volume > startVolume) mainAudioSource.volume = startVolume;
    
                yield return null;
            }
        }

    }
}
