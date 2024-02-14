using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip car_Claxon;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCarClaxon() => audioSource.PlayOneShot(car_Claxon);
    public void LoadMainScene() => SceneManager.LoadScene("MainScene");
}
