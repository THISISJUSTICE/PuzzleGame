using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Player : MonoBehaviour
{
    public List<AudioClip> bgms; //재생할 음악 클립
    public AudioSource bgmAudio;
    public int playKind; //재생할 음악의 종류 선정

    void Start(){
        bgmAudio = GetComponent<AudioSource>();
        playKind = 0;
    }

    private void FixedUpdate() {
        if(!bgmAudio.isPlaying){

        }
    }

}
