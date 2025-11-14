using System;
using System.Collections.Generic;
using Game.Targets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Game.Training
{
    /// <summary>
    /// Gère l'ensemble d'une session d'entraînement en chronométrant le joueur,
    /// en enregistrant les meilleures performances et en mettant à jour l'UI.
    /// </summary>
    public class TrainingManager : MonoBehaviour
    {
        private const string BestTimeKey = "BestTrainingTime";

        [Header("Références UI")] 
        [Tooltip("Texte affichant le temps courant. Assigner depuis l'inspector (Text ou TextMeshProUGUI).")]
        [SerializeField]
        private TMP_Text timeText;

        [Tooltip("Texte affichant le meilleur temps. Assigner depuis l'inspector (Text ou TextMeshProUGUI).")]
        [SerializeField]
        private TMP_Text bestTimeText;

        [Tooltip("Texte affichant le nombre de cibles restantes. Assigner depuis l'inspector.")]
        [SerializeField]
        private TMP_Text remainingTargetsText;

        [Header("Boutons")]
        [Tooltip("Bouton Restart. Configurer l'événement OnClick pour appeler OnRestartClicked depuis l'inspector.")]
        [SerializeField]
        private Button restartButton;

        [Tooltip("Bouton Back To Menu. Configurer l'événement OnClick pour appeler OnBackToMenuClicked depuis l'inspector.")]
        [SerializeField]
        private Button backToMenuButton;

        [Header("Noms des scènes")]
        [SerializeField]
        private string mainMenuSceneName = "MainMenu";

        [SerializeField]
        private string trainingSceneName = "TrainingRange";

        private readonly List<Target> _targets = new();
        private bool _isSessionRunning;
        private float _currentTime;

        public static TrainingManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Une instance supplémentaire de TrainingManager a été détectée et sera détruite.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (backToMenuButton != null)
            {
                backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
            }

            InitialiseUI();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(OnRestartClicked);
            }

            if (backToMenuButton != null)
            {
                backToMenuButton.onClick.RemoveListener(OnBackToMenuClicked);
            }
        }

        private void Update()
        {
            if (_isSessionRunning)
            {
                _currentTime += Time.deltaTime;
                UpdateTimeDisplay();
            }

            UpdateRemainingTargetsDisplay();
        }

        /// <summary>
        /// Enregistre une cible active dans la session.
        /// </summary>
        public void RegisterTarget(Target target)
        {
            if (target == null)
            {
                return;
            }

            if (!_targets.Contains(target))
            {
                _targets.Add(target);
                UpdateRemainingTargetsDisplay();
            }
        }

        /// <summary>
        /// Supprime la cible de la session lorsque celle-ci est désactivée.
        /// </summary>
        public void UnregisterTarget(Target target)
        {
            if (target == null)
            {
                return;
            }

            if (_targets.Remove(target))
            {
                UpdateRemainingTargetsDisplay();
            }
        }

        /// <summary>
        /// Appelée lorsqu'une cible est touchée.
        /// </summary>
        public void OnTargetHit(Target target)
        {
            if (!_isSessionRunning)
            {
                StartSession();
            }

            _targets.Remove(target);
            UpdateRemainingTargetsDisplay();

            if (_targets.Count == 0)
            {
                EndSession();
            }
        }

        /// <summary>
        /// Recharge la scène d'entraînement.
        /// </summary>
        public void OnRestartClicked()
        {
            SceneManager.LoadScene(trainingSceneName);
        }

        /// <summary>
        /// Retourne au menu principal.
        /// </summary>
        public void OnBackToMenuClicked()
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void StartSession()
        {
            _currentTime = 0f;
            _isSessionRunning = true;
            UpdateTimeDisplay();
        }

        private void EndSession()
        {
            _isSessionRunning = false;
            UpdateTimeDisplay();

            var hasBestTime = PlayerPrefs.HasKey(BestTimeKey);
            var bestTime = hasBestTime ? PlayerPrefs.GetFloat(BestTimeKey) : float.MaxValue;

            if (_currentTime < bestTime)
            {
                PlayerPrefs.SetFloat(BestTimeKey, _currentTime);
                PlayerPrefs.Save();
            }

            UpdateBestTimeDisplay();
        }

        private void InitialiseUI()
        {
            UpdateTimeDisplay();
            UpdateBestTimeDisplay();
            UpdateRemainingTargetsDisplay();
        }

        private void UpdateTimeDisplay()
        {
            if (timeText != null)
            {
                timeText.text = FormatTime(_currentTime);
            }
        }

        private void UpdateBestTimeDisplay()
        {
            if (bestTimeText == null)
            {
                return;
            }

            if (PlayerPrefs.HasKey(BestTimeKey))
            {
                var bestTime = PlayerPrefs.GetFloat(BestTimeKey);
                bestTimeText.text = FormatTime(bestTime);
            }
            else
            {
                bestTimeText.text = "--:--.---";
            }
        }

        private void UpdateRemainingTargetsDisplay()
        {
            if (remainingTargetsText == null)
            {
                return;
            }

            remainingTargetsText.text = _targets.Count.ToString();
        }

        private static string FormatTime(float time)
        {
            var clampedTime = Mathf.Max(0f, time);
            var timeSpan = TimeSpan.FromSeconds(clampedTime);
            return string.Format("{0:00}:{1:00}.{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}
