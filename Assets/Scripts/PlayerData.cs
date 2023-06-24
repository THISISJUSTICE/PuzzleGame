using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

// - ��ŷ ���

public class PlayerData : MonoBehaviour
{
    private static PlayerData instance = null; //�̱��� ����

    //���� �Ŵ��� �ν��Ͻ��� ������ �� �ִ� ������Ƽ. static�̹Ƿ� �ٸ� Ŭ�������� ���� ȣ���� �� �ִ�.
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public class Data
    {
        public string userName; //�÷��̾� �г���
        public int[] stageTotalScore; //�� ����
        public int[] clearStage; //�� �������� �� ������� Ŭ���� �� ��������
        //����� ������
        public List<int> stage0Flip; //0�������� �� Ŭ���� �� �ø� Ƚ��
        public List<int> stage1Flip; //1�������� �� Ŭ���� �� �ø� Ƚ��
        public List<int> stage2Flip; //2�������� �� Ŭ���� �� �ø� Ƚ��
        public List<int> stage0Score; //0�������� �� Ŭ���� �� ����
        public List<int> stage1Score; //1�������� �� Ŭ���� �� ����
        public List<int> stage2Score; //2�������� �� Ŭ���� �� ����

        public int[] masterCurrentClear; //������ ����� ���� Ŭ������ Ƚ��
        public int[] masterMaxScore; //������ ��忡�� ���ݱ��� �ִ�� ���� ����
        public int[] masterCurrentScore; //������ ��忡�� ���� ���� ����
        public bool[] isMasterDoing; //���� ���� ������ ��尡 �ִ��� �Ǵ�
        //����, ���� ���� �ø� Ƚ���� �ؽ�Ʈ ���Ͽ� ����

        public bool soundCk;
        public float soundVolume;
        public bool bgmCk;
        public float bgmVolume;

        public Data()
        {
            stageTotalScore = new int[3];
            clearStage = new int[3];
            masterCurrentClear = new int[3];
            masterMaxScore = new int[3];
            masterCurrentScore = new int[3];
            isMasterDoing = new bool[3];

            stage0Flip = new List<int>();
            stage1Flip = new List<int>();
            stage2Flip = new List<int>();
            stage0Score = new List<int>();
            stage1Score = new List<int>();
            stage2Score = new List<int>();
        }
    }

    string FileName = "GameData.json"; //���� ������ ���� �̸� ����
    public Data data;
    public GameManager gameManager;

    public int[] totalStageCount; //�� �������� �� �� �������� ��
    public List<int>[] stageFlips; //�� �������� �� �ø� ��(��� ������)
    public List<int>[] stageScores; //�� �������� �� ����(��� ������)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            //�� ��ȯ�� �Ǵ��� �ı����� �ʰ� �Ѵ�.
            DontDestroyOnLoad(gameObject);
        }

        Init();
        LoadData();
    }

    void Init()
    {
        totalStageCount = new int[3];
        stageFlips = new List<int>[3];
        stageScores = new List<int>[3];
        for (int i = 0; i < 3; i++)
        {
            stageFlips[i] = new List<int>();
            stageScores[i] = new List<int>();
        }
        totalStageCount[0] = gameManager.stage0Txt.Count();
        totalStageCount[1] = gameManager.stage1Txt.Count();
        totalStageCount[2] = gameManager.stage2Txt.Count();

        data = new Data();
    }

    void NewData()
    {
        for (int i = 0; i < 3; i++)
        {
            data.clearStage[i] = -1;
            data.stageTotalScore[i] = 0;
            data.masterCurrentClear[i] = 0;
            data.masterCurrentScore[i] = 0;
            data.masterMaxScore[i] = 0;
            data.isMasterDoing[i] = false;
        }
        data.userName = "Player";

        data.soundCk = false;
        data.soundVolume = 1.0f;
        data.bgmCk = false;
        data.bgmVolume = 1.0f;

        SaveData();
    }

    //������ ������ ������Ʈ(StageManager���� ȣ��)
    public void UpdateData(int kind, int curStage, int flip, int score, int totalScore)
    {
        //��� �����͸� ����� �����Ϳ� ����
        switch (kind)
        {
            case 0:
                data.stage0Flip = stageFlips[0];
                data.stage0Score = stageScores[0];
                break;
            case 1:
                data.stage1Flip = stageFlips[1];
                data.stage1Score = stageScores[1];
                break;
            case 2:
                data.stage2Flip = stageFlips[2];
                data.stage2Score = stageScores[2];
                break;
        }
        data.stageTotalScore[kind] = totalScore;

        //totalscore ���
        gameManager.uiManger.mainMenu.score.text = "Score: " + TotalScore();
        SaveData();
    }

    public int TotalScore() {
        int total = 0;
        for (int i = 0; i < 3; i++) 
            total += data.stageTotalScore[i] + data.masterMaxScore[i];
        return total;
    }

    void LoadData()
    {
        try
        {
            data = LoadJsonFile<Data>(Application.persistentDataPath, FileName);
        }
        catch
        {
            NewData();
            return;
        }
        //����� �����͸� ��� �����Ϳ� ����
        stageFlips[0] = data.stage0Flip;
        stageFlips[1] = data.stage1Flip;
        stageFlips[2] = data.stage2Flip;
        stageScores[0] = data.stage0Score;
        stageScores[1] = data.stage1Score;
        stageScores[2] = data.stage2Score;
    }

    //���ڿ��� �� JSON �����͸� �޾Ƽ� ���ϴ� Ÿ���� ��ü�� ��ȯ
    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    //�����ϱ�
    public void SaveData()
    {
        //Ŭ������ ���ڿ��� �� Json �����ͷ� ��ȯ
        string ToJsonData = JsonConvert.SerializeObject(data);

        //Json ������ �����ϰ� ����(������ �̹� ������ �����)
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", Application.persistentDataPath, FileName), FileMode.Create);
        byte[] dataByte = Encoding.UTF8.GetBytes(ToJsonData);
        fileStream.Write(dataByte, 0, dataByte.Length);
        fileStream.Close();
    }

}
