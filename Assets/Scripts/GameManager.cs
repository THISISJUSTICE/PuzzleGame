using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//로비 스크롤 위치 맨 위로 초기화해두기(안 해도 지장은 없음)

// - 가로 회전 축은 큐브에서는 안 써도 문제 없음(벌집 때 다시 확인 해보기, 가로 축을 넣을 시 부모가 하나 더 필요할 가능성 있음)

// - 배경음(메인 화면 용, 각 4종류의 스테이지 종류별로 나누기), (아예 하나로 통일해서 틀기)

// - 효과음 다시 선택(선택 완료 후에는 적절히 소리 키우기)

// 2. 육각형 모양(2차원)
// - 스테이지 제작


// 3. 벌집 입체
// - 올클리어(관련 UI)

// 4. 특수한 모드 후보
// - 테트리스처럼 무한대로 지속가능한 모드
// - 멀티 플레이로 다른 사람과 퍼즐을 풀거나 하는 등의 모드
// - 3. 마피아 모드(기본 맵과 동일 혹은 비슷한 맵을 푸는데, 한 명은 푸는걸 방해하고, 나머지는 방해자가 누군지 찾는 모드, 총 투표횟수 제한, 투표 수 동일시 무효처리,
// 방해자는 해당 스테이지를 클리어하지 못하게 하면 승리, 나머지는 방해자를 찾거나 스테이지를 클리어하면 승리)


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
        queHorse = new Queue<Basic_horse>[4];
        for (int i = 0; i < 4; i++)
            queHorse[i] = new Queue<Basic_horse>();
        
        stage = new int[4];
        stdPos = new Vector3[2];
        stdRot = new Quaternion[2];
        for (int i = 0; i < 2; i++) {
            stdPos[i] = new Vector3();
            stdRot[i] = new Quaternion();
        }
    }

    //시작할 스테이지, 스테이지 종류를 입력받아 스테이지 실행
    public void GameStart(int curstage, int kind) {
        curKind = kind;
        ReadStageFile(curstage, kind);
        stage[kind] = curstage;
        flipCount = 0;

        //게임 시작 직전에는 UI 버튼을 비활성화
        UI_Btn_OnOff(false);

        if (!finalClear)
        {
            CreateHorse();
            DefineHorsePos();
            StartCoroutine(PlaceHorse(createHorsePos));
            uiManger.ingameMenu.flipCount.text = "";
            uiManger.ingameMenu.flipCount.text += flipCount;
            uiManger.ingameMenu.maxFlipCount.text = "";
            uiManger.ingameMenu.maxFlipCount.text += realMaxFlip - flipCount;
        }
        else
        {
            uiManger.ingameMenu.flipCount.gameObject.SetActive(false);
            uiManger.ingameMenu.maxFlipCount.gameObject.SetActive(false);
            uiManger.ingameMenu.stageTitle.gameObject.SetActive(false);
            uiManger.ingameMenu.allClearTitle.gameObject.SetActive(true);
        }
    }

    #region Horse_Create_and_Delete

    //변수에 저장된 텍스트 파일을 읽어 보드 입력 (맵 파일을 드래그 앤 드롭으로 받음)
    void ReadStageFile(int stage, int kind) {
        List<TextAsset> textFile = stage0Txt;

        switch (kind) {
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

        if (textFile.Count <= stage) {
            Debug.Log("파일 없음");
            finalClear = true;
            return;
        }

        uiManger.ingameMenu.stageTitle.text = "STAGE " + (stage + 1);

        StringReader strRea = new StringReader(textFile[stage].text);
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
                switch (kind) {
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
                else if(sIndex < s - 1)
                {
                    uIndex = 0;
                    sIndex++;
                }

            }
        }

        Define_AdjacentSide();

        //보드의 말 수 * 2 + 최소 플립
        maxFlip = (u * v * s) * 17 / 10 + (minFlip % u) * 2;
        realMaxFlip = (maxFlip / 5 + (minFlip / 10)) * 5 + (minFlip % 10);
    }

    //3차원에 한 면에 인접한 면이 무엇인지 정의
    void Define_AdjacentSide() {
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
                for (int i = 0; i < 6; i++) {
                    for (int j = 0; j < 2; j++) {
                        adjSide[i, j + 2] = 5 - adjSide[i, j];
                    }
                }
            }
        }
    }

    //보드에 있는 정보를 바탕으로 말 소환
    void CreateHorse() {
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
    public void RealDeleteHorse() {
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

    #region PlaceHorse

    //보드의 칸에 따라 말의 좌표 및 카메라 위치, 각도를 결정
    void DefineHorsePos() {
        switch (curKind) {
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
            uiManger.ingameMenu.isScroll = true;
        }
        else
        {
            uiManger.ingameMenu.isScroll = false;
            uiManger.ingameMenu.scroll3Dobject.SetActive(false);
        }
    }

    //kind 0
    void HorsePos0() {
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
        mainCamera.transform.rotation = Quaternion.Euler((u-3) * 5 + 60, -90, 0);
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
        mainCamera.transform.rotation = Quaternion.Euler((u-3) * 5 + 60, -90, 0);
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
        else if (s == 6) {   
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
    IEnumerator PlaceHorse(Vector3 createPosition) {
        Vector3 pos, initPos = createPosition;
        float addx = 0, addz = 0, initz = 0;
        yield return new WaitForSeconds(createHorseTime / (s*u*v));
        switch (curKind) {
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
                        yield return new WaitForSeconds(createHorseTime / (s*u*v)); //말의 수가 많은 만큼 시간 줄이기
                    }
                    pos.z += addz;
                }
                pos.z = initz;
                pos.x += addx;
                if(curKind == 1) pos.z += (j + 1)*0.5f;
            }
        }

        isFlip = true;

        if (curKind == 2 && s == 6) {
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
    public IEnumerator Flip_in_Board(int hu, int hv, int hs) {
        //현재 플립이 실행 중이 아니고, 인게임 UI가 켜져있고, 인게임 세팅 UI가 껴져 있을 때 실행
        //생성 모드가 아닌 상태에서 클리어하거나 클리어 실패를 하지 않을 때 실행        
        if (!isFlip && !uiManger.ingameMenu.ingameSettingMenu.activeSelf && uiManger.ingameMenu.gameObject.activeSelf
        && !(CheckClear() && !isCreatorMode) && !IsFail())
        {
            int tu, tv, ts; //임시로 담을 변수
            int lu, lv, ls; //가장 마지막에 실행된 애니메이션을 확인하는 변수

            isFlip = true;
            flipCount++;
            uiManger.ingameMenu.flipCount.text = "";
            uiManger.ingameMenu.flipCount.text += flipCount;
            uiManger.ingameMenu.maxFlipCount.text = "";
            uiManger.ingameMenu.maxFlipCount.text += realMaxFlip - flipCount;

            Debug.Log($"s: {hs}, u: {hu}, v: {hv}");

            board[hs, hu, hv] = instantHorse[hs, hu, hv].FlipHorse(board[hs, hu, hv]); //지정한 말을 뒤집기      
            lu = hu;
            lv = hv;
            ls = hs;
            yield return new WaitForSeconds(0.1f);

            //뒤집은 말 주변의 말들도 뒤집기(스테이지 종류에 따라 로직 변경)
            switch (curKind) {
                case 0:
                case 1:
                    for (int i = -1; i <= 1; i++)
                    {
                        tu = hu + i;
                        for (int j = -1; j <= 1; j++)
                        {   
                            tv = hv + j;
                            if(i == j){
                                if(i==0 || curKind == 1) continue;            
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
                // case 1:
                //     for(int i=-1; i<=1; i++){
                //         tu = hv + i;
                //         for(int j=-1; j<=1; j++){
                //             if(i == j) continue;
                //             tv = hv + j;
                //             if (tu >= 0 && tu < u && tv >= 0 && tv < v && instantHorse[hs, tu, tv] != null)
                //             {
                //                 board[hs, tu, tv] = instantHorse[hs, tu, tv].FlipHorse(board[hs, tu, tv]);
                //                 lu = tu;
                //                 lv = tv;
                //                 yield return new WaitForSeconds(0.1f);
                //             }
                //         }

                //     }
                //     break;
                case 2:
                    //adj 3면
                    if (s == 6 && hv == v - 1) {
                        ts = adjSide[hs, 3];
                        tv = v - 1;
                        for (int l = hu; l< hu+3; l++) {
                            tu = u - l;
                            if (tu >= 0 && tu < u && instantHorse[ts, tu, tv] != null) {
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
                            if (tu >= 0 && tu < u && instantHorse[ts, tu, tv] != null) {
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
                    if (hv == 0) {
                        ts = adjSide[hs, 1];
                        tu = 0;
                        for (int r = -1; r < 2; r++)
                        {
                            tv = hu + r;
                            if (tv >= 0 && tv< v && instantHorse[ts, tu, tv] != null) {    
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
                StartCoroutine(StageClear());
            }
            else if (IsFail()) StartCoroutine(StageFail());
            
            //가장 마지막에 실행한 애니메이션이 끝나야 다음 애니메이션 실행 가능
            StartCoroutine(CheckAnim(lu,lv,ls));
        }
    }

    //마지막으로 뒤집은 말의 뒤집기 애니메이션이 끝났는지 확인
    IEnumerator CheckAnim(int hu, int hv, int hs) {
        while (!instantHorse[hs, hu, hv].AnimPlayCheck())
        {
            yield return new WaitForSeconds(0.1f);
        }
        isFlip = false;
    }

    #region Clear

    //모든 보드의 칸이 0인지 확인하여 클리어 했는지 체크
    public bool CheckClear() {
        int i, j, k;

        for (i = 0; i < s; i++)
        {
            for (j = 0; j < u; j++)
            {
                for (k = 0; k < v; k++)
                {
                    if (instantHorse[i, j, k] == null) continue;
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
        else return false;
    }

    //스테이지에 실패했을 때 실행
    IEnumerator StageFail()
    {
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
        string path = "Assets/Stages/Stages" + curKind + "/" + "stage";
        string temp;
        StreamWriter sw;
        sw = new StreamWriter(path + stageName.text + ".txt");
        if (curKind <= 1)
            sw.WriteLine(u + "," + v + "," + flipCount);
        else
            sw.WriteLine(s + "," + u + "," + v + "," + flipCount);
        
        for (int i = 0; i < s; i++) {
            for (int j = 0; j < u; j++) {
                temp = "";
                for (int k = 0; k < v; k++) {
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
    public void CreateStage(int range){
        if(!isRandom){
            isRandom = true;
            StartCoroutine(CreateRandomStage(range));
        }
    }

    IEnumerator CreateRandomStage(int range){
        int randomFlip;
        for(int i=0; i<s; i++){
            for(int j=0; j<u; j++){
                for(int k=0; k<v; k++){
                    if(board[i,j,k] != 5){
                        randomFlip = UnityEngine.Random.Range(0,100);
                        if(randomFlip <= range){
                            StartCoroutine(Flip_in_Board(j,k,i));
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
        uiManger.ingameMenu.flipCount.text = "" + flipCount;
        uiManger.ingameMenu.maxFlipCount.text = "" + realMaxFlip;

        UI_Btn_OnOff(false);
        StartCoroutine(PlaceHorse(createHorsePos));
    }

    /*public void RotateHorizontal3Dobject(float value) 
    {
        rotateObject.rotation = Quaternion.Euler(360 * value, rotateObject.eulerAngles.y, rotateObject.eulerAngles.z);
    }*/

    public void RotateVertical3Dobject(float value)
    {
        rotateObject.rotation = Quaternion.Euler(rotateObject.eulerAngles.x, rotateObject.eulerAngles.y, 540 * value);
        uiManger.objectRotateSound.Play();
    }

    public void CameraReset() {
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(0)); //인게임 세팅 메뉴 투명도
        mainCamera.transform.position = new Vector3(cameraInitX, mainCamera.transform.position.y, mainCamera.transform.position.z);
        mainCamera.fieldOfView = cameraInitZoom;
        uiManger.ingameMenu.cameraZoom.value = (cameraInitZoom - 55) / 70 + 0.5f;
    }

    public void CameraVerticalSetting(bool isUp) {
        float pos = 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(1)); //인게임 세팅 메뉴 투명도
        if(isUp){
            if(mainCamera.transform.position.x < 10) 
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + pos, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
        else{
            pos *= -1;
            if(mainCamera.transform.position.x > 0) 
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + pos, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }

        if (mainCamera.transform.position.x < 10 && mainCamera.transform.position.x > 0)
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + pos, mainCamera.transform.position.y, mainCamera.transform.position.z);
        //카메라 x축 범위 0~10(칸당 0.5)
    }

    public void CameraZoomSetting(float value) {
        float setVal = value - 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(2)); //인게임 세팅 메뉴 투명도
        mainCamera.fieldOfView = setVal * 70 + 55;
        //카메라 줌 범위 20~90
    }

    //게임 UI 버튼 활성화 비활성화 설정
    void UI_Btn_OnOff(bool on) {
        uiManger.clearMenu.exitBtn.interactable = on;
        uiManger.clearMenu.backBtn.interactable = on;
        uiManger.clearMenu.nextBtn.interactable = on;
        uiManger.ingameMenu.resetBtn.interactable = on;
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
