using Game.Building;
using Game.NPC;
using MorningBird.Sound;
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
    [SerializeField] private Transform feet;  
    [SerializeField] private Transform pickupPoint;
    [SerializeField] MorningBird.Sound.AudioStorage jumpSound, getPointSound, backgroundMusic, danceMusic;
    private bool isJumping = false;
    private bool isSliding = false;
    private Rigidbody rb;
    private GameObject heldObject = null;
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 2f;
    private Animator animator;
    private bool checkPlayerGrounded;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();
        SoundManager.Instance.RequestPlayAmbience(danceMusic, transform.position, true, setFollowTarget : transform);
        // Confine and hide the cursor
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        // Calculate the direction of movement based on player input
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        // Walk animation
        if (direction.magnitude > 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        // Adjust the player's velocity while keeping vertical velocity the same
        rb.velocity = transform.TransformDirection(direction) * speed + new Vector3(0, rb.velocity.y, 0);

        float rotation = Input.GetAxis("Mouse X") * rotationSpeed;
        rotation *= Time.deltaTime;

        Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turn);

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            SoundManager.Instance.RequestPlayClip(jumpSound, setFollowTarget : transform);
            isJumping = true;
            animator.SetTrigger("Jump");
            StartCoroutine(jumpCheckDelay());
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
            animator.SetTrigger("Slide");
            StartCoroutine(Slide());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }

        if(Input.GetKeyDown(KeyCode.F)) 
        {
            animator.SetTrigger("Dance");
            StartCoroutine(WaitForAnim(speed, 2f));
        }

        if (isJumping && checkPlayerGrounded)
        {
            TestIfGrounded();
        }
    }

    private void Pickup()
    {
        animator.SetTrigger("Plant");
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
                if(speed != 0)
                    StartCoroutine(WaitForAnim(speed, 1f));
                heldObject = hitColliders[0].gameObject;
                StartCoroutine(WaitForPickup());
            }
        }
    }

    private void TestIfGrounded()
    {
        RaycastHit hit;
        if(Physics.Raycast(feet.position, Vector3.down, out hit, 0.05f))
        {
            if(hit.transform.tag == "Ground")
            {
                isJumping = false;
                checkPlayerGrounded = false;
            }
        }
    }

    IEnumerator jumpCheckDelay()
    {
        yield return new WaitForSeconds(0.1f);
        checkPlayerGrounded = true;
    }

    IEnumerator Slide()
    {
        isSliding = true;
        float oldSpeed = speed;
        speed = slideForce;
        yield return new WaitForSeconds(slideDuration);
        speed = oldSpeed;
        isSliding = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            checkPlayerGrounded=false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dropoff") && heldObject != null && speed != 0)
        {
            // Drop off the object
            animator.SetTrigger("Plant");
            StartCoroutine(WaitForAnim(speed, 0.5f));
            StartCoroutine(WaitForPutDown(other.gameObject));
        }
    }

    private IEnumerator WaitForPutDown(GameObject other)
    {
        yield return new WaitForSeconds(0.5f);
        heldObject.transform.SetParent(null);
        heldObject.transform.localPosition = other.transform.position + new Vector3(0, 1, 0);
        Destroy(heldObject);
        heldObject = null;
        other.GetComponent<Building_FlowerPoint>()?.SetFlowerPointCondition(EBuildingProtesterState.Flower);
        other.GetComponent<Civilion_Common>()?.SetStateOfProtester(ECivilionState.Peace);
        SoundManager.Instance.RequestPlayClip(getPointSound, setFollowTarget: transform);
    }

    private IEnumerator WaitForPickup()
    {
        if (!heldObject)
        {
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(1f);
            if(heldObject != null)
            {
                heldObject.GetComponentInParent<PlantSpawner>().PlantRemoved();
                heldObject.transform.SetParent(pickupPoint);
                heldObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    private IEnumerator WaitForAnim(float tempSpeed, float duration)
    {
        speed = 0;
        yield return new WaitForSeconds(duration);
        speed = tempSpeed;
    }
}
