using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
    // public vars

    public float mouseSensitivityX = 1;
    public float mouseSensitivityY = 1;
    public float walkSpeed = 6;
    public float jumpForce = 220;
    public LayerMask groundedMask;
    public Animator cameraAnim;

    public GameObject[] layers;


    public static float DistanceFromTarget;  //float variable to keep distance from the target 
    public float ToTarget;

    // System vars
    /*[SerializeField]*/
    bool grounded;
    /*[SerializeField]*/
    bool madeCrack;
    /*[SerializeField]*/
    bool madeHole;

    public int upcount =0;
    public int downcount = 0;
    public string holeNum;
    private int[] correctAns;
    private int correctAnsCount;
    bool madeCrackParticle;
    bool madeHoleparticle;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    float verticalLookRotation;
    int jumpCount = 0;
    int flyCount = 0;
    public int presentLayer = 0;
    Transform cameraTransform;
    public Rigidbody rb;
    int wincount = 0;

    void Awake()
    {

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //cameraTransform = Camera.main.transform;
        //rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
       
        // Look rotation:
        //transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
        //verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
        //verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        //cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

        CalculateMovement();


        // Jump
        if (isGrounded())
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpCount++;
                rb.AddForce(transform.up * jumpForce,ForceMode.Impulse);

                if (jumpCount == 1)
                {
                   // Debug.Log("Jump");
                    madeCrack = true;
                    Invoke("MoveAgain", 2.5f);
                }
                else if (jumpCount == 2)
                {
                    madeHole = true;
                    //Debug.Log("Hole");
                    Invoke("MoveAgain", 2.5f);
                    jumpCount = 0;
                }

            }


            if (Input.GetKeyDown(KeyCode.F))
            {

                flyCount++;
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

                if (flyCount == 1)
                {
                    //Debug.Log("Jumpp");
                    madeCrackParticle = true;
                    Invoke("MoveAgainParticle", 2.5f);
                }
                else if (flyCount == 2)
                {
                    madeHoleparticle = true;
                    //Debug.Log("Holep");
                    Invoke("MoveAgainParticle", 2.5f);
                    flyCount = 0;
                }

            }

        }
       
    }
  

    void FixedUpdate()
    {
        // Apply movement to rigidbody

        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }

    void CalculateMovement()
    {

        // Calculate movement:
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
        Vector3 targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

    }


    bool isGrounded()
    {
        // Grounded check
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
        {
            Debug.DrawRay(transform.position, -transform.up);
            // Debug.Log("Grounded");
            return true;
        }
        else
        {

            return false;
        }
     }

    void MoveAgain()
    {
        if (madeCrack)
            madeCrack = false;

        if (madeHole)
            madeHole = false;


    }
    void MoveAgainParticle()
    {
        if (madeCrackParticle)
            madeCrackParticle = false;

        if (madeHoleparticle)
            madeHoleparticle = false;


    }
    public void invokePlanetDown()
    {

        //correctAns[correctAnsCount] = presentLayer;
        layers[presentLayer].gameObject.SetActive(false);
        layers[++presentLayer].gameObject.SetActive(true);
        //correctAnsCount++;
    }
    public void invokePlanetUp()
    {
        Debug.Log(correctAnsCount);
        //correctAns[correctAnsCount] = presentLayer;
        layers[presentLayer].gameObject.SetActive(false);
        layers[--presentLayer].gameObject.SetActive(true);
        //correctAnsCount--;
    }
    public void GameStatus()
    {
        if(presentLayer ==1 && holeNum == "Hole 1" && upcount<=3 )
        {
            ++wincount;
            Debug.Log("Wincount1" + wincount);
        }
        else if (presentLayer == 2 && holeNum == "Hole 3" && upcount <= 3)
        {
            ++wincount;
            Debug.Log("Wincount2" + wincount);
        }
        else if (presentLayer == 3 && holeNum == "Hole 4" && upcount <= 3)
        {
            ++wincount;
            Debug.Log("Wincount3" + wincount);
        }
        else if (presentLayer == 4 && upcount <= 3)
        {
            ++wincount;
            Debug.Log("Wincount4" + wincount);
        }
        if(upcount>3 ||downcount >=6 )
        {
            Debug.Log("GameOver");
        }
        if (wincount == 3)
        {
            Debug.Log("Has won");
        }
      

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
            holeNum= other.gameObject.name;
           // Debug.Log(holeNum);
            ++downcount;
            Invoke("invokePlanetDown", 0.5f);
            Invoke("GameStatus", 1.5f);
            Debug.Log("downcount" + downcount);
         
           
        }
        if (other.gameObject.tag == "UpStream" && madeCrackParticle)
        {
            Debug.Log("crack trigger particle");
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
             //cameraAnim.SetTrigger("CameraShake");

        }

        if (other.gameObject.tag == "UpStream" && madeHoleparticle)
        {
            Debug.Log("hole trigger particle");
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            ++upcount;
            Invoke("invokePlanetUp", 0.5f);

            Invoke("GameStatus", 1.5f);
           
            Debug.Log("upcount" + upcount);
           
        }

     
    }

}


