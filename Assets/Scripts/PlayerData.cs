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
        public int rank; //랭킹(실시간으로 랭킹 변동)
        public int[] stageTotalScore; //총 점수
        public string lastDate; //총 점수를 얻은 날짜(랭킹 계산용)

        public int[] clearStage; //각 스테이지 별 현재까지 클리어 한 스테이지
        //저장용 데이터
        public List<int> stage0Flip; //0스테이지 별 클리어 시 플립 횟수
        public List<int> stage1Flip; //1스테이지 별 클리어 시 플립 횟수
        public List<int> stage2Flip; //2스테이지 별 클리어 시 플립 횟수
        public List<int> stage3Flip; //3스테이지 별 클리어 시 플립 횟수
        public List<int> stage0Score; //0스테이지 별 클리어 시 점수
        public List<int> stage1Score; //1스테이지 별 클리어 시 점수
        public List<int> stage2Score; //2스테이지 별 클리어 시 점수
        public List<int> stage3Score; //3스테이지 별 클리어 시 점수

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
        //rank 계산
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
        data.rank = 1; //최초엔 총 인원수 +1
        data.lastDate = DateTime.Now.ToString("F");

        data.soundCk = false;
        data.soundVolume = 1.0f;
        data.bgmCk = false;
        data.bgmVolume = 1.0f;

        SaveData();
        Debug.Log("NewData");
    }

    //저장한 데이터 업데이트(StageManager에서 호출)
    public void UpdateData(int kind, int curStage, int flip, int score, int totalScore)
    {
        //스테이지 데이터로 옮기기
        //최초 클리어 시
        if (data.clearStage[kind] < curStage)
        {
            data.clearStage[kind]++;
            stageFlips[kind].Add(flip);
            stageScores[kind].Add(score);
        }
        //이미 클리어한 스테이지를 클리어 했을 경우 + 기존보다 높은 점수를 기록하지 못할 시
        else if (stageFlips[kind][curStage] <= flip) return;
        //이미 클리어한 스테이지를 클리어 했을 경우 + 기존보다 높은 점수를 기록할 시
        else
        {
            stageFlips[kind][curStage] = flip;
            stageScores[kind][curStage] = score;
        }

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
            case 3:
                data.stage3Flip = stageFlips[3];
                data.stage3Score = stageScores[3];
                break;
        }
        data.stageTotalScore[kind] = totalScore;

        //totalscore 계산
        gameManager.uiManger.mainMenu.score.text = "Score: " + TotalScore();

        //rank 계산
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
            Debug.Log("로드 실패");
            NewData();
            return;
        }
        //저장용 데이터를 사용 데이터에 저장
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
