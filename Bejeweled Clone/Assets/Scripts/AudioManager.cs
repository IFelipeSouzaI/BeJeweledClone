using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{  
    public Sound[] sfx = new Sound[2];
    public AudioSource music;

    void Awake()
    {
        for(int i = 0; i < sfx.Length; i++){
            sfx[i].source = gameObject.AddComponent<AudioSource>();
            sfx[i].source.clip = sfx[i].clip;
            sfx[i].source.volume = sfx[i].volume;
            sfx[i].source.pitch = sfx[i].pitch;
            sfx[i].source.loop = sfx[i].loop;
        }
    }

    public void Play(string name){
        Sound s = new Sound();
        if(CanvasManager.SFX){
            for(int i = 0; i < sfx.Length; i++){
                if(sfx[i].name == name){
                    s = sfx[i];
                    break;
                }
            }
            if(s == null){
                Debug.Log("Erro: Sfx do not exist");
                return;
            }
            s.source.Play();
        }
    }

    public void MusicOnOff(){
        if(CanvasManager.Music){
            music.Play();
        }else{
            music.Stop();
        }
    }

}
