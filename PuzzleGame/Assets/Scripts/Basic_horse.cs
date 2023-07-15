using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basic_horse : MonoBehaviour
{
    public GameObject horseMesh;
    public int kind;

    GameManager gameManager;
    Animator anim;
    Toggle horseSoundToggle;
    int s, u, v; //보드상 좌표 지정

    //초기화
    public void Init(GameManager gameManager){
        anim = horseMesh.GetComponent<Animator>();
        horseSoundToggle = horseMesh.transform.GetChild(0).GetComponent<Toggle>();
        horseSoundToggle.onValueChanged.AddListener(HorseSound);
        this.gameManager = gameManager;
    }

    //좌표 설정
    public void SetCoordinate(int s, int u, int v)
    {
        this.s = s;
        this.u = u;
        this.v = v;
    }
    

    public void PlaySummon(int curState) 
    {
        if (curState == 0)
        {
            anim.Play("BSummon");
            //StartCoroutine(FlipSoundPlay(0.19f, curState));
        }
        else
        {
            anim.Play("WSummon");
            //StartCoroutine(FlipSoundPlay(0.19f, curState));
        }
    }

    //true면 흰색: 0, false면 검은색: 1
    void HorseSound(bool sound){
        if(horseSoundToggle.interactable){
            if(horseSoundToggle.isOn) gameManager.horseAudio.clip = gameManager.flipSounds[0];
            else gameManager.horseAudio.clip = gameManager.flipSounds[1];
            gameManager.horseAudio.Play();
        }
    }

    // //흰색이 바닥에 닿으면 0, 검은색은 1번 클립 재생
    // IEnumerator FlipSoundPlay(float time, int sound){
    //     yield return new WaitForSeconds(time);
    //     gameManager.horseAudio.clip = gameManager.flipSounds[sound];
    //     gameManager.horseAudio.Play();
    // }

    //말이 마우스로 클릭되었을 때 호출
    private void OnMouseDown()
    {
        Debug.Log("Click Horse");
        if (!AnimPlayCheck())
        {
            gameManager.ClickHorse(u, v, s);
        }
    }

    //현재 말의 애니메이션이 재생 중인지 확인하는 함수
    public bool AnimPlayCheck() 
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (((stateInfo.IsTag("Flip") || stateInfo.IsTag("Summon")) && stateInfo.normalizedTime < 1.0f)) return true;
        else return false;  
    }

    //말을 뒤집는 함수
    public int FlipHorse(int curState) 
    {
        //SetAudioClip(curState);
        if (curState == 1)
        {
            curState = 0;
            anim.Play("FlipWtoB");    
            //StartCoroutine(FlipSoundPlay(0.32f, 0));
        }
        else
        {
            curState = 1;
            anim.Play("FlipBtoW");
            //StartCoroutine(FlipSoundPlay(0.32f, 1));
        }
        return curState;
    }

}
