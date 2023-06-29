using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_horse : MonoBehaviour
{
    public GameObject horseMesh;
    public int kind;
    
    GameManager gameManager;
    Animator anim;
    int s, u, v; //����� ��ǥ ����

    public void Init(int u, int v, GameManager gameManager, int s = 0)
    {
        anim = horseMesh.GetComponent<Animator>();
        this.s = s;
        this.u = u;
        this.v = v;
        this.gameManager = gameManager;
    }

    public void PlaySummon(int curState) 
    {
        if (curState == 0)
        {
            anim.Play("BSummon");
            StartCoroutine(FlipSoundPlay(0.19f, 0));
        }
        else
        {
            anim.Play("WSummon");
            StartCoroutine(FlipSoundPlay(0.19f, 1));
        }
    }

    //����� �ٴڿ� ������ 0, �������� 1�� Ŭ�� ���
    IEnumerator FlipSoundPlay(float time, int sound){
        yield return new WaitForSeconds(time);
        gameManager.horseAudio.clip = gameManager.flipSounds[sound];
        gameManager.horseAudio.Play();
    }

    //���� ���콺�� Ŭ���Ǿ��� �� ȣ��
    private void OnMouseDown()
    {
        if (!AnimPlayCheck())
        {
            StartCoroutine(gameManager.Flip_in_Board(u, v, s));
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
            StartCoroutine(FlipSoundPlay(0.32f, 0));
        }
        else
        {
            curState = 1;
            anim.Play("FlipBtoW");
            StartCoroutine(FlipSoundPlay(0.32f, 1));
        }
        return curState;
    }

}
