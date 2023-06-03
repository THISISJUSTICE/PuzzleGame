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

    public int kind; //�������� ����(0: Quadangle 1: Hexagon, 2: Cube, 3: Hive) (�ۿ��� �Է�)
    public StageData[] stageBtns; //������ �������� ��ư
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
        for (int i = 0; i < totalStageCount; i++)
        {
            stageBtns[i] = Instantiate(stageBtn, transform.position, transform.rotation).GetComponent<StageData>();
            stageBtns[i].transform.parent = content.gameObject.transform;
            stageBtns[i].Init(kind, i, gameManager);
            tempRect = stageBtns[i].GetComponent<RectTransform>();
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
                Debug.Log($"ClearStage [i: {0}]");
                stageBtns[i].InputData(true, false, PlayerData.Instance.stageFlips[kind][i], PlayerData.Instance.stageScores[kind][i]);
            }
            //Ŭ������ ���������� �ٷ� ���� �� ��������
            else if (i == PlayerData.Instance.data.clearStage[kind] + 1) {
                stageBtns[i].InputData(false, false);
            }
            //�׿� ������
            else
            {
                stageBtns[i].InputData();
            }
        }

        //Event
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

        //�÷��̾� ������ �ֽ�ȭ
        if(isRenew)
            PlayerData.Instance.UpdateData(kind, curStage, clearFlip, stageBtns[curStage].score, CalTotalScore());
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
