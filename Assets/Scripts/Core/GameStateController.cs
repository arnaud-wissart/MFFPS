using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Contrôle global des paramètres persistants du jeu (sensibilité, volume, plein écran).
    /// Implémente un singleton simple pour rester disponible entre les scènes.
    /// </summary>
    public sealed class GameStateController : MonoBehaviour
    {
        private const string MouseSensitivityKey = "Game.MouseSensitivity";
        private const string MasterVolumeKey = "Game.MasterVolume";
        private const string FullscreenKey = "Game.IsFullscreen";

        private const float DefaultMouseSensitivity = 1.0f;
        private const float DefaultMasterVolume = 1.0f;
        private const bool DefaultFullscreen = true;

        private static GameStateController _instance;

        /// <summary>
        /// Instance unique de <see cref="GameStateController"/>.
        /// </summary>
        public static GameStateController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameStateController>();
                }

                return _instance;
            }
        }

        /// <summary>
        /// Sensibilité de la souris utilisée par les contrôles du joueur.
        /// </summary>
        public float MouseSensitivity { get; private set; }

        /// <summary>
        /// Volume global du jeu.
        /// </summary>
        public float MasterVolume { get; private set; }

        /// <summary>
        /// Indique si le jeu est en mode plein écran.
        /// </summary>
        public bool IsFullscreen { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettings();
            ApplyCurrentSettings();
        }

        /// <summary>
        /// Met à jour la sensibilité de la souris et persiste la nouvelle valeur.
        /// </summary>
        /// <param name="value">Nouvelle sensibilité. Les valeurs négatives sont clampées à 0.</param>
        public void SetMouseSensitivity(float value)
        {
            MouseSensitivity = Mathf.Max(0f, value);
            PlayerPrefs.SetFloat(MouseSensitivityKey, MouseSensitivity);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Met à jour le volume global, le persiste et applique immédiatement la modification.
        /// </summary>
        /// <param name="value">Nouvelle valeur de volume, clampée entre 0 et 1.</param>
        public void SetMasterVolume(float value)
        {
            MasterVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
            PlayerPrefs.Save();

            AudioListener.volume = MasterVolume;
        }

        /// <summary>
        /// Met à jour l'état plein écran, le persiste et applique immédiatement la modification.
        /// </summary>
        /// <param name="value">Nouvel état plein écran.</param>
        public void SetFullscreen(bool value)
        {
            IsFullscreen = value;
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            PlayerPrefs.Save();

            Screen.fullScreen = IsFullscreen;
        }

        /// <summary>
        /// Réapplique les paramètres courants aux systèmes Unity correspondants.
        /// </summary>
        public void ApplyCurrentSettings()
        {
            AudioListener.volume = MasterVolume;
            Screen.fullScreen = IsFullscreen;
        }

        private void LoadSettings()
        {
            MouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, DefaultMouseSensitivity);
            MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);
            IsFullscreen = PlayerPrefs.GetInt(FullscreenKey, DefaultFullscreen ? 1 : 0) == 1;
        }
    }
}
