using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageData : MonoBehaviour
{
    public int stageKind; //4 ���� �� �ϳ��� ��������
    public int curstage; //���� �������� ǥ��(0���� ����, UI���� 1�� ���� ���� ǥ��)
    public int score; //�������� Ŭ���� ���� ����
    public bool isClear; //Ŭ���� �ߴ��� ����
    public bool isLock; //����ִ��� ����

    public Button opened; //������� ���� �� ǥ��
    GameObject locked; //������� �� ǥ��
    Text stageTxt; //���� ���������� ǥ���ϴ� UI
    GameObject starGroup; //��, �� ������ �θ�
    Image[] scoreStar; //������ ���� ��� ��

    int clearFlipCount; //�������� Ŭ���� �� ������ Ƚ��

    public void Init(int stageKind, int curstage, GameManager gameManager) {
        this.stageKind = stageKind;
        this.curstage = curstage;
        scoreStar = new Image[3];
        UIInit();
        UISet(gameManager);
    }

    //StageManager���� ȣ��(�ʿ��� ���� �Է�)
    public void InputData(bool isClear = false, bool isLock = true, int clearFlipCount = 0, int score = 0) {
        this.isClear = isClear;
        this.isLock = isLock;
        this.clearFlipCount = clearFlipCount;
        this.score = score;

        //�Էµ� ������ �������� �ʿ��� ������ UI�� ǥ��       
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

    //��ư ���, �ؽ�Ʈ ����
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

    //��� ������ ǥ��
    public void IndicateLock() {
        opened.gameObject.SetActive(false);
        locked.SetActive(true);
        isLock = true;
    }

    //��� ���� ������ ǥ��
    public void IndicateOpen() {
        locked.SetActive(false);
        opened.gameObject.SetActive(true);
        isLock = false;
    }

    //�������� Ŭ���� �� ���� �Ŵ������� ȣ��(�ּ� �ø� ��, Ŭ���� ���������� �ø� ��, �ִ� �ø� ���� �μ��� ����, ���� ����)
    //������ ������ ������ true ��ȯ
    public bool StageClear(int minFlip, int clearFlip, int maxFlip) {
        //���ʷ� Ŭ���� �� �ش� �ø� Ƚ�� �Է�
        if (!isClear)
        {
            clearFlipCount = clearFlip;
            isClear = true;
        }
        //�̹� Ŭ������ ��� �� ���� �ø� Ƚ���� Ŭ���� �ø� Ƚ���� ��
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

    //�ø� Ƚ���� ���� ȹ�� ������ ����
    int CalScore(int minFlip, int clearFlip, int maxFlip) {
        int score;
        int middle = (minFlip + maxFlip) / 3;
        int middle2 = (middle + maxFlip) / 2;

        //�� 3��
        if (clearFlip <= minFlip) score = 100;
        else
        {
            //�� 2��
            if (clearFlip == middle) score = 50;
            //�� 2.5��
            else if (clearFlip < middle && clearFlip > minFlip) score = 75;
            //�� 1��
            else if (clearFlip == middle2) score = 15;
            //�� 1.5��
            else if (clearFlip > middle && clearFlip < middle2) score = 25;
            //�� 0.5��
            else score = 5;
        }     
        return score;
    }

    //���� ������ �������� �� ǥ��
    void IndicateStar(int score) {
        switch (score) {
            case 100:
                //�� 3��
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 1;
                scoreStar[2].fillAmount = 1;
                break;
            case int n when (n < 100 && n >= 75):
                //�� 2.5��
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 1;
                scoreStar[2].fillAmount = 0.5f;
                break;
            case int n when (n < 75 && n >= 50):
                //�� 2��
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 1;
                scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 50 && n >= 25):
                //�� 1.5��
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 0.5f;
                scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 25 && n >= 15):
                //�� 1��
                scoreStar[0].fillAmount = 1;
                scoreStar[1].fillAmount = 0;
                scoreStar[2].fillAmount = 0;
                break;
            case 5:
                //�� 0.5��
                scoreStar[0].fillAmount = 0.5f;
                scoreStar[1].fillAmount = 0;
                scoreStar[2].fillAmount = 0;
                break;
        }
    }

}
