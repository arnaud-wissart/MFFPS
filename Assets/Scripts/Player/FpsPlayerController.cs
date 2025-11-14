using Game.Core;
using Game.Targets;
using UnityEngine;

namespace Game.Player
{
    /// <summary>
    /// Contrôleur FPS basique gérant déplacement, rotation caméra, saut et tir.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FpsPlayerController : MonoBehaviour
    {
        [Header("Paramètres de mouvement")]
        [SerializeField]
        private float moveSpeed = 5f;

        [Header("Paramètres de visée")]
        [SerializeField]
        private float baseSensitivity = 1f;

        [Header("Paramètres de saut et gravité")]
        [SerializeField]
        private float gravity = -9.81f;

        [SerializeField]
        private float jumpHeight = 1.5f;

        [Header("Paramètres de tir")]
        [SerializeField]
        private float maxShootDistance = 200f;

        [SerializeField]
        private LayerMask shootLayerMask = Physics.DefaultRaycastLayers; // TODO: ajuster le LayerMask pour les cibles spécifiques si nécessaire.

        [Header("Références")]
        [SerializeField]
        private Transform cameraTransform;

        [SerializeField]
        private float minVerticalAngle = -80f;

        [SerializeField]
        private float maxVerticalAngle = 80f;

        private CharacterController characterController;
        private float verticalVelocity;
        private float cameraPitch;
        private Vector3 horizontalMove;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (cameraTransform == null)
            {
                cameraTransform = GetComponentInChildren<Camera>()?.transform;
            }
        }

        private void Update()
        {
            if (cameraTransform == null)
            {
                return;
            }

            // Calcul de la sensibilité effective en fonction de l'état du jeu.
            float sensitivity = baseSensitivity;
            if (GameStateController.Instance != null)
            {
                sensitivity *= GameStateController.Instance.MouseSensitivity;
            }

            GérerRotation(sensitivity);
            GérerMouvement();
            GérerSaut();
            AppliquerGravité();
            DéplacerJoueur();
            GérerTir();
        }

        /// <summary>
        /// Gère la rotation de la caméra et du joueur via la souris.
        /// </summary>
        private void GérerRotation(float sensitivity)
        {
            // Rotation horizontale sur le joueur.
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            transform.Rotate(Vector3.up, mouseX);

            // Rotation verticale sur la caméra avec clamp.
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, minVerticalAngle, maxVerticalAngle);
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        /// <summary>
        /// Gère les déplacements latéraux et avant/arrière.
        /// </summary>
        private void GérerMouvement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 direction = (transform.right * horizontal) + (transform.forward * vertical);
            direction = Vector3.ClampMagnitude(direction, 1f);
            horizontalMove = direction * moveSpeed;
        }

        /// <summary>
        /// Gère l'input de saut.
        /// </summary>
        private void GérerSaut()
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (characterController.isGrounded && Input.GetButtonDown("Jump"))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        /// <summary>
        /// Applique la gravité chaque frame.
        /// </summary>
        private void AppliquerGravité()
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        /// <summary>
        /// Applique le déplacement vertical calculé au CharacterController.
        /// </summary>
        private void DéplacerJoueur()
        {
            Vector3 move = horizontalMove;
            move += Vector3.up * verticalVelocity;
            characterController.Move(move * Time.deltaTime);
        }

        /// <summary>
        /// Gère le tir via raycast depuis la caméra.
        /// </summary>
        private void GérerTir()
        {
            if (!Input.GetButtonDown("Fire1"))
            {
                return;
            }

            // Raycast pour détecter une cible et déclencher son comportement.
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, maxShootDistance, shootLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.TryGetComponent(out Target target))
                {
                    target.OnHit();
                }
            }
        }
    }
}
