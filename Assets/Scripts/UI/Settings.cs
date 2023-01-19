using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    public static string DisplayShipInfo = "display_info";

    [Header("SFX")]
    [SerializeField] Slider sfxSlider;
    [SerializeField] Toggle sfxToggle;

    [Header("Music")]
    [SerializeField] Slider musicSlider;
    [SerializeField] Toggle musicToggle;

    [Header("Other")]
    [SerializeField] public Toggle displayShipInfo;

    [Header("UI")]
    [SerializeField] GameObject panel;
    [SerializeField] Button close;

    SoundManager soundManager;

    private void Start() {
        soundManager = FindObjectOfType<SoundManager>();

        musicSlider.value = soundManager.getMainAudioVolume();
        musicToggle.isOn = soundManager.mainSoundOn;
        sfxSlider.value = soundManager.getSFXVolume();
        sfxToggle.isOn = soundManager.sfxOn;
        displayShipInfo.isOn = Helpers.Int32ToBool(PlayerPrefs.GetInt(DisplayShipInfo, 0));

        sfxToggle.onValueChanged.AddListener(delegate{ HandleSFXToggled(); });
        musicToggle.onValueChanged.AddListener(delegate{ HandleMusicToggled(); });
        sfxSlider.onValueChanged.AddListener(delegate{ HandleSFXSliderChanged(); });
        musicSlider.onValueChanged.AddListener(delegate{ HandleMusicSliderChanged(); });
        displayShipInfo.onValueChanged.AddListener(delegate{ HandleDisplayShipInfoChanged(); });

        close.onClick.AddListener(delegate { HandleClose(); });
    }

    public void HandleClose() {
        panel.SetActive(false);
    }

    public void HandleDisplayShipInfoChanged() {
        PlayerPrefs.SetInt(DisplayShipInfo, Helpers.BoolToInt32(displayShipInfo.isOn));
    }

    public void HandleMusicSliderChanged() {
        soundManager.mainAudioVolumeChanged(musicSlider.value);
    }

    public void HandleMusicToggled() {
        soundManager.toggleMainSound(musicToggle.isOn);
    }

    public void HandleSFXSliderChanged() {
        soundManager.sfxVolumeChanged(sfxSlider.value);
    }

    public void HandleSFXToggled() {
        soundManager.toggleSfxSound(sfxToggle.isOn);
    }
}
