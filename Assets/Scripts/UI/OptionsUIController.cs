using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Synchronise les contrôles d'options avec l'état courant du jeu et propage les changements utilisateur.
    /// </summary>
    public sealed class OptionsUIController : MonoBehaviour
    {
        [Header("Contrôles d'options")]
        [SerializeField]
        private Slider mouseSensitivitySlider;

        [SerializeField]
        private Slider masterVolumeSlider;

        [SerializeField]
        private Toggle fullscreenToggle;

        private GameStateController _gameStateController;

        private void Start()
        {
            _gameStateController = GameStateController.Instance;
            if (_gameStateController == null)
            {
                Debug.LogError("OptionsUIController : GameStateController.Instance est introuvable.");
                return;
            }

            InitialiseControls();
        }

        /// <summary>
        /// Répercute la nouvelle sensibilité de souris dans le GameStateController.
        /// </summary>
        /// <param name="value">Valeur normalisée provenant du slider.</param>
        public void OnMouseSensitivityChanged(float value)
        {
            _gameStateController?.SetMouseSensitivity(value);
        }

        /// <summary>
        /// Répercute le volume principal dans le GameStateController.
        /// </summary>
        /// <param name="value">Valeur normalisée provenant du slider.</param>
        public void OnMasterVolumeChanged(float value)
        {
            _gameStateController?.SetMasterVolume(value);
        }

        /// <summary>
        /// Met à jour l'état plein écran du jeu via le GameStateController.
        /// </summary>
        /// <param name="value">Nouvel état plein écran.</param>
        public void OnFullscreenChanged(bool value)
        {
            _gameStateController?.SetFullscreen(value);
        }

        private void InitialiseControls()
        {
            if (mouseSensitivitySlider != null)
            {
                mouseSensitivitySlider.SetValueWithoutNotify(_gameStateController.MouseSensitivity);
            }
            else
            {
                Debug.LogWarning("OptionsUIController : mouseSensitivitySlider n'est pas assigné.");
            }

            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.SetValueWithoutNotify(_gameStateController.MasterVolume);
            }
            else
            {
                Debug.LogWarning("OptionsUIController : masterVolumeSlider n'est pas assigné.");
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.SetIsOnWithoutNotify(_gameStateController.IsFullscreen);
            }
            else
            {
                Debug.LogWarning("OptionsUIController : fullscreenToggle n'est pas assigné.");
            }
        }
    }
}
