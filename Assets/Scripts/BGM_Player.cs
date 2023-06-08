using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM_Player : MonoBehaviour
{
    public List<AudioClip> bgms; //재생할 음악 클립(0은 메인 메뉴UI에서, 1은 인게임에서 재생)
    public AudioSource bgmAudio;
}
