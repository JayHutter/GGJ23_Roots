using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public Button mainMenuStartButton;
    public EventSystem eventSystem;

    public Animator anim;

    float timer;

    public TMP_Text timerText;
    public Gradient healthGradient;
    public RawImage healthUi;
    public TMP_Text healthText;
    public TMP_Text tetherText;

    PlayerController pc;

    private bool isPlaying = false;

    private void Start()
    {
        eventSystem.SetSelectedGameObject(mainMenuStartButton.gameObject);
        StartCoroutine(GameTimer());
        pc = PlayerController.instance;

        isPlaying = true;
    }

    public void StartGame()
    {
        anim.SetTrigger("Start Game");
        eventSystem.SetSelectedGameObject(null);
        StartCoroutine(GameTimer());
        isPlaying = true;
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void ShowOptions()
    {

    }
    
    private IEnumerator GameTimer()
    {
        while(true)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString();

            yield return null;
        }
    }

    private void UpdateHealth()
    {
        var color = healthGradient.Evaluate(Mathf.InverseLerp(0, pc.maxHealth, pc.health));
        healthUi.color = color;
        healthText.text = pc.health.ToString();
    }

    private void UpdateTether()
    {
        tetherText.text = "Length - " + pc.tether.GetLength();
    }

    private void Update()
    {
        if (isPlaying)
        {
            UpdateHealth();
            UpdateTether();
        }
    }

}
