using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Text scoreText;
    public Text bstComboText;

    private int bonusAmount = 0;
    public Slider bonusBar;

    public GameObject optionsWindow;    
    public static bool isPaused = false;
    public static bool SFX = true;
    public static bool Music = true; 

    public void setInfo(int score, int combo, int bestCombo)
    {
        scoreText.text = score.ToString();
        bonusAmount += combo;
        if(bonusAmount > 1000){
            FindObjectOfType<AudioManager>().Play("BonusEffect");
            bonusAmount -= 1000;
        }
        bonusBar.value = bonusAmount;
        bstComboText.text = bestCombo.ToString();
    }

    public void Pause(){
        FindObjectOfType<AudioManager>().Play("Button");
        isPaused = !isPaused;
        optionsWindow.SetActive(isPaused);
        if(isPaused){
            Time.timeScale = 0f;
        }else{
            Time.timeScale = 1f;
        }
    }

    public void QuitGame(){
        FindObjectOfType<AudioManager>().Play("Button");
        Application.Quit();
    }


}
