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
    int s, u, v; //����� ��ǥ ����

    //�ʱ�ȭ
    public void Init(GameManager gameManager){
        anim = horseMesh.GetComponent<Animator>();
        horseSoundToggle = horseMesh.transform.GetChild(0).GetComponent<Toggle>();
        horseSoundToggle.onValueChanged.AddListener(HorseSound);
        this.gameManager = gameManager;
    }

    //��ǥ ����
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

    //true�� ���: 0, false�� ������: 1
    void HorseSound(bool sound){
        if(horseSoundToggle.interactable){
            if(horseSoundToggle.isOn) gameManager.horseAudio.clip = gameManager.flipSounds[0];
            else gameManager.horseAudio.clip = gameManager.flipSounds[1];
            gameManager.horseAudio.Play();
        }
    }

    // //����� �ٴڿ� ������ 0, �������� 1�� Ŭ�� ���
    // IEnumerator FlipSoundPlay(float time, int sound){
    //     yield return new WaitForSeconds(time);
    //     gameManager.horseAudio.clip = gameManager.flipSounds[sound];
    //     gameManager.horseAudio.Play();
    // }

    //���� ���콺�� Ŭ���Ǿ��� �� ȣ��
    private void OnMouseDown()
    {
        Debug.Log("Click Horse");
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
