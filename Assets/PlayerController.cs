using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Animation")]
    [SerializeField] private string isWalkingParam = "IsWalking";

    private CharacterController controller;
    private Animator animator;
    private Vector3 verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // On gère le déplacement via script, donc on désactive le root motion
        // au cas où l'animation Mixamo en contiendrait.
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        // 1) Lecture des flèches du clavier (GetAxisRaw = réponse instantanée, pas de lissage)
        float h = Input.GetAxisRaw("Horizontal"); // ← →
        float v = Input.GetAxisRaw("Vertical");   // ↑ ↓

        Vector3 input = new Vector3(h, 0f, v);
        Vector3 moveDir = input.sqrMagnitude > 1f ? input.normalized : input;

        bool isMoving = moveDir.sqrMagnitude > 0.01f;

        // 2) Rotation progressive vers la direction du mouvement
        if (isMoving)
        {
            Quaternion target = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, target, rotationSpeed * Time.deltaTime);
        }

        // 3) Gravité (pour rester collé au sol même sur de petites bosses)
        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;
        verticalVelocity.y += gravity * Time.deltaTime;

        // 4) Déplacement final (horizontal + vertical en un seul Move)
        Vector3 horizontalMove = moveDir * walkSpeed;
        controller.Move((horizontalMove + verticalVelocity) * Time.deltaTime);

        // 5) Mise à jour de l'Animator
        animator.SetBool(isWalkingParam, isMoving);
    }
}