using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerSprite;

    private Vector3 movement;
    private Rigidbody rb;
    private PlayerControls inputActions;

    private Vector3 lastMoveDir = Vector3.back; // 角色默认朝南character is set to face south by default.

    private const string MOVE_X_PARAM = "MoveX";
    private const string MOVE_Z_PARAM = "MoveZ";


    private void Awake()
    {
        inputActions = new PlayerControls();  
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator.SetFloat(MOVE_Z_PARAM, lastMoveDir.z);
    }

    void Update()
    {
        float delatX = inputActions.Player.Move.ReadValue<Vector2>().x;
        float delatZ = inputActions.Player.Move.ReadValue<Vector2>().y;

        movement = new Vector3(delatX, 0 , delatZ).normalized;

        if(movement.magnitude > 0.01f)
        {
            animator.SetFloat(MOVE_X_PARAM, delatX);
            animator.SetFloat(MOVE_Z_PARAM, delatZ);
            lastMoveDir = movement.normalized;
        }
        else
        {
            animator.SetFloat(MOVE_X_PARAM, lastMoveDir.x);
            animator.SetFloat(MOVE_Z_PARAM, lastMoveDir.z);
        }

    }

    private void FixedUpdate()
    {
        rb.MovePosition(this.transform.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
