using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform characterTransform;
    public LayerMask groundMask;

    public float moveSpeed = 50f;
    public float brakeForce = .5f;
    public float maxSpeed = 5f;
    public float turnSpeed = 15f;
    public float jumpForce = 750f;
    public float gravityModifier = .1f;

    private Vector2 m_Move;
    private bool m_Jump;
    private Rigidbody m_Rigidbody;
    private Collider m_Collider;
    private Animator m_Animator;
    private bool m_CanMove = false;
    private bool m_IsGrounded = true;

    private float m_JumpDelay = 0.3f;
    private float m_NextJumpTime = 0f;

    private bool m_IsOnVehicle = false;
    private bool m_IsOnVehicleArea = false;

    private PlayerInput m_Input;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();
        m_Animator = GetComponentInChildren<Animator>();
        m_Input = GetComponent<PlayerInput>();
    }

    //----- LÓGICA DE INPUT -----
    private void OnMove(InputValue movement)
    {
        m_Move = movement.Get<Vector2>();
    }

    private void OnJump(InputValue jumpInput)
    {
        m_Jump = jumpInput.isPressed;
    }

    private void OnEnter()
    {
        if(m_IsOnVehicleArea)
        {
            Debug.Log("Entrou no Veículo");
            PlayerNaveManager.instance.HideMessage();
            PlayerNaveManager.instance.ChangePlayer(PlayerType.Nave);
        }
        else
        {
            Debug.Log("Não está na área de colisão do Veículo.");
        }
    }

    public void DeactivateCharacter()
    {
        m_IsOnVehicle = true;
    }

    public void ActivateCharacter(Vector3 point)
    {
        m_IsOnVehicle = false;
        transform.position = point;
    }

    //----- LÓGICA DE MOVIMENTAÇÃO -----
    void FixedUpdate()
    {
        if (m_IsOnVehicle)
            return;

        m_CanMove = (Mathf.Abs(m_Move.x) > 0 || Mathf.Abs(m_Move.y) > 0);

        m_IsGrounded = (Physics.Raycast(transform.position, Vector3.down, 0.3f, groundMask));

        if (m_CanMove)
        {
            Vector3 moveDirection = new Vector3(m_Move.x, 0, m_Move.y).normalized;

            Vector3 projectedCameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);

            Quaternion rotationToCamera = Quaternion.LookRotation(projectedCameraForward, Vector3.up);

            //Apply the obtained rotation on the Move Direction vector
            moveDirection = rotationToCamera * moveDirection;
            
            if(new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z).magnitude < maxSpeed)
            {
                m_Rigidbody.velocity += moveDirection * moveSpeed * Time.deltaTime;
            }

            Vector3 viewDirection = m_Rigidbody.velocity.normalized;
            Quaternion newRotation = Quaternion.Lerp(characterTransform.transform.rotation, Quaternion.LookRotation(viewDirection, Vector3.up), Time.deltaTime * turnSpeed);
            characterTransform.localEulerAngles = new Vector3(0, newRotation.eulerAngles.y, 0);
        }

        if(m_IsGrounded)
        {
            if(m_Jump && m_NextJumpTime < Time.time)
            {
                m_Rigidbody.AddForce(Vector3.up * jumpForce);
                m_NextJumpTime = Time.time + m_JumpDelay;
            }

            if (m_Rigidbody.velocity.sqrMagnitude > 1)
            {
                m_Rigidbody.velocity += -m_Rigidbody.velocity.normalized * brakeForce;
            }
        }
        else
        {
            m_Rigidbody.velocity += Physics.gravity * gravityModifier;
        }

        m_Animator.SetFloat("speed", new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z).magnitude);
        m_Animator.SetBool("isGrounded", m_IsGrounded);
    }
    //FIXED UPDATE END


    //------ LÓGICA COLISÃO -----
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        {
            m_IsOnVehicleArea = true;

            if(m_Input.currentControlScheme == "Keyboard")
            {
                PlayerNaveManager.instance.ShowMessage("Hold \"ENTER\"");
            }
            else
            {
                PlayerNaveManager.instance.ShowMessage("Hold \"O Button\"");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        {
            m_IsOnVehicleArea = false;
            PlayerNaveManager.instance.HideMessage();
        }
    }
}
