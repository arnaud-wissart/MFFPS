using Game.Training;
using UnityEngine;

namespace Game.Targets
{
    /// <summary>
    /// Représente une cible de tir capable de se déclarer auprès du gestionnaire d'entraînement
    /// et de réagir lorsque le joueur la touche.
    /// </summary>
    public class Target : MonoBehaviour
    {
        private bool _isRegistered;

        private void OnEnable()
        {
            var trainingManager = TrainingManager.Instance;
            if (trainingManager == null)
            {
                Debug.LogWarning("TrainingManager non disponible lors de l'activation de la cible.", this);
                _isRegistered = false;
                return;
            }

            trainingManager.RegisterTarget(this);
            _isRegistered = true;
        }

        private void OnDisable()
        {
            if (!_isRegistered)
            {
                return;
            }

            _isRegistered = false;

            var trainingManager = TrainingManager.Instance;
            if (trainingManager == null)
            {
                // Le manager peut déjà être détruit lors du déchargement de la scène.
                return;
            }

            trainingManager.UnregisterTarget(this);
        }

        /// <summary>
        /// Appelé par le contrôleur de tir lorsque la cible est touchée.
        /// </summary>
        public void OnHit()
        {
            var trainingManager = TrainingManager.Instance;
            if (trainingManager == null)
            {
                Debug.LogWarning("TrainingManager non disponible lors du traitement d'un impact.", this);
            }
            else
            {
                trainingManager.OnTargetHit(this);
            }

            // TODO: Ajouter un feedback visuel (animation, changement de matériau, etc.).
            gameObject.SetActive(false);
        }
    }
}
