using Game.Core;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Gère l'affichage du menu principal et l'accès aux actions principales du jeu.
    /// </summary>
    public sealed class MainMenuUIController : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField]
        private GameObject mainMenuPanel;

        [SerializeField]
        private GameObject optionsPanel;

        [Header("Chargement de scène")]
        [SerializeField]
        private SceneLoader sceneLoader;

        private void Awake()
        {
            if (sceneLoader == null)
            {
                sceneLoader = FindObjectOfType<SceneLoader>();
                if (sceneLoader == null)
                {
                    Debug.LogError("MainMenuUIController : aucun SceneLoader trouvé dans la scène.");
                }
            }

            EnsurePanelsState(mainMenuActive: true);
        }

        /// <summary>
        /// Lance la scène de jeu principale.
        /// </summary>
        public void OnPlayClicked()
        {
            if (sceneLoader == null)
            {
                Debug.LogError("MainMenuUIController.OnPlayClicked : SceneLoader non assigné.");
                return;
            }

            sceneLoader.LoadTrainingRange();
        }

        /// <summary>
        /// Affiche le panneau des options et masque le menu principal.
        /// </summary>
        public void OnOptionsClicked()
        {
            EnsurePanelsState(mainMenuActive: false);
        }

        /// <summary>
        /// Revient du panneau options au menu principal.
        /// </summary>
        public void OnBackFromOptionsClicked()
        {
            EnsurePanelsState(mainMenuActive: true);
        }

        /// <summary>
        /// Quitte l'application ou stoppe le mode Play dans l'éditeur.
        /// </summary>
        public void OnQuitClicked()
        {
            Application.Quit();

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
        }

        private void EnsurePanelsState(bool mainMenuActive)
        {
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(mainMenuActive);
            }
            else
            {
                Debug.LogWarning("MainMenuUIController : mainMenuPanel n'est pas assigné.");
            }

            if (optionsPanel != null)
            {
                optionsPanel.SetActive(!mainMenuActive);
            }
            else
            {
                Debug.LogWarning("MainMenuUIController : optionsPanel n'est pas assigné.");
            }
        }
    }
}
