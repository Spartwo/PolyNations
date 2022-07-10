using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timekeep : MonoBehaviour
{

    //Gamespeed Variables
    public float GameSpeed = 1f;
    public GameObject GameSpeedUI;
    public GameObject CurrentTimeUI;
    private string CurrentTime;
    //stored time values
    private int Minutes;
    private int Hours;
    private int Days;
    private int Years;

    public void Start()
    {
        Time.timeScale = 1f;
        //call the repeating timer to update the gametime
        InvokeRepeating("GameTimer", 1f, 1f);
    }
    public void PauseUnpause()
    {   
        //method to toggle timescale to 0 when called
        if (Time.timeScale == 0)
        {
            Time.timeScale = GameSpeed;
        } else {
            Time.timeScale = 0;
        }
    }
    public void FixedUpdate()
    {
        //Takes slider value and sets as gamespeed
        GameSpeed = GameObject.Find("Time_Slider").GetComponent<Slider>().value;
        Time.timeScale = GameSpeed;
        DisplayDate();
    }

    void DisplayDate()
    {
        GameSpeedUI.GetComponent<TMP_Text>().text = GameSpeed.ToString();
        CurrentTimeUI.GetComponent<TMP_Text>().text = CurrentTime;
    }

    void GameTimer()
    {
        //Wait for 1 second by default
        Minutes = Minutes + 1;
        if (Minutes == 61)
        {
            Minutes = 1;
            Hours += 1;
        }
        if (Hours == 25)
        {
            Hours = 1;
            Days += 1;
        }
        if (Days == 366)
        {
            Days = 1;
            Years += 1;
        }

        CurrentTime = 
            "T+" + Years 
            + "y:" + Days.ToString("000") 
            + "d:" + Hours.ToString("00") 
            + "h:" + Minutes.ToString("00") + "m";
    }
}
