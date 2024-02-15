using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrologueManager : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip car_Claxon;

    private Animator animator;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void PlayCarClaxon() => audioSource.PlayOneShot(car_Claxon);
    public void LoadMainScene() => SceneManager.LoadScene("MainScene");

    public void StartButtonClick() => animator.SetTrigger("StartPrologue");
    public void EndButtonClick()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
}
