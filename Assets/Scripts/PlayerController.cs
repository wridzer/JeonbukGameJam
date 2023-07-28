using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float jumpForce = 5.0f;
    public float slideForce = 2.0f;
    public float pickupRadius = 1.0f;  // Radius for the SphereCast
    public Transform pickupPoint;  // assign in Inspector
    private bool isJumping = false;
    private bool isSliding = false;
    private Rigidbody rb;
    private GameObject heldObject = null;
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2f;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        // Confine and hide the cursor
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        float forwardTranslation = Input.GetAxis("Vertical") * speed;
        float horizontalTranslation = Input.GetAxis("Horizontal") * speed;
        float rotation = Input.GetAxis("Mouse X") * rotationSpeed;

        forwardTranslation *= Time.deltaTime;
        horizontalTranslation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MovePosition(rb.position + this.transform.forward * forwardTranslation + this.transform.right * horizontalTranslation);
        rb.MoveRotation(rb.rotation * turn);

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isJumping = true;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetButtonDown("Fire3") && !isJumping && !isSliding)
        {
            StartCoroutine(Slide());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject != null)
            {
                // Put down the object
                heldObject.transform.SetParent(null);
                heldObject = null;
            }
            else
            {
                // Pick up an object
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRadius, LayerMask.GetMask("Pickup"));
                if (hitColliders.Length > 0)
                {
                    heldObject = hitColliders[0].gameObject;
                    heldObject.transform.SetParent(pickupPoint);
                    heldObject.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    IEnumerator Slide()
    {
        isSliding = true;

        rb.AddForce(transform.forward * slideForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);

        isSliding = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dropoff") && heldObject != null)
        {
            // Drop off the object
            heldObject.transform.SetParent(null);
            heldObject.transform.localPosition = other.transform.position + new Vector3(0, 1, 0);
            heldObject = null;
        }
    }
}
