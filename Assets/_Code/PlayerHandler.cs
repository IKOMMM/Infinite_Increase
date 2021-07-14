using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHandler : MonoBehaviour
{
    [Header("PlayerMoveHandler")]
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] public float fuelAmount = 1500f;
    [SerializeField] public float maxfuelAmount = 1500f;
    [SerializeField] float fuelReduceByThrust = 5;
    [SerializeField] float fuelIncreaseByFuelBox = 20f;
    [SerializeField] float levelLoadDelay = 2f;
    Rigidbody rigidBody;
    [Header("Audio")]
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    AudioSource audioSource;
    [Header("Particles")]
    [SerializeField] ParticleSystem mainEngineParticles01;
    [SerializeField] ParticleSystem mainEngineParticles02;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;
    [Header("UI Elements")]
    [SerializeField] UIHandler uiHandler;
    [Header("GameState")]
    bool gameIsPaused = false;
    bool gameStarted = false;

    bool isTransitioning = false;
    bool collisionsDisabled = false;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        uiHandler.PauseUI.SetActive(false);
    }    

    // Update is called once per frame
    void Update()
    {

        Debug.Log(fuelAmount);
        StartGameHandler();
        if (gameStarted == true)
        {            
            PauseGameHandler();
        }       

        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }                
    }

    void StartGameHandler()
    {
        if(gameStarted == false)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                uiHandler.StartUI.SetActive(false);
                uiHandler.GamePlayUI.SetActive(true);
                Time.timeScale = 1f;
                gameStarted = true;
            }
            else
            {
                uiHandler.StartUI.SetActive(true);
                uiHandler.GamePlayUI.SetActive(false);
                Time.timeScale = 0f;
                gameStarted = false;
            }
        }
        else
        {
            Console.WriteLine("Pres Space To Start");
        }   
    }
    void PauseGameHandler()
    {
        if(gameIsPaused == false)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Console.WriteLine("ESCAPE Game Is Paused");
                gameIsPaused = true;
            }
        }
        
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionsDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //DO NOTHING
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Fuel":
                fuelAmount += fuelIncreaseByFuelBox;
                if (fuelAmount >= maxfuelAmount)
                {
                    fuelAmount = maxfuelAmount;
                }
                Destroy(collision.gameObject);
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void RespondToThrustInput()
    {       
        
        if (Input.GetKey(KeyCode.Space) && fuelAmount >= 0) // can thrust while rotating
        {            
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }

        if (fuelAmount <= 0)
        {
            Invoke(nameof(LoadFirstLevel), 3f);
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles01.Stop();
        mainEngineParticles02.Stop();
    }

    private void ApplyThrust()
    {
        fuelAmount -= fuelReduceByThrust;
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) // so it doesn't layer
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles01.Play();
        mainEngineParticles02.Play();         
    }

    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero; // remove rotation due to physics

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }
}