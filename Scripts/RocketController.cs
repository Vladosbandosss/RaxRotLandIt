using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketController : MonoBehaviour
{
    [SerializeField] private float forceAmmount = 50;
    [SerializeField] private float rotateAmmount = 100f;

    [SerializeField] private AudioClip rocketSound,winSound,loseSound;

    [SerializeField] private ParticleSystem rocketPartical, winPartical, losePartical;

    private float transitionTime = 2f;

    private enum State
    {
        WIN,
        LOSE,
        ALIVE
    }

    private State playerState = State.ALIVE;

    private AudioSource audioSource;
    private Rigidbody rb;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == State.ALIVE)
        {
            HandleRotate();
        }
    }

    private void FixedUpdate()
    {
        if (playerState == State.ALIVE)
        {
            HandleMovement();
        }
    }

    private void HandleRotate()
    {
        rb.freezeRotation = true;

        float rotationThisFrame = rotateAmmount * Time.deltaTime;

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rb.freezeRotation = false;
    }

    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * forceAmmount);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(rocketSound);
            }

            rocketPartical.Play();
        }
        else
        {
            rocketPartical.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerState != State.ALIVE)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":

                break;

            case "Finish":
                LevelFinished();
                break;

                default:
                PlayerDied();
                break;
        }
    }

    private void PlayerDied()
    {
        playerState = State.LOSE;
        audioSource.Stop();
        audioSource.PlayOneShot(loseSound);
        losePartical.Play();
        Invoke("RestartLevel", transitionTime);
    }

    private void LevelFinished()
    {
        playerState = State.WIN;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winPartical.Play();
        Invoke("LoadNextLevel", transitionTime);
    }

    void LoadNextLevel()
    {
        int currendScenePlusNextScene = SceneManager.GetActiveScene().buildIndex;
        currendScenePlusNextScene += 1;
        int count = SceneManager.sceneCountInBuildSettings;

        if (currendScenePlusNextScene == count)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(currendScenePlusNextScene);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
