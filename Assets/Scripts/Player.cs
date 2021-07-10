using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{

    //INSTANCE
    public static Player Instance;

    //PLAYER RIGIDBODY
    private Rigidbody rb;

    //PLAYER SPEED
    public float playerSpeed = 5f;

    //GAME STATUS
    public bool isGameStarted = false;
    public bool isDead = false;
    public bool isPlayerFinish = false;
    public Animator animator;

    //GAME VALUES
    public int heelCount = 1;
    public float heelHeightConstant = 2.5f;


    //POSITION VALUES
    public Vector2 mouseStartPos;
    public Vector2 mouseActualPos;

    //GAME OBJECTS
    public GameObject leftHeel;
    public GameObject rightHeel;
    public GameObject tutorialCanvas;

    public List<GameObject> Heels = new List<GameObject>();


    void Awake()
    {
        //SET INSTANCE
        Instance = this;

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Heels.Add(leftHeel);
        Heels.Add(rightHeel);

    }


    void Update()
    {
        //Sets Players Speed

        if (isGameStarted && !isDead && !isPlayerFinish )
        {
            transform.position += Vector3.forward * playerSpeed * Time.deltaTime;

        }

        //Sets Players Direction

        if (Input.GetMouseButtonDown(0))
        {
            mouseStartPos = Input.mousePosition;

        }
      
        if (Input.GetMouseButton(0) && !isDead && !isPlayerFinish)
        {
           
            isGameStarted = true;

            tutorialCanvas.SetActive(false);

            animator.SetBool("isWalking", true);

            mouseActualPos = Input.mousePosition;

            Vector3 mouseDifPos = mouseActualPos - mouseStartPos;
      
            transform.position += new Vector3(mouseDifPos.x / 500, 0, 0);
            mouseStartPos = Input.mousePosition;

        }

         // Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y + 2, transform.position.z - 2);
          
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") )
        {
             
            if(heelCount - collision.gameObject.GetComponent<Obstacle>().obstacleHeight > 0)
            {
                collision.gameObject.tag = "Untagged";
                heelCount -= collision.gameObject.GetComponent<Obstacle>().obstacleHeight;

                ChangeHeelHeight();
            }
            else
            {
                isDead = true;
                animator.SetBool("isDying", true);

                StartCoroutine(RestartLevel());
            }
            
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            isPlayerFinish = true;
            animator.SetBool("isFinish", true);
            animator.SetBool("isWalking", false);
            StartCoroutine(RestartLevel());

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("shoe"))
        {
            heelCount++;
            Destroy(other.gameObject);
            ChangeHeelHeight();
        }

        if (other.gameObject.CompareTag("LegStretchEnter"))
        {
            animator.SetBool("isLegStretching", true);
        }

        if (other.gameObject.CompareTag("LegStretchExit"))
        {
            rb.AddForce(Vector3.up * 2 , ForceMode.VelocityChange);
            animator.SetBool("isLegStretching", false);
        }

    }
    

    public void ChangeHeelHeight()
    {
        for (int i = 0; i < 2; i++)
        {
            Heels[i].gameObject.transform.localScale = new Vector3(Heels[i].gameObject.transform.localScale.x, heelHeightConstant * heelCount, Heels[i].gameObject.transform.localScale.z);
        }
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3);

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentScene);
    }


}

