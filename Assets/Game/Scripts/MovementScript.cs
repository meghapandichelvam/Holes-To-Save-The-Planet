using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{

    public float walkSpeed = 6;
    public float jumpForce =10;
    public LayerMask groundMask;
    public Animator CameraAnim;
    public GameObject[] layers;

    [SerializeField] bool grounded;
    [SerializeField] bool madeCrack;
    [SerializeField] bool madeHole;

    public Rigidbody rb;
    public int presentLayer;
    Vector3 MoveAmount;
    Vector3 smoothMoveVelocity;
    float VetricalLookVelocity;
    int jumpCount = 0;
    Transform cameraTransform;
    
    void Start()
    {
        
    }

  
    void Update()
    {
        CalculateMovement();

       // Vector3 localMove = transform.TransformDirection(MoveAmount) * Time.deltaTime;
       //rb.MovePosition(rb.position + localMove);
        if (isGrounded())
        {
            if(Input.GetKey(KeyCode.Space))
            {
                Debug.Log("Jumpouter");
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                if (jumpCount == 1)
                {
                    Debug.Log("Jump");
                    madeCrack = true;
                    Invoke("MoveAgain", 1.5f);
                }
                else if (jumpCount == 2)
                {
                    madeHole = true;
                    Debug.Log("Hole");
                    Invoke("MoveAgain", 1.5f);
                    jumpCount = 0;
                }
            }
        }
    }

    public void CalculateMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;

        Vector3 targetMovement = moveDir * walkSpeed;
        MoveAmount = Vector3.SmoothDamp(MoveAmount, targetMovement, ref smoothMoveVelocity, 0.15f);

    }
    public bool isGrounded()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit ,1+0.1f,groundMask))
        {
            Debug.DrawRay(transform.position, -transform.up);
            //Debug.Log("Grounded");
            return true;
        }
        else
        {
            return false;
        }

    }
    public void FixedUpdate()
    {
        Vector3 localMove = transform.TransformDirection(MoveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }

    void MoveAgain()
    {
        if (madeCrack)
            madeCrack = false;

        if (madeHole)
            madeHole = false;


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hole" && madeCrack)
        {
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            //cameraAnim.SetTrigger("CameraShake");


        }

        if (other.gameObject.tag == "Hole" && madeHole)
        {
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            //other.gameObject.GetComponent<Renderer>().material.color = Color.black;
            //cameraAnim.SetTrigger("CameraShake");
            //layers[currentLayer].gameObject.SetActive(false);
            //layers[currentLayer++].gameObject.SetActive(true);


            if (presentLayer == 0)
            {

                layers[0].gameObject.SetActive(false);
                layers[1].gameObject.SetActive(true);
                presentLayer = presentLayer + 1;

            }
            else if (presentLayer == 1)
            {

                layers[1].gameObject.SetActive(false);
                layers[2].gameObject.SetActive(true);
                presentLayer = presentLayer + 1;

            }
            else if (presentLayer == 2)
            {

                layers[2].gameObject.SetActive(false);
                layers[3].gameObject.SetActive(true);
                presentLayer = presentLayer + 1;
            }
        }
    }
}
