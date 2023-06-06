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
        public int rank; //��ŷ(�ǽð����� ��ŷ ����)
        public int[] stageTotalScore; //�� ����
        public string lastDate; //�� ������ ���� ��¥(��ŷ ����)

        public int[] clearStage; //�� �������� �� ������� Ŭ���� �� ��������
        //����� ������
        public List<int> stage0Flip; //0�������� �� Ŭ���� �� �ø� Ƚ��
        public List<int> stage1Flip; //1�������� �� Ŭ���� �� �ø� Ƚ��
        public List<int> stage2Flip; //2�������� �� Ŭ���� �� �ø� Ƚ��
        public List<int> stage3Flip; //3�������� �� Ŭ���� �� �ø� Ƚ��
        public List<int> stage0Score; //0�������� �� Ŭ���� �� ����
        public List<int> stage1Score; //1�������� �� Ŭ���� �� ����
        public List<int> stage2Score; //2�������� �� Ŭ���� �� ����
        public List<int> stage3Score; //3�������� �� Ŭ���� �� ����

        public bool soundCk;
        public float soundVolume;
        public bool bgmCk;
        public float bgmVolume;

        public Data()
        {
            stageTotalScore = new int[4];
            clearStage = new int[4];
            stage0Flip = new List<int>();
            stage1Flip = new List<int>();
            stage2Flip = new List<int>();
            stage3Flip = new List<int>();
            stage0Score = new List<int>();
            stage1Score = new List<int>();
            stage2Score = new List<int>();
            stage3Score = new List<int>();
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
        //rank ���
    }

    void Init()
    {
        totalStageCount = new int[4];
        stageFlips = new List<int>[4];
        stageScores = new List<int>[4];
        for (int i = 0; i < 4; i++)
        {
            //totalStageCount[i] = File_Count(i);
            stageFlips[i] = new List<int>();
            stageScores[i] = new List<int>();
        }
        totalStageCount[0] = gameManager.stage0Txt.Count();
        totalStageCount[1] = gameManager.stage1Txt.Count();
        totalStageCount[2] = gameManager.stage2Txt.Count();
        totalStageCount[3] = gameManager.stage3Txt.Count();

        data = new Data();
    }

    void NewData()
    {
        for (int i = 0; i < 4; i++)
        {
            data.clearStage[i] = -1;
            data.stageTotalScore[i] = 0;
        }
        data.userName = "Google ID";
        data.rank = 1; //���ʿ� �� �ο��� +1
        data.lastDate = DateTime.Now.ToString("F");

        data.soundCk = false;
        data.soundVolume = 1.0f;
        data.bgmCk = false;
        data.bgmVolume = 1.0f;

        SaveData();
        Debug.Log("NewData");
    }

    //������ ������ ������Ʈ(StageManager���� ȣ��)
    public void UpdateData(int kind, int curStage, int flip, int score, int totalScore)
    {
        //�������� �����ͷ� �ű��
        //���� Ŭ���� ��
        if (data.clearStage[kind] < curStage)
        {
            data.clearStage[kind]++;
            stageFlips[kind].Add(flip);
            stageScores[kind].Add(score);
        }
        //�̹� Ŭ������ ���������� Ŭ���� ���� ��� + �������� ���� ������ ������� ���� ��
        else if (stageFlips[kind][curStage] <= flip) return;
        //�̹� Ŭ������ ���������� Ŭ���� ���� ��� + �������� ���� ������ ����� ��
        else
        {
            stageFlips[kind][curStage] = flip;
            stageScores[kind][curStage] = score;
        }

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
            case 3:
                data.stage3Flip = stageFlips[3];
                data.stage3Score = stageScores[3];
                break;
        }
        data.stageTotalScore[kind] = totalScore;

        //totalscore ���
        gameManager.uiManger.mainMenu.score.text = "Score: " + TotalScore();

        //rank ���
        //gameManager.uiManger.mainMenu.rank.text = "Rank: " + data.rank;
        data.lastDate = DateTime.Now.ToString("F");
        SaveData();
    }

    public int TotalScore() {
        int total = 0;
        for (int i = 0; i < 4; i++)
            total += data.stageTotalScore[i];
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
            Debug.Log("�ε� ����");
            NewData();
            return;
        }
        //����� �����͸� ��� �����Ϳ� ����
        stageFlips[0] = data.stage0Flip;
        stageFlips[1] = data.stage1Flip;
        stageFlips[2] = data.stage2Flip;
        stageFlips[3] = data.stage3Flip;
        stageScores[0] = data.stage0Score;
        stageScores[1] = data.stage1Score;
        stageScores[2] = data.stage2Score;
        stageScores[3] = data.stage3Score;
        data.bgmCk = false;
        data.bgmVolume = 1;
        data.soundCk = false;
        data.soundVolume = 1;
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
