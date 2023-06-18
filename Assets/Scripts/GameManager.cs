using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//로비 스크롤 위치 맨 위로 초기화해두기(안 해도 지장은 없음)

// - 두번째 입체는 완전히 보류(현재는 삼각형을 사용한 것을 생각중)
// - 브금 이상한거 수정하기, 홈 화면이 아니면 인게임 브금이 끊기지 않도록 재생 함수 난발하지 않기
// - 클리어, 실패 UI에서 닫기 버튼 눌렀을 때, 로비와 재시작 사이의 오류 수정(로딩 UI 하나 더 만들어서 해결하기)


// - **UI 폰트 수정
// - **UI 디자인 다시 꾸미기
// - **메인 화면 스테이지 버튼 왼쪽엔 그림, 오른쪽엔 레벨과 점수 표시

// - 세팅 화면이 켜져있을 땐, 메인 화면 UI가 눌리지 않도록 하기


// - 효과음 다시 선택하기(선택 후 적절히 소리 키우기), (말 효과음을 음표 사운드 중 2개를 랜덤으로 선택)?
// - 퍼즐 모드 제작


// 4. 퍼즐 모드(스테이지 종류는 랜덤, 크기가 큰 보드가 판이고, 그보다는 조금 작은 보드가 주어지는데, 큰 보드에서 작은 보드의 모양을 완성하면 그 스테이지 클리어)
// - (최초에 최대 횟수를 정해주고, 스테이지 클리어때 마다 추가적으로 플립 횟수 지급, 플립 횟수가 0이 되면 실패, 총 플립 횟수와 클리어 횟수로 최고 점수 기록)
// - 보드 연산(전체 보드, 정답 보드가 있을 때, 정답 보드의 수 만큼 임시 보드를 생성하고, 임시 보드의 변수는 전체 보드의 좌표를 참조하도록 연산)

// 5. 테트리스 모드(흰색, 검은색이 섞인 블록이 떨어지고 이를 배치, 배치한 말을 플립하여 한 줄이 같은 색이면 사라지는 테트리스
// - (블록 1개 당 2번의 플립 가능, 플립을 두 번하면 블록이 떨어짐)


// 5. 구글 플레이 연동


public class GameManager : MonoBehaviour
{
    #region Variable Declaration

    public Camera mainCamera; //메인 카메라
    public UIManager uiManger;

    public InputField stageName; //생성할 스테이지 이름을 결정하는 UI
    public Toggle creatorMode; //체크되면 스테이지 생성 모드로 변경하는 UI
    public List<TextAsset> stage0Txt; //0스테이지 파일을 담는 변수
    public List<TextAsset> stage1Txt; //1스테이지 파일을 담는 변수
    public List<TextAsset> stage2Txt; //2스테이지 파일을 담는 변수
    public List<TextAsset> stage3Txt; //3스테이지 파일을 담는 변수

    public Transform cubeSide3; //3차원 큐브의 3개의 면
    public Transform cubeSide6; //3차원 큐브의 6개의 면
    public Transform rotateObject; //회전이 들어가는 3차원 오브젝트

    public int[] stage; //각 모드의 현재 스테이지
    public float createHorseTime; //말을 다 소환할 때까지 걸리는 시간
    public int curKind; //현재 진행하고 있는 스테이지의 종류(0: 사각형, 1: 육각형, 2: 큐브, 3: 벌집)

    public AudioSource horseAudio; //말 재생 소리
    public List<AudioClip> horseSounds; //말의 면에 따라 다른 소리 적용

    GameObject[] basic_horse; //말 오브젝트(종류 별로 나누기 위해 배열로 선언)

    //kind 0, 1
    int[,,] board; //0: 검은색, 1: 흰색, 5: 없음
    int[,,] initBoard; //파일을 불러왔을 초기 상태의 보드
    int[,,] masterTempBoard; //게임이 종료되지 않은 상태의 보드를 저장할 보드
    int s, u, v; //보드 칸(s: 면, u: 세로, v: 가로)
    int[,] adjSide; //3차원 보드의 한 면당 인접한 면을 표시

    Vector3 createHorsePos; //말을 배치할 기준 위치
    Basic_horse[,,] instantHorse; //소환한 2차원 말

    bool isFlip; //현재 애니메이션이 실행 중인지 확인하는 변수
    bool finalClear; //최종 클리어를 확인하는 변수
    public bool isCreatorMode; //스테이지 생성 모드인지 확인

    Queue<Basic_horse>[] queHorse; //지운 말을 재활용하기 위한 변수
    int flipCount; //말을 뒤집은 횟수
    int maxFlip; //말 뒤집기 최대 횟수(점수용)
    int realMaxFlip; //실제로 한 스테이지에서 최대로 뒤집을 수 있는 횟수
    int minFlip; //스테이지를 클리어하기 위한 최소 뒤집기 횟수
    int[] masterTempFlip; //마스터 모드의 남은 플립 수를 임시로 담을 변수

    Vector3[] stdPos; //위치를 정한 회전 오브젝트와 면 오브젝트의 초깃값을 저장
    Quaternion[] stdRot; //각도를 정한 회전 오브젝트와 면 오브젝트의 초깃값을 저장

    //설정으로 조정하지 않을 경우의 카메라 원래 값
    float cameraInitX;
    float cameraInitZoom;

    #endregion

    void Awake()
    {
        Init();
    }

    //초기화 함수
    void Init()
    {
        basic_horse = new GameObject[4];
        basic_horse[0] = Resources.Load("Prefabs/Prefab_Basic_horse") as GameObject;
        basic_horse[1] = Resources.Load("Prefabs/Prefab_Hexagon_horse") as GameObject;
        basic_horse[2] = Resources.Load("Prefabs/Prefab_Cube_horse") as GameObject;
        //basic_horse[3] = Resources.Load("Prefabs/Prefab_Cube_horse") as GameObject;
        finalClear = false;
        isCreatorMode = false;
        flipCount = 0;
        masterTempBoard = null;
        masterTempFlip = new int[4];
        queHorse = new Queue<Basic_horse>[4];
        for (int i = 0; i < 4; i++)
            queHorse[i] = new Queue<Basic_horse>();

        stage = new int[4];
        stdPos = new Vector3[2];
        stdRot = new Quaternion[2];
        for (int i = 0; i < 2; i++)
        {
            stdPos[i] = new Vector3();
            stdRot[i] = new Quaternion();
        }
    }

    //시작할 스테이지, 스테이지 종류를 입력받아 스테이지 실행
    public void GameStart(int curstage, int kind)
    {
        List<TextAsset> textFile = stage0Txt;

        switch (kind)
        {
            case 0:
                textFile = stage0Txt;
                break;
            case 1:
                textFile = stage1Txt;
                break;
            case 2:
                textFile = stage2Txt;
                break;
            case 3:
                textFile = stage3Txt;
                break;
        }

        if (textFile.Count <= curstage)
        {
            finalClear = true;
        }
        else finalClear = false;

        curKind = kind;
        stage[kind] = curstage;
        flipCount = 0;

        //게임 시작 직전에는 UI 버튼을 비활성화
        UI_Btn_OnOff(false);

        if (!finalClear)
        {
            ReadStageFile(curstage, textFile);
            CreateHorse();
            DefineHorsePos();
            StartCoroutine(PlaceHorse(createHorsePos));
            uiManger.ingameMenu.masterGroup.SetActive(false);
            uiManger.ingameMenu.resetBtn.gameObject.SetActive(true);
            // uiManger.ingameMenu.flipCount.gameObject.SetActive(true);
            // uiManger.ingameMenu.flipCount.text = "" + flipCount;
            uiManger.ingameMenu.maxFlipCount.text = "" + (realMaxFlip - flipCount);
        }
        else
        {
            uiManger.ingameMenu.stageTitle.text = "MASTER";
            uiManger.ingameMenu.resetBtn.gameObject.SetActive(false);
            uiManger.ingameMenu.masterGroup.SetActive(true);
            uiManger.ingameMenu.masterScore.text = "0";
            // uiManger.ingameMenu.flipCount.gameObject.SetActive(false);
            StartCoroutine(MasterGame());
        }
        uiManger.BGMPlay(1);
    }

    #region Horse_Create_and_Delete

    //변수에 저장된 텍스트 파일을 읽어 보드 입력 (맵 파일을 드래그 앤 드롭으로 받음)
    void ReadStageFile(int stage, List<TextAsset> textFile)
    {
        uiManger.ingameMenu.stageTitle.text = "STAGE " + (stage + 1);

        FiletoBoard(textFile[stage].text);
        
        //보드의 말 수 * 2 + 최소 플립
        maxFlip = (u * v * s) * 17 / 10 + (minFlip % u) * 2;
        realMaxFlip = (maxFlip / 5 + (minFlip / 10)) * 5 + (minFlip % 10);
    }

    //텍스트 파일의 정보를 보드에 입력
    void FiletoBoard(String txtFile)
    {
        StringReader strRea = new StringReader(txtFile);
        bool first = true;
        int uIndex = 0, sIndex = 0;

        // *스테이지 종류에 따라 적절히 수정하기*
        while (strRea != null)
        {
            string line = strRea.ReadLine();

            //텍스트 파일을 다 읽었는지 확인
            if (line == null)
            {
                break;
            }

            //첫 번째 줄인지 확인
            if (first)
            {
                //첫 번째 줄에는 행, 열의 수를 입력
                int start = 0;
                first = false;
                switch (curKind)
                {
                    //2차원
                    case 0:
                    case 1:
                        s = 1;
                        break;
                    //3차원
                    case 2:
                    case 3:
                        s = int.Parse(line.Split(',')[start++]);
                        break;
                }
                u = int.Parse(line.Split(',')[start++]);
                v = int.Parse(line.Split(',')[start++]);
                minFlip = int.Parse(line.Split(',')[start]);
                initBoard = new int[s, u, v];
                board = new int[s, u, v];
            }
            else
            {
                for (int i = 0; i < line.Split(',').Length; i++)
                {
                    board[sIndex, uIndex, i] = int.Parse(line.Split(',')[i]);
                    initBoard[sIndex, uIndex, i] = board[sIndex, uIndex, i];
                }

                if (uIndex < u - 1)
                    uIndex++;
                else if (sIndex < s - 1)
                {
                    uIndex = 0;
                    sIndex++;
                }

            }
        }
        Define_AdjacentSide();
    }

    //3차원에 한 면에 인접한 면이 무엇인지 정의
    void Define_AdjacentSide()
    {
        if (curKind == 2)
        {
            if (s == 3)
                adjSide = new int[3, 2];
            else
                adjSide = new int[6, 4];

            //면 0의 인접한 면
            adjSide[0, 0] = 2;
            adjSide[0, 1] = 1;
            //면 1의 인접한 면
            adjSide[1, 0] = 0;
            adjSide[1, 1] = 2;
            //면 2의 인접한 면
            adjSide[2, 0] = 1;
            adjSide[2, 1] = 0;

            if (s == 6)
            {
                //면 3의 인접한 면
                adjSide[3, 0] = 5;
                adjSide[3, 1] = 4;
                //면 4의 인접한 면
                adjSide[4, 0] = 3;
                adjSide[4, 1] = 5;
                //면 5의 인접한 면
                adjSide[5, 0] = 4;
                adjSide[5, 1] = 3;
                //인접한 2면만 정하면 나머지는 반복문으로 계산
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        adjSide[i, j + 2] = 5 - adjSide[i, j];
                    }
                }
            }
        }
    }

    //보드에 있는 정보를 바탕으로 말 소환
    void CreateHorse()
    {
        instantHorse = new Basic_horse[s, u, v];
        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < u; j++)
            {
                for (int k = 0; k < v; k++)
                {
                    if (board[i, j, k] == 5) instantHorse[i, j, k] = null;
                    else
                    {
                        if (queHorse[curKind].Count > 0)
                            instantHorse[i, j, k] = queHorse[curKind].Dequeue();
                        else
                            instantHorse[i, j, k] = Instantiate(basic_horse[curKind], transform.position, Quaternion.Euler(0, 0, 0)).gameObject.GetComponent<Basic_horse>();
                        instantHorse[i, j, k].Init(j, k, this, i);

                        if (s > 1)
                        {
                            //면의 수에 따라 해당하는 면의 자식으로 변환
                            if (s == 3)
                            {
                                instantHorse[i, j, k].transform.parent = cubeSide3.GetChild(i);
                                if (i == 0) instantHorse[i, j, k].transform.localRotation = Quaternion.Euler(0, 0, 0);
                                else if (i == 1) instantHorse[i, j, k].transform.localRotation = Quaternion.Euler(0, -90, 0);
                                else if (i == 2) instantHorse[i, j, k].transform.localRotation = Quaternion.Euler(0, 90, 0);
                            }
                            else if (s == 6)
                            {
                                instantHorse[i, j, k].transform.parent = cubeSide6.GetChild(i);
                                instantHorse[i, j, k].transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                        }
                        instantHorse[i, j, k].gameObject.SetActive(false);
                    }
                }
            }
        }

        //생성이 완료될 동안 로딩 화면
    }

    //스테이지가 끝나고, 기존 말들을 맵에서 제거
    IEnumerator DeleteHorse()
    {
        yield return new WaitForSeconds(0.7f);
        RealDeleteHorse();
    }

    //말을 큐에 넣는 방식으로 스테이지에서 제거
    public void RealDeleteHorse()
    {
        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < u; j++)
            {
                for (int k = 0; k < v; k++)
                {
                    if (instantHorse[i, j, k] != null)
                    {
                        instantHorse[i, j, k].gameObject.SetActive(false);
                        queHorse[curKind].Enqueue(instantHorse[i, j, k]);
                        instantHorse[i, j, k] = null;
                    }
                }
            }
        }

        if (s == 3)
            cubeSide3.gameObject.SetActive(false);
        else if (s == 6)
        {
            cubeSide6.gameObject.SetActive(false);
        }
    }

    #endregion

    #region MasterMode

    //모든 스테이지를 클리어했을 때, 플레이할 수 있는 모드
    IEnumerator MasterGame(bool next = false)
    {
        //이전 스테이지를 클리어하고 다음 스테이지를 진행하려는 상황일 때
        if (next)
        {
            CreateMasterBoard(PlayerData.Instance.data.masterCurrentClear[curKind]);
        }
        //이전에 진행한 스테이지를 불러올 때
        else if (PlayerData.Instance.data.isMasterDoing[curKind] && !next)
        {
            // if(masterTempBoard == null)
            //     MasterLoad();
            // else{
            //     BoardCopy(board, masterTempBoard);
            //     realMaxFlip = masterTempFlip[curKind];
            // }
            MasterLoad();
            uiManger.ingameMenu.masterScore.text = "" + PlayerData.Instance.data.masterCurrentScore[curKind];
        }
        //처음부터 시작
        else
        {
            realMaxFlip = 0;
            StartCoroutine(MasterFlipRise(20 + curKind * 10));
            CreateMasterBoard(PlayerData.Instance.data.masterCurrentClear[curKind]);
        }

        uiManger.ingameMenu.maxFlipCount.text = "" + realMaxFlip;
        uiManger.ingameMenu.masterMaxScore.text = "" + PlayerData.Instance.data.masterMaxScore[curKind];
        MasterMaxCrownMove(uiManger.ingameMenu.masterMaxScore.preferredWidth);
        yield return new WaitForSeconds(0.1f);
        CreateHorse();
        DefineHorsePos();
        StartCoroutine(PlaceHorse(createHorsePos));

        PlayerData.Instance.data.isMasterDoing[curKind] = true;
        MasterSave();
    }

    void MasterMaxCrownMove(float width){
        RectTransform imageRect = uiManger.ingameMenu.crownIcon.GetComponent<RectTransform>();
        imageRect.localPosition = new Vector2(600 - width, imageRect.localPosition.y);
    }

    //현재 클리어한 스테이지 수에 따른 난이도 설정
    int[] DefineMasterLevel(int curClear)
    {
        int ds = 1, du = 4, dMin = 1, dMax = 2; //보드의 s,u,v, 최소 플립수, 최대 플립 수 
        int[] defineArr = new int[4]; //출력할 배열
        switch (curKind)
        {
            case 0:
                // 칸 수 지정
                ds = 1;
                if (curClear >= 56) du = 6;
                else
                {
                    du = 4 + LevelCalculator(5, curClear);
                }

                //플립 수 범위 지정
                if (curClear >= 30)
                {
                    dMin = du * 2;
                    dMax = (ds * du * du) - du;
                }
                else if (curClear >= 20)
                {
                    dMin = du;
                    dMax = ds * du * du;
                }
                else
                {
                    dMin = 1;
                    dMax = du + ((s * u * v) / 3);
                }

                break;

            case 1:
                // 칸 수 지정
                ds = 1;
                if (curClear >= 80) du = 6;
                else
                {
                    du = 4 + LevelCalculator(6, curClear);
                }

                //플립 수 범위 지정
                if(curClear >= 50){
                    dMin = du*2;
                    dMax = (ds*du*du) - du;
                }
                else if(curClear >= 25){
                    dMin = du;
                    dMax = ds*du*du;
                }
                else{
                    dMin = 1;
                    dMax = du + ((s*u*v) / 3);
                }

                break;

            case 2:
                // 칸 수 지정
                if (curClear >= 80)
                {
                    int random = UnityEngine.Random.Range(0, 2);
                    if (random == 0)
                    {
                        ds = 3;
                        du = 6;
                    }
                    else
                    {
                        ds = 6;
                        du = 4;
                    }
                }
                else
                {
                    int level = LevelCalculator(6, curClear);
                    if (level > 3)
                    {
                        ds = 6;
                        du = level - 1;
                    }
                    else
                    {
                        ds = 3;
                        du = 3 + level;
                    }
                }

                //플립 수 범위 지정
                if(curClear >= 50){
                    dMin = du*2;
                    dMax = (ds*du*du) - du;
                }
                else if(curClear >= 25){
                    dMin = du;
                    dMax = ds*du*du;
                }
                else{
                    dMin = 1;
                    dMax = du + ((s*u*v) / 3);
                }

                break;
        }

        defineArr[0] = ds;
        defineArr[1] = du;
        defineArr[2] = dMin;
        defineArr[3] = dMax;

        return defineArr;
    }

    //스테이지 난이도를 설정
    int LevelCalculator(int levelCount, int curClear)
    {
        int level = curClear / 4;
        int levelModi = levelCount - 1;

        while (level >= levelCount)
        {
            level -= levelModi;
            levelModi--;
        }

        return level;
    }

    //마스터 모드의 랜덤 보드를 생성
    void CreateMasterBoard(int curClear)
    {
        int boardWhiteCount, boardBlankCount; //랜덤 보드의 흰색, 공백의 개수
        int x=1, y=1, z=1;
        int[,] flipCdn;
        int[] levelDesign = DefineMasterLevel(curClear);

        s = levelDesign[0];
        u = levelDesign[1];
        v = u;
        if(curKind == 2) Define_AdjacentSide();

        board = new int[s, u, v];
        boardBlankCount = UnityEngine.Random.Range(0, (s * u * v) / 3);

        if(levelDesign[3] < s*u*v - u){                                 
            levelDesign[3] += boardBlankCount;
        }
        boardWhiteCount = UnityEngine.Random.Range(levelDesign[2], levelDesign[3] - boardBlankCount);
        Debug.Log("boardWhiteCount: " + boardWhiteCount);
        flipCdn = new int[boardWhiteCount, 3];

        //최초엔 0으로 보드 초기화
        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < u; j++)
            {
                for (int k = 0; k < v; k++)
                {
                    board[i, j, k] = 0;
                }
            }
        }

        //보드에 블랭크 채우기
        for (int i = 0; i < boardBlankCount; i++)
        {
            do
            {
                x = UnityEngine.Random.Range(0, u);
                y = UnityEngine.Random.Range(0, v);
                z = UnityEngine.Random.Range(0, s);
            } while (board[z, x, y] == 5);
            board[z, x, y] = 5;
        }

        //보드에 플립할 말 정하기
        //말 리스트 채우기
        for (int i = 0; i < boardWhiteCount; i++)
        {
            do
            {
                x = UnityEngine.Random.Range(0, u);
                y = UnityEngine.Random.Range(0, v);
                z = UnityEngine.Random.Range(0, s);
            } while (board[z, x, y] == 5 || board[z, x, y] == 3);
            //좌표의 중복 제거를 위해 임시로 보드에 3을 입력
            board[z, x, y] = 3;
            flipCdn[i, 0] = z;
            flipCdn[i, 1] = x;
            flipCdn[i, 2] = y;
        }

        for (int i = 0; i < boardWhiteCount; i++)
            board[flipCdn[i, 0], flipCdn[i, 1], flipCdn[i, 2]] = 0;
        

        //리스트에 지정한 데로 플립
        for (int i = 0; i < boardWhiteCount; i++)
            MasterBoardFlip(flipCdn[i, 1], flipCdn[i, 2], flipCdn[i, 0]);
        
        if(CheckClear()){
            Debug.Log($"flipCdn: [{z}, {x}, {y}]");
            MasterBoardFlip(x, y, z);
        }
        
    }

    //랜덤 보드를 생성하기 위한 플립 함수
    void MasterBoardFlip(int hu, int hv, int hs)
    {
        int tu, tv, ts; //임시로 담을 변수

        board[hs, hu, hv] = BoardFlipHorse(board[hs, hu, hv]); //지정한 말 뒤집기

        //지정한 말 주변을 뒤집기
        switch (curKind)
        {
            case 0:
            case 1:
                for (int i = -1; i <= 1; i++)
                {
                    tu = hu + i;
                    for (int j = -1; j <= 1; j++)
                    {
                        tv = hv + j;
                        if (i == j)
                        {
                            if (i == 0 || curKind == 1) continue;
                        }
                        if (tu >= 0 && tu < u && tv >= 0 && tv < v && board[hs, tu, tv] != 5)
                        {
                            board[hs, tu, tv] = BoardFlipHorse(board[hs, tu, tv]);
                        }

                    }
                }
                break;

            case 2:
                //adj 3면
                if (s == 6 && hv == v - 1)
                {
                    ts = adjSide[hs, 3];
                    tv = v - 1;
                    for (int l = hu; l < hu + 3; l++)
                    {
                        tu = u - l;
                        if (tu >= 0 && tu < u && board[ts, tu, tv] != 5)
                        {
                            board[ts, tu, tv] = BoardFlipHorse(board[ts, tu, tv]);
                        }
                    }
                }

                //adj 0 면
                if (hu == 0)
                {
                    ts = adjSide[hs, 0];
                    tv = 0;
                    for (int l = -1; l < 2; l++)
                    {
                        tu = hv + l;
                        if (tu >= 0 && tu < u && board[ts, tu, tv] != 5)
                        {
                            board[ts, tu, tv] = BoardFlipHorse(board[ts, tu, tv]);
                        }

                    }
                }

                //s면
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        tu = hu + i;
                        tv = hv + j;
                        if (tu == hu && tv == hv) continue;
                        if (tu >= 0 && tu < u && tv >= 0 && tv < v && board[hs, tu, tv] != 5)
                        {
                            board[hs, tu, tv] = BoardFlipHorse(board[hs, tu, tv]);
                        }

                    }
                }

                //adj1 면
                if (hv == 0)
                {
                    ts = adjSide[hs, 1];
                    tu = 0;
                    for (int r = -1; r < 2; r++)
                    {
                        tv = hu + r;
                        if (tv >= 0 && tv < v && board[ts, tu, tv] != 5)
                        {
                            board[ts, tu, tv] = BoardFlipHorse(board[ts, tu, tv]);
                        }
                    }
                }

                //adj 2면
                if (s == 6 && hu == u - 1)
                {
                    ts = adjSide[hs, 2];
                    tu = u - 1;
                    for (int l = hv; l < hv + 3; l++)
                    {
                        tv = v - l;
                        if (tv >= 0 && tv < v && board[ts, tu, tv] != 5)
                        {
                            board[ts, tu, tv] = BoardFlipHorse(board[ts, tu, tv]);
                        }
                    }
                }
                break;
            case 3:
                break;
        }
    }

    //보드 생성에만 실행하는 플립 함수
    int BoardFlipHorse(int curState)
    {
        if (curState == 1)
            curState = 0;

        else
            curState = 1;

        return curState;
    }

    //마스터 모드의 한 스테이지 클리어 시 호출
    IEnumerator MasterClear()
    {
        StartCoroutine(DeleteHorse());
        Ingame_Lobby_Setting_OnOff(false);

        uiManger.masterClearSound.Play();

        //플레이어 데이터 업데이트
        int[] mastercurLevel = DefineMasterLevel(PlayerData.Instance.data.masterCurrentClear[curKind]);
        PlayerData.Instance.data.masterCurrentClear[curKind]++;
        int[] masterNextLevel = DefineMasterLevel(PlayerData.Instance.data.masterCurrentClear[curKind]);
        StartCoroutine(MasterFlipRise(masterNextLevel[3] + masterNextLevel[2]));
        StartCoroutine(MasterScoreRise(mastercurLevel[2] * (mastercurLevel[0] + mastercurLevel[1])));

        MasterSave();

        //다음 스테이지 시작
        yield return new WaitForSeconds(1);
        StartCoroutine(MasterGame(true));
    }

    //마스터 모드 스테이지에서 실패했을 때 호출
    void MasterFail()
    {
        StartCoroutine(StageFail());
        PlayerData.Instance.data.isMasterDoing[curKind] = false;
        PlayerData.Instance.data.masterCurrentClear[curKind] = 0;
        PlayerData.Instance.data.masterCurrentScore[curKind] = 0;
        MasterSave();
        masterTempBoard = null;
    }

    //마스터 모드에서 점수가 올라갈 때 호출
    IEnumerator MasterScoreRise(int masterScore)
    {
        bool isMaxAnim = false;
        PlayerData.Instance.data.masterCurrentScore[curKind] += masterScore;
        if (PlayerData.Instance.data.masterMaxScore[curKind] < PlayerData.Instance.data.masterCurrentScore[curKind]) {
            PlayerData.Instance.data.masterMaxScore[curKind] = PlayerData.Instance.data.masterCurrentScore[curKind];
            isMaxAnim = true;
        }

        //점수 올라가는 애니메이션
        for (int i = 1; i <= masterScore; i++)
        {
            uiManger.ingameMenu.masterScore.text = "" + (PlayerData.Instance.data.masterCurrentScore[curKind] - masterScore + i);
            if(isMaxAnim){
                uiManger.ingameMenu.masterMaxScore.text = "" + (PlayerData.Instance.data.masterCurrentScore[curKind] - masterScore + i);
                MasterMaxCrownMove(uiManger.ingameMenu.masterMaxScore.preferredWidth);
            }
            if(i > 1) uiManger.scoreSound.Play();
            yield return new WaitForSeconds(1.1f / masterScore);
        }
    }

    //마스터 모드의 플립이 오르는 애니메이션
    IEnumerator MasterFlipRise(int masterFlip)
    {
        realMaxFlip += masterFlip;
        for (int i = 1; i <= masterFlip; i++)
        {
            uiManger.ingameMenu.maxFlipCount.text = "" + (realMaxFlip - masterFlip + i);
            yield return new WaitForSeconds(1.1f / masterFlip);
        }
    }

    //현재 진행 중인 마스터 모드의 게임 상태를 백업
    void MasterSave() {
        //보드의 정보를 텍스트 파일로 저장
        string path = "Assets/Resources/MasterModeStage/";
        SaveStage(path, "MasterStage" + curKind, realMaxFlip);

        //마스터 모드의 정보를 Json 파일로 저장
        PlayerData.Instance.SaveData();

        //텍스트 파일이 제대로 저장되지 않았을 경우 변수에 저장
        // masterTempBoard = new int[s,u,v];
        // BoardCopy(masterTempBoard, board);
        // masterTempFlip[curKind] = realMaxFlip;
        //----------------------------------------------------------------------------------삭제 후보
    }

    //데스크탑 실행에만 사용(모바일 시에는 삭제)
    void BoardCopy(int[,,] first, int[,,] second)
    {
        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < u; j++)
            {
                for (int k = 0; k < v; k++)
                {
                    first[i, j, k] = second[i, j, k];
                }
            }
        }
    }
    //----------------------------------------------------------------------------------삭제 후보

    //백업된 마스터 모드의 게임 상태를 불러오기
    void MasterLoad()
    {
        TextAsset masterFile = Resources.Load("MasterModeStage/MasterStage" + curKind) as TextAsset;
        FiletoBoard(masterFile.text);
        realMaxFlip = minFlip;
    }

    #endregion

    #region PlaceHorse

    //보드의 칸에 따라 말의 좌표 및 카메라 위치, 각도를 결정
    void DefineHorsePos()
    {
        switch (curKind)
        {
            case 0:
                HorsePos0();
                break;
            case 1:
                HorsePos1();
                break;
            case 2:
                HorsePos2();
                break;
            case 3:
                HorsePos3();
                break;
        }

        //카메라로 볼 수 없는 면이 생기면 오브젝트를 회전할 수 있는 스크롤 생성
        if (curKind == 2 && s == 6)
        {
            Debug.Log("스크롤 생성");
            uiManger.ingameMenu.isScroll = true;
            uiManger.ingameMenu.scroll3Dobject.SetActive(true);
        }
        else
        {
            Debug.Log("스크롤 없음");
            uiManger.ingameMenu.isScroll = false;
            uiManger.ingameMenu.scroll3Dobject.SetActive(false);
        }
    }

    //kind 0
    void HorsePos0()
    {
        float x, y, z;
        switch (u)
        {
            case 3:
            case 4:
                x = -2;
                break;
            case 5:
                x = -4;
                break;
            case 6:
                x = -3;
                break;
            case 7:
                x = -3.5f;
                break;
            case 8:
                x = -3;
                break;
            default:
                x = 0;
                break;
        }
        cameraInitX = (u - 3) * (-0.5f) + 4.5f;
        mainCamera.transform.position = new Vector3(cameraInitX, 10, 0);
        mainCamera.transform.rotation = Quaternion.Euler((u - 3) * 5 + 60, -90, 0);
        z = (v - 3) * (-0.5f) - 1;

        switch (Mathf.Max(u, v))
        {
            case 3:
                y = 1;
                break;
            case 4:
                y = 0;
                break;
            case 5:
                y = -1;
                break;
            case 6:
                y = -2;
                break;
            case 7:
                y = -5;
                break;
            case 8:
                y = -6;
                break;
            default:
                y = 0;
                break;
        }
        cameraInitZoom = 60;
        mainCamera.fieldOfView = cameraInitZoom;
        createHorsePos = new Vector3(x, y, z);
    }

    //kind 1
    void HorsePos1()
    {
        float x, y, z;
        switch (u)
        {
            case 3:
                x = -1.5f;
                z = -1.5f;
                y = 1;
                break;
            case 4:
                x = -1.5f;
                z = -2.2f;
                y = 1;
                break;
            case 5:
                x = -2.5f;
                z = -3.2f;
                y = -2;
                break;
            case 6:
                x = -3;
                z = -4;
                y = -4;
                break;
            case 7:
                x = -3;
                z = -4.6f;
                y = -6;
                break;
            case 8:
                x = -3.5f;
                z = -5.35f;
                y = -9;
                break;
            case 9:
                x = -2.5f;
                z = -6;
                y = -11;
                break;
            default:
                x = 0;
                z = 0;
                y = 0;
                break;
        }
        cameraInitX = (u - 3) * (-0.5f) + 4.5f;
        mainCamera.transform.position = new Vector3(cameraInitX, 10, 0);
        mainCamera.transform.rotation = Quaternion.Euler((u - 3) * 5 + 60, -90, 0);
        //z = (v - 3) * (-0.5f) - 1;
        cameraInitZoom = 60;
        mainCamera.fieldOfView = cameraInitZoom;
        createHorsePos = new Vector3(x, y, z);
    }

    //kind 2
    void HorsePos2()
    {
        //회전이 필요한 오브젝트는 회전 오브젝트의 위치, 각도, 면 오브젝트의 위치, 각도까지 계산
        //면이 3개 일때
        if (s == 3)
        {
            cubeSide3.gameObject.SetActive(true);

            //면을 포함한 오브젝트의 위치와 카메라 위치 조정
            cubeSide3.rotation = Quaternion.Euler(-22.5f, 40, 22.5f);
            cameraInitX = 4.5f;
            mainCamera.transform.position = new Vector3(cameraInitX, 10, 0);

            //한 면의 칸 수: (2x2 ~ 6x6), u=v
            switch (u)
            {
                case 2:
                case 3:
                default:
                    cubeSide3.position = new Vector3(0, 0, 0);
                    mainCamera.transform.rotation = Quaternion.Euler(60, -90, 0);
                    cameraInitZoom = 60;
                    break;

                case 4:
                case 5:
                    cubeSide3.position = new Vector3(0, 0, 0);
                    mainCamera.transform.rotation = Quaternion.Euler(64, -90, 0);
                    cameraInitZoom = 70;
                    break;

                case 6:
                    cubeSide3.position = new Vector3(-1, -1.5f, 0);
                    mainCamera.transform.rotation = Quaternion.Euler(64, -90, 0);
                    cameraInitZoom = 75;
                    break;
            }
            mainCamera.fieldOfView = cameraInitZoom;
        }

        //면이 6개일 때
        else if (s == 6)
        {
            cubeSide6.gameObject.SetActive(true);

            //카메라 위치, 각도, 줌 조정
            cameraInitX = 4.5f;
            mainCamera.transform.position = new Vector3(cameraInitX, 10, 0);
            mainCamera.transform.rotation = Quaternion.Euler(60, -90, 0);
            cameraInitZoom = 60;
            mainCamera.fieldOfView = cameraInitZoom;

            stdRot[0] = Quaternion.Euler(0, 0, 0);
            stdRot[1] = Quaternion.Euler(0, 45, 0);

            //한 면의 칸 수: (2x2 ~ 4x4) u=v
            switch (u)
            {
                case 2:
                    stdPos[0] = new Vector3(1, 1.5f, -0.4f);
                    stdPos[1] = new Vector3(0.7f, 1, 0);
                    break;
                case 3:
                default:
                    stdPos[0] = new Vector3(0, 0, -0.4f);
                    stdPos[1] = new Vector3(1.5f, 1.5f, 0);
                    break;
                case 4:
                    stdPos[0] = new Vector3(-1, -2, -0.4f);
                    stdPos[1] = new Vector3(2.1f, 2.1f, 0);
                    break;
            }

            cubeSide6.GetChild(3).localPosition = new Vector3((u - 2) * (-1) - 1.5f, (u - 2) * (-1) - 1.5f, (u - 2) * (-1) - 1);
            cubeSide6.GetChild(4).localPosition = new Vector3((u - 2) * (-1) - 1, (u - 2) * (-1) - 1.5f, (u - 2) * (-1) - 1.5f);
            cubeSide6.GetChild(5).localPosition = new Vector3((u - 2) * (-1) - 1, (u - 2) * (-1) - 2, (u - 2) * (-1) - 1);
            //면들의 중심을 채워줄 큐브 오브젝트
            cubeSide6.GetChild(6).localPosition = new Vector3((u - 2) * (-0.5f) - 0.5f, (u - 2) * (-0.5f) - 1, (u - 2) * (-0.5f) - 0.5f);
            cubeSide6.GetChild(6).localScale = new Vector3((u - 2) + 1.5f, (u - 2) + 1.5f, (u - 2) + 1.5f);
            cubeSide6.GetChild(6).gameObject.SetActive(false);
        }
    }

    //kind 3
    void HorsePos3()
    {

    }

    //보드에 있는 정보를 바탕으로 적절한 위치에 말 배치
    IEnumerator PlaceHorse(Vector3 createPosition)
    {
        Vector3 pos, initPos = createPosition;
        float addx = 0, addz = 0, initz = 0;
        yield return new WaitForSeconds(createHorseTime / (s * u * v));
        switch (curKind)
        {
            case 0:
                initPos = createPosition;
                addx = 1.5f;
                addz = 1;
                initz = createPosition.z;
                break;
            case 1:
                initPos = createPosition;
                addx = 1;
                addz = 1;
                initz = createPosition.z;
                break;
            case 2:
                initPos = new Vector3(0, 0, 0);
                addx = 1;
                addz = 1;
                initz = 0;
                if (s == 6)
                {
                    Set3DRotate(cubeSide6);
                    cubeSide6.GetChild(6).gameObject.SetActive(false);
                }
                break;
        }

        for (int i = 0; i < s; i++)
        {
            pos = initPos;
            for (int j = 0; j < u; j++)
            {
                for (int k = 0; k < v; k++)
                {
                    if (instantHorse[i, j, k] != null)
                    {
                        instantHorse[i, j, k].transform.localPosition = pos;
                        instantHorse[i, j, k].gameObject.SetActive(true);
                        instantHorse[i, j, k].PlaySummon(board[i, j, k]);
                        yield return new WaitForSeconds(createHorseTime / (s * u * v)); //말의 수가 많은 만큼 시간 줄이기
                    }
                    pos.z += addz;
                }
                pos.z = initz;
                pos.x += addx;
                if (curKind == 1) pos.z += (j + 1) * 0.5f;
            }
        }

        isFlip = true;

        if (curKind == 2 && s == 6)
        {
            yield return new WaitForSeconds(0.1f);
            cubeSide6.GetChild(6).gameObject.SetActive(true);
        }

        //배치 애니메이션이 끝나고부터 플립 실행 가능
        int a = 1, b = 1, c = 1;
        while (instantHorse[s - c, u - a, v - b] == null)
        {
            if (v < b)
            {
                b = 1;
                a++;
            }
            else b++;
        }
        StartCoroutine(CheckAnim(u - a, v - b, s - c));

        //말 배치가 끝나면 UI 버튼 활성화
        UI_Btn_OnOff(true);
    }

    //스크롤, 3차원 물체 각도 초기화
    void Set3DRotate(Transform tempObj)
    {
        uiManger.ingameMenu.vScroll.value = 0;
        rotateObject.position = stdPos[0];
        rotateObject.rotation = stdRot[0];
        tempObj.localPosition = stdPos[1];
        tempObj.localRotation = stdRot[1];
    }

    #endregion

    //말 하나를 뒤집으면 주변에 있는 말들도 함께 뒤집히게 하는 함수
    public IEnumerator Flip_in_Board(int hu, int hv, int hs)
    {
        //현재 플립이 실행 중이 아니고, 인게임 UI가 켜져있고, 인게임 세팅 UI가 껴져 있을 때 실행
        //생성 모드가 아닌 상태에서 클리어하거나 클리어 실패를 하지 않을 때 실행        
        if (!isFlip && !uiManger.ingameMenu.ingameSettingMenu.activeSelf && uiManger.ingameMenu.gameObject.activeSelf
        && !(CheckClear() && !isCreatorMode) && !IsFail())
        {
            int tu, tv, ts; //임시로 담을 변수
            int lu, lv, ls; //가장 마지막에 실행된 애니메이션을 확인하는 변수

            isFlip = true;

            //마스터 모드
            if (finalClear)
            {
                realMaxFlip--;
                StartCoroutine(MasterScoreRise(1));
                uiManger.ingameMenu.maxFlipCount.text = "" + realMaxFlip;
            }
            else
            {
                flipCount++;
                // uiManger.ingameMenu.flipCount.text = "";
                // uiManger.ingameMenu.flipCount.text += flipCount;
                uiManger.ingameMenu.maxFlipCount.text = "" + (realMaxFlip - flipCount);
            }

            Debug.Log($"s: {hs}, u: {hu}, v: {hv}");

            board[hs, hu, hv] = instantHorse[hs, hu, hv].FlipHorse(board[hs, hu, hv]); //지정한 말을 뒤집기      
            lu = hu;
            lv = hv;
            ls = hs;
            yield return new WaitForSeconds(0.1f);

            //지정한 말 주변의 말들도 뒤집기(스테이지 종류에 따라 로직 변경)
            switch (curKind)
            {
                case 0:
                case 1:
                    for (int i = -1; i <= 1; i++)
                    {
                        tu = hu + i;
                        for (int j = -1; j <= 1; j++)
                        {
                            tv = hv + j;
                            if (i == j)
                            {
                                if (i == 0 || curKind == 1) continue;
                            }
                            if (tu >= 0 && tu < u && tv >= 0 && tv < v && instantHorse[hs, tu, tv] != null)
                            {
                                board[hs, tu, tv] = instantHorse[hs, tu, tv].FlipHorse(board[hs, tu, tv]);
                                lu = tu;
                                lv = tv;
                                yield return new WaitForSeconds(0.1f);
                            }

                        }
                    }
                    break;

                case 2:
                    //adj 3면
                    if (s == 6 && hv == v - 1)
                    {
                        ts = adjSide[hs, 3];
                        tv = v - 1;
                        for (int l = hu; l < hu + 3; l++)
                        {
                            tu = u - l;
                            if (tu >= 0 && tu < u && instantHorse[ts, tu, tv] != null)
                            {
                                board[ts, tu, tv] = instantHorse[ts, tu, tv].FlipHorse(board[ts, tu, tv]);
                                lu = tu;
                                lv = tv;
                                ls = ts;
                                yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }

                    //adj 0 면
                    if (hu == 0)
                    {
                        ts = adjSide[hs, 0];
                        tv = 0;
                        for (int l = -1; l < 2; l++)
                        {
                            tu = hv + l;
                            if (tu >= 0 && tu < u && instantHorse[ts, tu, tv] != null)
                            {
                                board[ts, tu, tv] = instantHorse[ts, tu, tv].FlipHorse(board[ts, tu, tv]);
                                lu = tu;
                                lv = tv;
                                ls = ts;
                                yield return new WaitForSeconds(0.1f);
                            }

                        }
                    }

                    //s면
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            tu = hu + i;
                            tv = hv + j;
                            if (tu == hu && tv == hv) continue;
                            if (tu >= 0 && tu < u && tv >= 0 && tv < v && instantHorse[hs, tu, tv] != null)
                            {
                                board[hs, tu, tv] = instantHorse[hs, tu, tv].FlipHorse(board[hs, tu, tv]);
                                lu = tu;
                                lv = tv;
                                ls = hs;
                                yield return new WaitForSeconds(0.1f);
                            }

                        }
                    }

                    //adj1 면
                    if (hv == 0)
                    {
                        ts = adjSide[hs, 1];
                        tu = 0;
                        for (int r = -1; r < 2; r++)
                        {
                            tv = hu + r;
                            if (tv >= 0 && tv < v && instantHorse[ts, tu, tv] != null)
                            {
                                board[ts, tu, tv] = instantHorse[ts, tu, tv].FlipHorse(board[ts, tu, tv]);
                                lu = tu;
                                lv = tv;
                                ls = ts;
                                yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }

                    //adj 2면
                    if (s == 6 && hu == u - 1)
                    {
                        ts = adjSide[hs, 2];
                        tu = u - 1;
                        for (int l = hv; l < hv + 3; l++)
                        {
                            tv = v - l;
                            if (tv >= 0 && tv < v && instantHorse[ts, tu, tv] != null)
                            {
                                board[ts, tu, tv] = instantHorse[ts, tu, tv].FlipHorse(board[ts, tu, tv]);
                                lu = tu;
                                lv = tv;
                                ls = ts;
                                yield return new WaitForSeconds(0.1f);
                            }
                        }
                    }
                    break;
                case 3:
                    break;
            }

            if (isCreatorMode) Debug.Log("생성 모드");

            //클리어 체크
            if (CheckClear() && !isCreatorMode)
            {
                isFlip = false;
                Debug.Log("Clear!");
                if (finalClear) StartCoroutine(MasterClear());
                else StartCoroutine(StageClear());
            }
            else if (IsFail())
            {
                if (finalClear) MasterFail();
                else StartCoroutine(StageFail());
            }

            if (finalClear) MasterSave();

            //가장 마지막에 실행한 애니메이션이 끝나야 다음 애니메이션 실행 가능
            StartCoroutine(CheckAnim(lu, lv, ls));
        }
    }

    //마지막으로 뒤집은 말의 뒤집기 애니메이션이 끝났는지 확인
    IEnumerator CheckAnim(int hu, int hv, int hs)
    {
        while (!instantHorse[hs, hu, hv].AnimPlayCheck())
        {
            yield return new WaitForSeconds(0.1f);
        }
        isFlip = false;
    }

    #region Clear

    //모든 보드의 칸이 0인지 확인하여 클리어 했는지 체크
    public bool CheckClear()
    {
        int i, j, k;

        for (i = 0; i < s; i++)
        {
            for (j = 0; j < u; j++)
            {
                for (k = 0; k < v; k++)
                {
                    if (board[i, j, k] == 5) continue;
                    if (board[i, j, k] != 0)
                        return false;
                }
            }
        }

        return true;
    }

    //현재 스테이지를 클리어 했을 때 실행
    IEnumerator StageClear()
    {
        StageManager stageManager = uiManger.lobbyMenu.stageBtns[curKind];

        //만약 로비 메뉴가 실행이 안되면 setactive 활성화 후 함수 실행, 비활성화 진행
        bool isRenew = stageManager.CurrentStageClear(stage[curKind], minFlip, flipCount, maxFlip);

        //BMG 일시 정지
        uiManger.BGMPause();

        //스테이지 표시
        uiManger.clearMenu.title.text = "STAGE " + (stage[curKind] + 1);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DeleteHorse());
        uiManger.ingameMenu.gameObject.SetActive(false);
        uiManger.clearMenu.clearMenu.SetActive(true);
        ClearUI_Btn_OnOff(false);

        //신기록 갱신 여부
        if (isRenew)
        {
            uiManger.clearMenu.comment.gameObject.SetActive(true);
            //점수 계산될 때까지 대기
            int random;
            for (int i = 0; i < 25; i++)
            {
                random = UnityEngine.Random.Range(10, 100);
                uiManger.clearMenu.scoreTxt.text = "Score: " + random;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
            uiManger.clearMenu.comment.gameObject.SetActive(false);

        int stageScore = stageManager.StageScore(stage[curKind]);

        //별 표시
        uiManger.ClearStar(stageManager.stageBtns[stage[curKind]].score, isRenew);

        //점수 표시
        uiManger.clearMenu.scoreTxt.text = "Score: " + stageScore;

        yield return new WaitForSeconds(1f);
        ClearUI_Btn_OnOff(true);
    }

    #endregion

    #region Fail

    //스테이지에 실패했는지 확인
    public bool IsFail()
    {
        if (flipCount >= realMaxFlip) return true;
        else if (realMaxFlip == 0) return true;
        else return false;
    }

    //스테이지에 실패했을 때 실행
    IEnumerator StageFail()
    {
        //BMG 일시 정지
        uiManger.BGMPause();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DeleteHorse());
        uiManger.ingameMenu.gameObject.SetActive(false);
        uiManger.failMenu.failMenu.SetActive(true);
        uiManger.failMenu.textAnim.Play("Fail");

        uiManger.failMenu.backBtn.interactable = false;
        uiManger.failMenu.exitBtn.interactable = false;

        yield return new WaitForSeconds(1f);

        uiManger.failMenu.backBtn.interactable = true;
        uiManger.failMenu.exitBtn.interactable = true;
    }

    #endregion

    #region UI_Actions
    //현재 보드 정보를 바탕으로 스테이지 텍스트 파일 생성
    public void GenerateStage()
    {
        Debug.Log("Generate: " + stageName.text);
        string path = "Assets/Stages/Stages" + curKind + "/";
        SaveStage(path, "stage" + stageName.text, flipCount);
    }

    //텍스트 파일 저장
    void SaveStage(string path, string fileName, int add)
    {
        StreamWriter sw = new StreamWriter(path + fileName + ".txt"); ;
        string temp;
        if (curKind <= 1)
            sw.WriteLine(u + "," + v + "," + add);
        else
            sw.WriteLine(s + "," + u + "," + v + "," + add);

        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < u; j++)
            {
                temp = "";
                for (int k = 0; k < v; k++)
                {
                    temp += board[i, j, k] + ",";
                }
                temp = temp.TrimEnd(',');
                if (j < u - 1 || i < s - 1)
                    temp += "\r\n";
                sw.Write(temp);
            }
        }

        sw.Flush();
        sw.Close();
    }

    public void SetCreatorMode()
    {
        isCreatorMode = creatorMode.isOn;
        Debug.Log("Creator Mode: " + isCreatorMode);
    }

    bool isRandom = false;
    public void CreateStage(int range)
    {
        if (!isRandom)
        {
            isRandom = true;
            StartCoroutine(CreateRandomStage(range));
        }
    }

    IEnumerator CreateRandomStage(int range)
    {
        int randomFlip;
        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < u; j++)
            {
                for (int k = 0; k < v; k++)
                {
                    if (board[i, j, k] != 5)
                    {
                        randomFlip = UnityEngine.Random.Range(0, 100);
                        if (randomFlip <= range)
                        {
                            StartCoroutine(Flip_in_Board(j, k, i));
                            yield return new WaitForSeconds(1);
                        }
                    }
                }
            }
        }
        isRandom = false;
    }

    //버튼을 눌렀을 때, 초기 상태와 같게 보드 리셋
    public void ResetBoard()
    {
        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < u; j++)
            {
                for (int k = 0; k < v; k++)
                {
                    if (instantHorse[i, j, k] != null)
                    {
                        instantHorse[i, j, k].gameObject.SetActive(false);
                        board[i, j, k] = initBoard[i, j, k];
                    }
                }
            }
        }


        flipCount = 0;
        //uiManger.ingameMenu.flipCount.text = "" + flipCount;
        uiManger.ingameMenu.maxFlipCount.text = "" + realMaxFlip;

        UI_Btn_OnOff(false);
        StartCoroutine(PlaceHorse(createHorsePos));
    }

    public void RotateHorizontal3Dobject(float value) 
    {
        rotateObject.transform.GetChild(0).rotation = Quaternion.Euler(540 * value, rotateObject.eulerAngles.y, rotateObject.eulerAngles.z);
    }

    public void RotateVertical3Dobject(float value)
    {
        rotateObject.rotation = Quaternion.Euler(rotateObject.eulerAngles.x, rotateObject.eulerAngles.y, 540 * value);
        //uiManger.objectRotateSound.Play();
    }

    public void CameraReset()
    {
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(0)); //인게임 세팅 메뉴 투명도
        mainCamera.transform.position = new Vector3(cameraInitX, mainCamera.transform.position.y, mainCamera.transform.position.z);
        mainCamera.fieldOfView = cameraInitZoom;
        uiManger.ingameMenu.cameraZoom.value = (cameraInitZoom - 55) / 70 + 0.5f;
    }

    public void CameraVerticalSetting(bool isUp)
    {
        float pos = 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(1)); //인게임 세팅 메뉴 투명도
        if (isUp)
        {
            if (mainCamera.transform.position.x < 10)
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + pos, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
        else
        {
            pos *= -1;
            if (mainCamera.transform.position.x > 0)
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + pos, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }

        if (mainCamera.transform.position.x < 10 && mainCamera.transform.position.x > 0)
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + pos, mainCamera.transform.position.y, mainCamera.transform.position.z);
        //카메라 x축 범위 0~10(칸당 0.5)
    }

    public void CameraZoomSetting(float value)
    {
        float setVal = value - 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(2)); //인게임 세팅 메뉴 투명도
        mainCamera.fieldOfView = setVal * 70 + 55;
        //카메라 줌 범위 20~90
    }

    //게임 UI 버튼 활성화 비활성화 설정
    void UI_Btn_OnOff(bool on)
    {
        uiManger.clearMenu.exitBtn.interactable = on;
        uiManger.clearMenu.backBtn.interactable = on;
        uiManger.clearMenu.nextBtn.interactable = on;
        uiManger.ingameMenu.resetBtn.interactable = on;
        Ingame_Lobby_Setting_OnOff(on);
    }

    void Ingame_Lobby_Setting_OnOff(bool on)
    {
        uiManger.ingameMenu.lobbyBtn.interactable = on;
        uiManger.ingameMenu.settingBtn.interactable = on;
    }

    void ClearUI_Btn_OnOff(bool on)
    {
        uiManger.clearMenu.exitBtn.interactable = on;
        uiManger.clearMenu.backBtn.interactable = on;
        uiManger.clearMenu.nextBtn.interactable = on;
    }

    #endregion

}
