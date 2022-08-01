using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleControllerDone : MonoBehaviour
{
    public float speed = 10f;
    public float rotateSpeed = 15f;
    public Transform visual;

    public float jumpForce = 6f;
    public LayerMask groundLayer;

    float lastFire;
    public float fireDelay = .3f;
    public float fireForce = 250f;
    public Transform firePoint;
    public GameObject fireProjectile;

    //Variaveis Input
    bool fire;
    Vector2 move;

    Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //Input - SendMessages
    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    void OnJump()
    {
        Jump();
    }

    void OnFire(InputValue value)
    {
        fire = value.isPressed;
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 2.1f, Color.cyan);

        if(fire && Time.time > lastFire)
        {
            lastFire = Time.time + fireDelay;
            GameObject projectile = Instantiate(fireProjectile, firePoint.position, Quaternion.LookRotation(firePoint.forward));
            projectile.GetComponent<Rigidbody>().AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
        }
    }

    //Move Logic
    private void FixedUpdate()
    {
        //Movimento
        Vector3 movement = new Vector3(move.x, 0f, move.y);
        transform.Translate(movement * speed * Time.deltaTime);

        //Rotação
        if (movement.magnitude > 0f)
        {
            Quaternion newRotation = Quaternion.LookRotation(movement, Vector3.up);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, newRotation, rotateSpeed);
        }

    }

    private void Jump()
    {
        if(Physics.Raycast(transform.position, Vector3.down, 2.1f, groundLayer))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

}
