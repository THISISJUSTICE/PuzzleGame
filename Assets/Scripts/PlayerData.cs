using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

// - 랭킹 계산

public class PlayerData : MonoBehaviour
{
    private static PlayerData instance = null; //싱글톤 패턴

    //게임 매니저 인스턴스에 접근할 수 있는 프로퍼티. static이므로 다른 클래스에서 맘껏 호출할 수 있다.
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
        public string userName; //플레이어 닉네임
        public int[] stageTotalScore; //총 점수
        public int[] clearStage; //각 스테이지 별 현재까지 클리어 한 스테이지
        //저장용 데이터
        public List<int> stage0Flip; //0스테이지 별 클리어 시 플립 횟수
        public List<int> stage1Flip; //1스테이지 별 클리어 시 플립 횟수
        public List<int> stage2Flip; //2스테이지 별 클리어 시 플립 횟수
        public List<int> stage0Score; //0스테이지 별 클리어 시 점수
        public List<int> stage1Score; //1스테이지 별 클리어 시 점수
        public List<int> stage2Score; //2스테이지 별 클리어 시 점수

        public int[] masterCurrentClear; //마스터 모드의 현재 클리어한 횟수
        public int[] masterMaxScore; //마스터 모드에서 지금까지 최대로 얻은 점수
        public int[] masterCurrentScore; //마스터 모드에서 현재 얻은 점수
        public bool[] isMasterDoing; //진행 중인 마스터 모드가 있는지 판단
        //보드, 현재 남은 플립 횟수는 텍스트 파일에 저장

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

    string FileName = "GameData.json"; //게임 데이터 파일 이름 설정
    public Data data;
    public GameManager gameManager;

    public int[] totalStageCount; //각 스테이지 별 총 스테이지 수
    public List<int>[] stageFlips; //각 스테이지 별 플립 수(사용 데이터)
    public List<int>[] stageScores; //각 스테이지 별 점수(사용 데이터)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            //씬 전환이 되더라도 파괴되지 않게 한다.
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

    //저장한 데이터 업데이트(StageManager에서 호출)
    public void UpdateData(int kind, int curStage, int flip, int score, int totalScore)
    {
        //사용 데이터를 저장용 데이터에 저장
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

        //totalscore 계산
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
        //저장용 데이터를 사용 데이터에 저장
        stageFlips[0] = data.stage0Flip;
        stageFlips[1] = data.stage1Flip;
        stageFlips[2] = data.stage2Flip;
        stageScores[0] = data.stage0Score;
        stageScores[1] = data.stage1Score;
        stageScores[2] = data.stage2Score;
    }

    //문자열로 된 JSON 데이터를 받아서 원하는 타입의 객체로 반환
    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }

    //저장하기
    public void SaveData()
    {
        //클래스를 문자열로 된 Json 데이터로 변환
        string ToJsonData = JsonConvert.SerializeObject(data);

        //Json 파일을 생성하고 저장(파일이 이미 있으면 덮어쓰기)
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", Application.persistentDataPath, FileName), FileMode.Create);
        byte[] dataByte = Encoding.UTF8.GetBytes(ToJsonData);
        fileStream.Write(dataByte, 0, dataByte.Length);
        fileStream.Close();
    }

}
