using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class StageManager : MonoBehaviour
{
    //������ ���޿�
    public GameManager gameManager; 
    public GameObject stageBtn; //��ư �����տ� ������ ����(gameManager, playerData)
    public RectTransform content; //��ũ�� ���� ������Ʈ

    public int kind; //�������� ����(0: Quadangle 1: Hexagon, 2: Cube) (�ۿ��� �Է�)
    public StageData[] stageBtns; //������ �������� ��ư

    public StageData masterStageBtn; //������ ��� �������� ��ư
    int totalStageCount; //�� ���������� ��

    private void Awake()
    {
        Init();
    }

    void Init() {
        totalStageCount = PlayerData.Instance.totalStageCount[kind];
        stageBtns = new StageData[totalStageCount];  

        //��ũ�� ����
        int scrollHeight = 0;
        if (totalStageCount / 4 > 5) scrollHeight = (totalStageCount / 4) - 5;
        content.sizeDelta = new Vector2(1424, 1900 + (300 * scrollHeight));
        CreateBtns();
        InputData_to_Btns();
    }

    //�������� ���� �������� �������� ��ư ����, ������ ��ġ�� ��ġ
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
            //������ ��� ��������
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

    //playerData�� ������ ���� ��ư ���� �Է�
    void InputData_to_Btns() {
        for (int i = 0; i < stageBtns.Length; i++) {
            //Ŭ������ ��������
            if (i <= PlayerData.Instance.data.clearStage[kind])
            {
                stageBtns[i].InputData(true, false, PlayerData.Instance.stageFlips[kind][i], PlayerData.Instance.stageScores[kind][i]);
            }
            //Ŭ������ ���������� �ٷ� ���� �� ��������
            else if (i == PlayerData.Instance.data.clearStage[kind] + 1) {
                stageBtns[i].InputData(false, false);
            }
            //�׿� ������
            else
                stageBtns[i].InputData();
        }

        //������ ��� ��ư
        if(PlayerData.Instance.data.clearStage[kind] == totalStageCount - 1){
            masterStageBtn.InputData(false, false,0, PlayerData.Instance.data.masterMaxScore[kind]);
        }
        else masterStageBtn.InputData();
    }

    //�������� Ŭ���� �� ���� �Ŵ������� ȣ��
    public bool CurrentStageClear(int curStage, int minFlip, int clearFlip, int maxFlip) {
        bool isRenew;

        //��������, �ø� ���� �μ��� �޾� �ش��ϴ� �������� ��ư ���� �ֽ�ȭ(������ �ֽ�ȭ�� �ʿ䰡 ������ true ��ȯ)
        isRenew = stageBtns[curStage].StageClear(minFlip, clearFlip, maxFlip);

        //Ŭ������ ���� �������� ��� ����
        if (curStage < stageBtns.Length - 1) {
            if (stageBtns[curStage + 1].isLock) {
                stageBtns[curStage + 1].IndicateOpen();
            }
        }
        //������ ��� ��� ����
        else {
            if(masterStageBtn.isLock)
                masterStageBtn.IndicateOpen();
        }

        //�÷��̾� ������ �ֽ�ȭ
        if(isRenew){

            //���� Ŭ���� ��
            if (PlayerData.Instance.data.clearStage[kind] < curStage)
            {
                PlayerData.Instance.data.clearStage[kind]++;
                PlayerData.Instance.stageFlips[kind].Add(clearFlip);
                PlayerData.Instance.stageScores[kind].Add(stageBtns[curStage].score);
            }
            //�̹� Ŭ������ ���������� Ŭ���� ���� ��� + �������� ���� ������ ������� ���� ��
            else if (PlayerData.Instance.stageFlips[kind][curStage] <= clearFlip) return isRenew;
            //�̹� Ŭ������ ���������� Ŭ���� ���� ��� + �������� ���� ������ ����� ��
            else
            {
                PlayerData.Instance.stageFlips[kind][curStage] = clearFlip;
                PlayerData.Instance.stageScores[kind][curStage] = stageBtns[curStage].score;
            }
            PlayerData.Instance.UpdateData(kind, curStage, clearFlip, stageBtns[curStage].score, CalTotalScore());
        }
        return isRenew;
    }

    //���� ���������� ��� ������ �ջ��ϴ� �Լ�
    int CalTotalScore() {
        int totalScore = 0;
        for (int i=0; i <= PlayerData.Instance.data.clearStage[kind]; i++) {
            totalScore += StageScore(i);
        }
        return totalScore;
    }

    //�������� �߰� �������� �����Ͽ� �� ���������� �� ���� ���
    public int StageScore(int clearStage) {
        int res = (clearStage + 2) * (clearStage + 3) / (clearStage + 1) + (((kind + 1) * (clearStage + 1)) / 2);
        return res + stageBtns[clearStage].score;
    }

}
