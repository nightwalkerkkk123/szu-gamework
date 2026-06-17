using System;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Lightweight per-user settings persisted via PlayerPrefs. Holds master /
    /// music / sfx volume and fullscreen toggle. Audio applies via
    /// AudioListener.volume; fullscreen via Screen.fullScreen.
    ///
    /// Round 2 MVP. Round 3 (out of scope) would route music/sfx sliders
    /// through an AudioMixer with separate exposed parameters.
    /// </summary>
    public static class SettingsService
    {
        private const string KeyMasterVolume = "Settings.MasterVolume";
        private const string KeyMusicVolume = "Settings.MusicVolume";
        private const string KeySfxVolume = "Settings.SfxVolume";
        private const string KeyFullscreen = "Settings.Fullscreen";

        private const float DefaultVolume = 0.8f;

        private static float _masterVolume = DefaultVolume;
        private static float _musicVolume = DefaultVolume;
        private static float _sfxVolume = DefaultVolume;
        private static bool _fullscreen = true;

        public static event Action OnSettingsChanged;

        public static float MasterVolume
        {
            get => _masterVolume;
            set { _masterVolume = Mathf.Clamp01(value); Apply(); }
        }

        public static float MusicVolume
        {
            get => _musicVolume;
            set { _musicVolume = Mathf.Clamp01(value); /* music bus hook TBD */ }
        }

        public static float SfxVolume
        {
            get => _sfxVolume;
            set { _sfxVolume = Mathf.Clamp01(value); /* sfx bus hook TBD */ }
        }

        public static bool Fullscreen
        {
            get => _fullscreen;
            set { _fullscreen = value; Apply(); }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Load();
            Apply();
        }

        public static void Load()
        {
            _masterVolume = PlayerPrefs.GetFloat(KeyMasterVolume, DefaultVolume);
            _musicVolume = PlayerPrefs.GetFloat(KeyMusicVolume, DefaultVolume);
            _sfxVolume = PlayerPrefs.GetFloat(KeySfxVolume, DefaultVolume);
            _fullscreen = PlayerPrefs.GetInt(KeyFullscreen, 1) == 1;
        }

        public static void Save()
        {
            PlayerPrefs.SetFloat(KeyMasterVolume, _masterVolume);
            PlayerPrefs.SetFloat(KeyMusicVolume, _musicVolume);
            PlayerPrefs.SetFloat(KeySfxVolume, _sfxVolume);
            PlayerPrefs.SetInt(KeyFullscreen, _fullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }

        private static void Apply()
        {
            AudioListener.volume = _masterVolume;
            if (Screen.fullScreen != _fullscreen)
            {
                Screen.fullScreen = _fullscreen;
            }
            OnSettingsChanged?.Invoke();
        }
    }
}
