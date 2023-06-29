using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour
{
    public int stageKind; //4 종류 중 하나의 스테이지
    public int curstage; //현재 스테이지 표시(0부터 시작, UI에는 1을 더한 것을 표시)
    public int score; //스테이지 클리어 계산용 점수
    public bool isClear; //클리어 했는지 여부
    public bool isLock; //잠겨있는지 여부

    public Button opened; //잠겨있지 않을 때 표시
    GameObject locked; //잠겨있을 때 표시
    Text stageTxt; //현재 스테이지를 표시하는 UI
    GameObject starGroup; //별, 별 바탕의 부모
    Image[] scoreStar; //점수에 따라 얻는 별

    int clearFlipCount; //스테이지 클리어 시 뒤집은 횟수

    public void Init(int stageKind, int curstage, GameManager gameManager) {
        this.stageKind = stageKind;
        this.curstage = curstage;
        scoreStar = new Image[3];
        UIInit();
        UISet(gameManager);
    }

    //StageManager에서 호출(필요한 정보 입력)
    public void InputData(bool isClear = false, bool isLock = true, int clearFlipCount = 0, int score = 0) {
        this.isClear = isClear;
        this.isLock = isLock;
        this.clearFlipCount = clearFlipCount;
        this.score = score;

        //입력된 정보를 바탕으로 필요한 정보를 UI에 표시       
        if (isLock) IndicateLock();
        else
        {
            IndicateOpen();
            if (isClear) 
                IndicateStar(score);       
        }
    }

    void UIInit() {
        opened = transform.GetChild(0).GetComponent<Button>();
        locked = transform.GetChild(1).gameObject;
        stageTxt = opened.transform.GetChild(0).GetComponent<Text>();
        starGroup = opened.transform.GetChild(1).gameObject;
        for (int i = 0; i < scoreStar.Length; i++) 
            scoreStar[i] = starGroup.transform.GetChild(i).GetChild(1).GetComponent<Image>();
    }

    //버튼 기능, 텍스트 세팅
    public void UISet(GameManager gameManager) {
        opened.onClick.AddListener(() => StageStart(gameManager));
        if(curstage == 1000) {
            stageTxt.text = "M";
            starGroup.SetActive(false);
        }
        else stageTxt.text = "" + (curstage + 1);
    }

    void StageStart(GameManager gameManager) {
        gameManager.RealDeleteHorse();
        gameManager.GameStart(curstage, stageKind);
    }

    //잠겨 있음을 표시
    public void IndicateLock() {
        opened.gameObject.SetActive(false);
        locked.SetActive(true);
        isLock = true;
    }

    //잠겨 있지 않음을 표시
    public void IndicateOpen() {
        locked.SetActive(false);
        opened.gameObject.SetActive(true);
        isLock = false;
    }

    //스테이지 클리어 시 게임 매니저에서 호출(최소 플립 수, 클리어 시점에서의 플립 수, 최대 플립 수를 인수로 받음, 점수 계산용)
    //갱신할 정보가 있으면 true 반환
    public bool StageClear(int minFlip, int clearFlip, int maxFlip) {
        //최초로 클리어 시 해당 플립 횟수 입력
        if (!isClear)
        {
            clearFlipCount = clearFlip;
            isClear = true;
        }
        //이미 클리어한 경우 더 작은 플립 횟수가 클리어 플립 횟수가 됨
        else
        {
            if (clearFlipCount > clearFlip)
                clearFlipCount = clearFlip;
            
            else return false;
        }
                                                                                                                                                                 
        score = CalScore(minFlip, clearFlipCount, maxFlip);
        IndicateStar(score);

        return true;
    }

    //플립 횟수에 따라서 획득 점수를 결정
    int CalScore(int minFlip, int clearFlip, int maxFlip) {
        int score;
        int middle = (minFlip + maxFlip) / 3;
        int middle2 = (middle + maxFlip) / 2;

        //별 3개
        if (clearFlip <= minFlip) score = 100;
        else
        {
            //별 2개
            if (clearFlip == middle) score = 50;
            //별 2.5개
            else if (clearFlip < middle && clearFlip > minFlip) score = 75;
            //별 1개
            else if (clearFlip == middle2) score = 15;
            //별 1.5개
            else if (clearFlip > middle && clearFlip < middle2) score = 25;
            //별 0.5개
            else score = 5;
        }     
        return score;
    }

    //얻은 점수를 바탕으로 별 표시
    void IndicateStar(int score) {
        switch (score) {
            case 100:
                //별 3개
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 1;
                scoreStar[2].fillAmount = 1;
                break;
            case int n when (n < 100 && n >= 75):
                //별 2.5개
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 1;
                scoreStar[2].fillAmount = 0.5f;
                break;
            case int n when (n < 75 && n >= 50):
                //별 2개
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 1;
                scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 50 && n >= 25):
                //별 1.5개
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 0.5f;
                scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 25 && n >= 15):
                //별 1개
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 0;
                scoreStar[2].fillAmount = 0;
                break;
            case 5:
                //별 0.5개
                scoreStar[0].fillAmount = 0.5f;
                scoreStar[1].fillAmount = 0;
                scoreStar[2].fillAmount = 0;
                break;
        }
    }

}
