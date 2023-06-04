using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic_horse : MonoBehaviour
{
    public GameObject horseMesh;
    public int kind;
    
    GameManager gameManager;
    Animator anim;
    int s, u, v; //보드상 좌표 지정

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
        }
        else
        {
            anim.Play("WSummon");
        }
        
    }

    //말이 마우스로 클릭되었을 때 호출
    private void OnMouseDown()
    {
        if (!AnimPlayCheck())
        {
            StartCoroutine(gameManager.Flip_in_Board(u, v, s));
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
