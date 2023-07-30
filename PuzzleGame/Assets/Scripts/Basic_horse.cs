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
    AudioSource horseAudio;
    int s, u, v; //����� ��ǥ ����

    //�ʱ�ȭ
    public void Init(GameManager gameManager){
        anim = horseMesh.GetComponent<Animator>();
        horseSoundToggle = horseMesh.transform.GetChild(0).GetComponent<Toggle>();
        horseSoundToggle.onValueChanged.AddListener(HorseSound);
        horseAudio = GetComponent<AudioSource>();
        this.gameManager = gameManager;
    }

    //��ǥ, �Ҹ� ����
    public void SetCoordinate(int s, int u, int v, bool mute, float volume)
    {
        this.s = s;
        this.u = u;
        this.v = v;
        horseAudio.mute = mute;
        horseAudio.volume = volume;
    }
    

    public void PlaySummon(int curState) 
    {
        if (curState == 0)
        {
            anim.Play("BSummon");
        }
        else
        {
            anim.Play("WSummon");
        }
    }

    //true�� ���: 0, false�� ������: 1
    void HorseSound(bool sound){
        if(horseSoundToggle.interactable){
            if(horseSoundToggle.isOn) horseAudio.clip = gameManager.flipSounds[0];
            else horseAudio.clip = gameManager.flipSounds[1];
            horseAudio.Play();
        }
    }

    public void SetHorseSoundMute(bool mute){
        horseAudio.mute = mute;
    }
    public void SetHorseSoundVolume(float volume){
        horseAudio.volume = volume;
    }

    //���� ���콺�� Ŭ���Ǿ��� �� ȣ��
    private void OnMouseDown()
    {
        if (!AnimPlayCheck())
        {
            gameManager.ClickHorse(u, v, s);
        }
    }

    //���� ���� �ִϸ��̼��� ��� ������ Ȯ���ϴ� �Լ�
    public bool AnimPlayCheck() 
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (((stateInfo.IsTag("Flip") || stateInfo.IsTag("Summon")) && stateInfo.normalizedTime < 1.0f)) return true;
        else return false;  
    }

    //���� ������ �Լ�
    public int FlipHorse(int curState) 
    {
        if (curState == 1)
        {
            curState = 0;
            anim.Play("FlipWtoB");    
        }
        else
        {
            curState = 1;
            anim.Play("FlipBtoW");
        }
        return curState;
    }

}
