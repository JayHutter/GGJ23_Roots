using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud instance;

    public Button mainMenuStartButton;
    public EventSystem eventSystem;

    public Animator anim;

    float timer;

    public TMP_Text timerText;
    public Gradient healthGradient;
    public RawImage healthUi;
    public TMP_Text healthText;
    public TMP_Text tetherText;
    public TMP_Text collectibleText;
    public TMP_Text deathsText;
    public TMP_Text carrotText;

    PlayerController pc;

    private bool isPlaying = false;

    private void Start()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;

        eventSystem.SetSelectedGameObject(mainMenuStartButton.gameObject);
        pc = PlayerController.instance;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
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
            UpdatePlayerValues();
        }
    }

    public void ShowCollectibleTextFor(string text, float time)
    {
        StartCoroutine(ShowTextFor(text, time));
    }

    IEnumerator ShowTextFor(string text, float time)
    {
        collectibleText.text = text;
        yield return new WaitForSeconds(time);
        collectibleText.text = "";
    }

    private void UpdatePlayerValues()
    {
        deathsText.text = "Deaths - " + PlayerController.instance.deaths;
        carrotText.text = "Carrots - " + PlayerController.instance.carrots;   
    }
}
