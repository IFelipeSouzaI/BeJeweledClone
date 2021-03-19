using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class On_OffButton : MonoBehaviour
{
    public Sprite off;
    public Sprite on;
    public Image info;

    private bool On = true;

    public void OnOff(){
        On = !On;
        if(On){
            info.sprite = on;
        }else{
            info.sprite = off;
        }
    }

}
