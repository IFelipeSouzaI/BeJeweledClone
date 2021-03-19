using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{

    public Sprite off;
    public Sprite on;
    public Button button;

    private bool On = true;

    public void OnOff(){
        On = !On;
        if(On){
            //button.position.x = 21;
            //button.selectedSprite = on;
        }else{
            button.rectTransform.position.x = -21;
            //button.selectedSprite = off;
        }
    }


}