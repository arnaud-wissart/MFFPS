using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    /// <summary>
    /// Fournit des méthodes utilitaires pour charger les scènes du jeu.
    /// </summary>
    public sealed class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// Charge une scène de manière synchrone par son nom.
        /// </summary>
        /// <param name="sceneName">Nom de la scène à charger.</param>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("SceneLoader.LoadScene appelé avec un nom de scène vide.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Charge la scène d'entraînement principale.
        /// </summary>
        public void LoadTrainingRange()
        {
            LoadScene("TrainingRange");
        }

        /// <summary>
        /// Exemple d'implémentation asynchrone à compléter selon les besoins.
        /// </summary>
        /// <param name="sceneName">Nom de la scène à charger.</param>
        public void LoadSceneAsync(string sceneName)
        {
            // TODO: Implémenter un écran de chargement si nécessaire (barre de progression, etc.).
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("SceneLoader.LoadSceneAsync appelé avec un nom de scène vide.");
                return;
            }

            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        }

        private System.Collections.IEnumerator LoadSceneAsyncRoutine(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}
