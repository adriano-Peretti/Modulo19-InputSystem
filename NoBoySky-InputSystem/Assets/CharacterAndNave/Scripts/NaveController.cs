using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NaveController : MonoBehaviour
{
    public Transform playerSpawnPoint;
    public MeshRenderer enterAreaMesh;

    public float accelerateForce = 500f;
    public float ascendForce = 350f;
    public float turnSpeed = 60;
    public Transform naveVisual = null;
    public LayerMask groundLayer;

    MyControls m_Controls;
    Rigidbody m_Rigidbody;

    float m_HorizontalMove, m_VerticalMove, m_Accelerate = 0f;

    bool m_IsOnVehicle = false;

    private void Awake()
    {
        m_Controls = new MyControls();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    //Associa funções aos eventos das Ações
    private void OnEnable()
    {
        m_Controls.Nave.Enable();

        m_Controls.Nave.HorizontalMove.performed += OnHorizontalMove;
        m_Controls.Nave.HorizontalMove.canceled += OnHorizontalMove;

        m_Controls.Nave.VerticalMove.performed += OnVerticalMove;
        m_Controls.Nave.VerticalMove.canceled += OnVerticalMove;

        m_Controls.Nave.Accelerate.started += OnAccelerate;
        m_Controls.Nave.Accelerate.canceled += OnAccelerate;

        m_Controls.Nave.Leave.performed += OnLeave;
    }

    //Desassocia funções aos eventos das Ações
    private void OnDisable()
    {
        m_Controls.Nave.Disable();

        m_Controls.Nave.HorizontalMove.performed -= OnHorizontalMove;
        m_Controls.Nave.HorizontalMove.canceled -= OnHorizontalMove;

        m_Controls.Nave.VerticalMove.performed -= OnVerticalMove;
        m_Controls.Nave.VerticalMove.canceled -= OnVerticalMove;

        m_Controls.Nave.Accelerate.started -= OnAccelerate;
        m_Controls.Nave.Accelerate.canceled -= OnAccelerate;

        m_Controls.Nave.Leave.performed -= OnLeave;
    }

    //----- LÓGICA DE INPUT -----
    void OnHorizontalMove(InputAction.CallbackContext context)
    {
        m_HorizontalMove = context.ReadValue<float>();
    }

    void OnVerticalMove(InputAction.CallbackContext context)
    {
        m_VerticalMove = context.ReadValue<float>();
    }

    void OnAccelerate(InputAction.CallbackContext context)
    {
        m_Accelerate = context.ReadValue<float>();
    }

    private void OnLeave(InputAction.CallbackContext context)
    {
        Debug.Log("Ejetar?");
        if (m_Rigidbody.velocity.sqrMagnitude < 0.1f)
        {
            if(Physics.Raycast(transform.position, Vector3.down, 2f, groundLayer))
            {
                Debug.Log("Saiu do Veículo");
                PlayerNaveManager.instance.ChangePlayer(PlayerType.Character);
            }
        }
    }

    public void DeactivateNave()
    {
        m_IsOnVehicle = false;
        m_Rigidbody.isKinematic = true;
        enterAreaMesh.enabled = true;
        m_Controls.Disable();     
    }

    public void ActivateNave()
    {
        m_IsOnVehicle = true;
        m_Rigidbody.isKinematic = false;
        enterAreaMesh.enabled = false;
        m_Controls.Enable();
    }

    //----- LÓGICA DE MOVIMENTAÇÃO -----

    private void FixedUpdate()
    {
        if (!m_IsOnVehicle)
            return;

        Vector3 rotationAngles = Vector3.zero;

        if(Mathf.Abs(m_HorizontalMove) > 0)
        {
            transform.Rotate(Vector3.up, m_HorizontalMove * turnSpeed * Time.deltaTime);
            rotationAngles.z = 45f * -m_HorizontalMove;
        }

        if(Mathf.Abs(m_VerticalMove) > 0)
        {
            m_Rigidbody.AddForce(Vector3.up * -m_VerticalMove * ascendForce * Time.deltaTime);
            rotationAngles.x = 30f * m_VerticalMove;
        }

        Quaternion toRotation = Quaternion.Euler(rotationAngles);

        naveVisual.localRotation = Quaternion.Lerp(naveVisual.localRotation, toRotation, Time.deltaTime);


        if (m_Accelerate > 0f)
        {
            m_Rigidbody.AddForce(transform.forward * m_Accelerate * accelerateForce * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 2f);
    }

}
