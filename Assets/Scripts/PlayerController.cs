using Game.Building;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float slideForce = 2.0f;
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private float pickupRadius = 1.0f;  
    [SerializeField] private Transform pickupPoint;  
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
        // Calculate the direction of movement based on player input
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        // Adjust the player's velocity while keeping vertical velocity the same
        rb.velocity = transform.TransformDirection(direction) * speed + new Vector3(0, rb.velocity.y, 0);

        float rotation = Input.GetAxis("Mouse X") * rotationSpeed;
        rotation *= Time.deltaTime;

        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
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
            Pickup();
        }
    }

    private void Pickup()
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
                heldObject.GetComponentInParent<PlantSpawner>()?.PlantRemoved();
                heldObject.transform.SetParent(pickupPoint);
                heldObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    IEnumerator Slide()
    {
        isSliding = true;

        rb.AddForce(transform.forward * slideForce, ForceMode.Impulse);
        yield return new WaitForSeconds(slideDuration);

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
            other.GetComponent<Building_FlowerPoint>()?.SetFlowerPointCondition(EBuildingProtesterState.Flower);
        }
    }
}
