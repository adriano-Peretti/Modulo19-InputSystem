using UnityEngine;
using UnityEngine.InputSystem;


public class CarController : MonoBehaviour
{
    [Header("Movement References")]
    public float speed = 100f;
    public float rotateSpeed = 15f;
    public float wheelRotateSpeed = 5f;
    public float wheelXRotateSpeed = 50f;
    public float brakeFactor = .5f;
    public LayerMask groundLayer;

    [Header("Visual References")]
    public Transform carTransform;
    public Transform[] frontWheels;
    public Transform[] backWheels;
    public GameObject brakeLights;

    [Header("Weapon References")]
    public Transform firePoint;
    public GameObject fireProjectile;
    public float fireForce = 250f;
    public float fireDelay = .3f;
    float lastFire = 0f;
    bool fire;

    Rigidbody m_Rigidbody;
    float wheelMaxRotationAngle = 30f;

    //Input Variables
    float m_HorizontalMove, m_VerticalMove;
    bool m_Brake = false;


    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        carTransform = GetComponentInChildren<Transform>();
    }

    //INPUT LOGIC

    //Acelera e Ré
    void OnVerticalMove(InputValue value)
    {
        m_VerticalMove = value.Get<float>();
    }

    //Volante
    void OnHorizontalMove(InputValue value)
    {
        m_HorizontalMove = value.Get<float>();
    }

    //Freio
    void OnBrake(InputValue value)
    {
        m_Brake = value.isPressed;
    }

    void OnFire(InputValue value)
    {
        fire = value.isPressed;
        Debug.Log("fired");
    }

    private void Update()
    {
        if (m_Brake != brakeLights.activeInHierarchy)
        {
            brakeLights.SetActive(m_Brake);
        }

        if (fire && Time.time > lastFire)
        {
            lastFire = Time.time + fireDelay;
            GameObject projectile = Instantiate(
                fireProjectile,
                firePoint.position,
                Quaternion.LookRotation(firePoint.forward));
            projectile.GetComponent<Rigidbody>().AddForce(
                firePoint.forward * fireForce,
                ForceMode.Impulse);
        }

    }

    #region Movement Logic
    //MOVEMENT LOGIC
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, -carTransform.up);

        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (!Physics.Raycast(ray, 1.6f, groundLayer))
        {
            return;
        }

        //-- Acelera o Carro baseado no VERTICAL MOVE
        m_Rigidbody.AddForce(m_VerticalMove * transform.forward * 100f * speed);

        //-- Se o carro estiver em movimento...
        if (m_Rigidbody.velocity.sqrMagnitude > 0.1f)
        {
            //-- Rotaciona o Carro na direção do HORIZONTAL MOVE
            if (Mathf.Abs(m_HorizontalMove) > 0.1f)
            {
                if (m_Brake)
                {
                    transform.Rotate(Vector3.up, m_VerticalMove * m_HorizontalMove * rotateSpeed / (brakeFactor / 2f) * Time.deltaTime);
                }
                else
                {
                    transform.Rotate(Vector3.up, m_VerticalMove * m_HorizontalMove * rotateSpeed * Time.deltaTime);
                }
            }

            //-- Rotaciona as Rodas, usando o HORIZONTAL MOVE e a VELOCITY
            Quaternion newRotation = Quaternion.Euler(frontWheels[0].localEulerAngles.x, m_HorizontalMove * wheelMaxRotationAngle, 0f);

            foreach (Transform wheel in frontWheels)
            {
                wheel.localRotation = Quaternion.RotateTowards(wheel.localRotation, newRotation, wheelRotateSpeed * Time.deltaTime);
                wheel.Rotate(Vector3.right, m_Rigidbody.velocity.sqrMagnitude * wheelXRotateSpeed * Time.deltaTime);
            }

            foreach (Transform wheel in backWheels)
            {
                wheel.Rotate(Vector3.right, m_Rigidbody.velocity.sqrMagnitude * wheelXRotateSpeed * Time.deltaTime);
            }

            //-- Se o BRAKE for acionado, aplica a função de Freio
            if (m_Brake)
            {
                m_Rigidbody.AddForce(new Vector3(-m_Rigidbody.velocity.x, 0f, -m_Rigidbody.velocity.z) * speed * brakeFactor);

                if (m_Rigidbody.velocity.sqrMagnitude < 0.1f)
                {
                    m_Rigidbody.velocity = Vector3.zero;
                }
            }
        }
    }//FIXED-UPDATE
    #endregion

    public void ChangeTexture(Texture tex)
    {
        int _numTexture = Random.Range(0, 8);

        Renderer rend = GetComponentInChildren<Renderer>();
        Material _body = rend.material;

        _body = new Material(_body);
        _body.name = _body.name + " (Instanced)";

        _body.mainTexture = tex;

        rend.material = _body;
    }

    public void ChangeSpawnPoint(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
}


