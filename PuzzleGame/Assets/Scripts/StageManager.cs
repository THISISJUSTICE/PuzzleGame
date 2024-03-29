using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class StageManager : MonoBehaviour
{
    //프리팹 전달용
    public GameManager gameManager; 
    public GameObject stageBtn; //버튼 프리팹에 전달할 변수(gameManager, playerData)
    public RectTransform content; //스크롤 하위 오브젝트

    public int kind; //스테이지 종류(0: Quadangle 1: Hexagon, 2: Cube) (밖에서 입력)
    public StageData[] stageBtns; //생성한 스테이지 버튼

    public StageData masterStageBtn; //마스터 모드 스테이지 버튼
    int totalStageCount; //총 스테이지의 수

    private void Awake()
    {
        Init();
    }

    void Init() {
        totalStageCount = PlayerData.Instance.totalStageCount[kind];
        stageBtns = new StageData[totalStageCount];  

        //스크롤 관련
        int scrollHeight = 0;
        if (totalStageCount / 4 > 5) scrollHeight = (totalStageCount / 4) - 5;
        content.sizeDelta = new Vector2(1424, 1900 + (300 * scrollHeight));
        content.position = new Vector3(0, -3000, 0);
        CreateBtns();
        InputData_to_Btns();
    }

    //스테이지 수를 바탕으로 스테이지 버튼 생성, 적절한 위치에 배치
    void CreateBtns() {
        RectTransform tempRect;
        for (int i = 0; i <= totalStageCount; i++)
        {   
            if(i < totalStageCount){
                stageBtns[i] = Instantiate(stageBtn, transform.position, transform.rotation).GetComponent<StageData>();
                stageBtns[i].transform.parent = content.gameObject.transform;
                stageBtns[i].Init(kind, i, gameManager);
                tempRect = stageBtns[i].GetComponent<RectTransform>();
            }
            //마스터 모드 스테이지
            else{
                masterStageBtn = Instantiate(stageBtn, transform.position, transform.rotation).GetComponent<StageData>();
                masterStageBtn.transform.parent = content.gameObject.transform;
                masterStageBtn.Init(kind, 1000, gameManager);
                tempRect = masterStageBtn.GetComponent<RectTransform>();
            }
            tempRect.anchoredPosition = new Vector2(200 + (300 * (i%4)), -200 - (300 * (i / 4)));
            tempRect.localScale = new Vector3(1, 1, 1);
        }
    }

    //playerData의 정보를 토대로 버튼 정보 입력
    void InputData_to_Btns() {
        for (int i = 0; i < stageBtns.Length; i++) {
            //클리어한 스테이지
            if (i <= PlayerData.Instance.data.clearStage[kind])
            {
                stageBtns[i].InputData(true, false, PlayerData.Instance.stageFlips[kind][i], PlayerData.Instance.stageScores[kind][i]);
            }
            //클리어한 스테이지의 바로 다음 한 스테이지
            else if (i == PlayerData.Instance.data.clearStage[kind] + 1) {
                stageBtns[i].InputData(false, false);
            }
            //그외 나머지
            else
                stageBtns[i].InputData();
        }

        //마스터 모드 버튼
        if(PlayerData.Instance.data.clearStage[kind] == totalStageCount - 1){
            masterStageBtn.InputData(false, false,0, PlayerData.Instance.data.masterMaxScore[kind]);
        }
        else masterStageBtn.InputData();
    }

    //스테이지 클리어 시 게임 매니저에서 호출
    public bool CurrentStageClear(int curStage, int minFlip, int clearFlip, int maxFlip) {
        bool isRenew;

        //스테이지, 플립 관련 인수를 받아 해당하는 스테이지 버튼 정보 최신화(정보를 최신화할 필요가 있으면 true 반환)
        isRenew = stageBtns[curStage].StageClear(minFlip, clearFlip, maxFlip);

        //클리어한 다음 스테이지 잠금 해제
        if (curStage < stageBtns.Length - 1) {
            if (stageBtns[curStage + 1].isLock) {
                stageBtns[curStage + 1].IndicateOpen();
            }
        }
        //마스터 모드 잠금 해제
        else {
            if(masterStageBtn.isLock)
                masterStageBtn.IndicateOpen();
        }

        //플레이어 데이터 최신화
        if(isRenew){

            //최초 클리어 시
            if (PlayerData.Instance.data.clearStage[kind] < curStage)
            {
                PlayerData.Instance.data.clearStage[kind]++;
                PlayerData.Instance.stageFlips[kind].Add(clearFlip);
                PlayerData.Instance.stageScores[kind].Add(stageBtns[curStage].score);
            }
            //이미 클리어한 스테이지를 클리어 했을 경우 + 기존보다 높은 점수를 기록하지 못할 시
            else if (PlayerData.Instance.stageFlips[kind][curStage] <= clearFlip) return isRenew;
            //이미 클리어한 스테이지를 클리어 했을 경우 + 기존보다 높은 점수를 기록할 시
            else
            {
                PlayerData.Instance.stageFlips[kind][curStage] = clearFlip;
                PlayerData.Instance.stageScores[kind][curStage] = stageBtns[curStage].score;
            }
            PlayerData.Instance.UpdateData(kind, curStage, clearFlip, stageBtns[curStage].score, CalTotalScore());
        }
        return isRenew;
    }

    //하위 스테이지의 모든 점수를 합산하는 함수
    int CalTotalScore() {
        int totalScore = 0;
        for (int i=0; i <= PlayerData.Instance.data.clearStage[kind]; i++) {
            totalScore += StageScore(i);
        }
        return totalScore;
    }

    //스테이지 추가 점수까지 포함하여 한 스테이지의 총 점수 계산
    public int StageScore(int clearStage) {
        int res = (clearStage + 2) * (clearStage + 3) / (clearStage + 1) + (((kind + 1) * (clearStage + 1)) / 2);
        return res + stageBtns[clearStage].score;
    }

}
