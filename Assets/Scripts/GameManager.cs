using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//�κ� ��ũ�� ��ġ �� ���� �ʱ�ȭ�صα�(�� �ص� ������ ����)

// - �ι�° ��ü�� ������ ����(����� �ﰢ���� ����� ���� ������)
// - ��� �̻��Ѱ� �����ϱ�, Ȩ ȭ���� �ƴϸ� �ΰ��� ����� ������ �ʵ��� ��� �Լ� �������� �ʱ�
// - Ŭ����, ���� UI���� �ݱ� ��ư ������ ��, �κ�� ����� ������ ���� ����(�ε� UI �ϳ� �� ���� �ذ��ϱ�)


// - **UI ��Ʈ ����
// - **UI ������ �ٽ� �ٹ̱�
// - **���� ȭ�� �������� ��ư ���ʿ� �׸�, �����ʿ� ������ ���� ǥ��

// - ���� ȭ���� �������� ��, ���� ȭ�� UI�� ������ �ʵ��� �ϱ�


// - ȿ���� �ٽ� �����ϱ�(���� �� ������ �Ҹ� Ű���), (�� ȿ������ ��ǥ ���� �� 2���� �������� ����)?
// - ���� ��� ����


// 4. ���� ���(�������� ������ ����, ũ�Ⱑ ū ���尡 ���̰�, �׺��ٴ� ���� ���� ���尡 �־����µ�, ū ���忡�� ���� ������ ����� �ϼ��ϸ� �� �������� Ŭ����)
// - (���ʿ� �ִ� Ƚ���� �����ְ�, �������� Ŭ��� ���� �߰������� �ø� Ƚ�� ����, �ø� Ƚ���� 0�� �Ǹ� ����, �� �ø� Ƚ���� Ŭ���� Ƚ���� �ְ� ���� ���)
// - ���� ����(��ü ����, ���� ���尡 ���� ��, ���� ������ �� ��ŭ �ӽ� ���带 �����ϰ�, �ӽ� ������ ������ ��ü ������ ��ǥ�� �����ϵ��� ����)

// 5. ��Ʈ���� ���(���, �������� ���� ����� �������� �̸� ��ġ, ��ġ�� ���� �ø��Ͽ� �� ���� ���� ���̸� ������� ��Ʈ����
// - (��� 1�� �� 2���� �ø� ����, �ø��� �� ���ϸ� ����� ������)


// 5. ���� �÷��� ����


public class GameManager : MonoBehaviour
{
    #region Variable Declaration

    public Camera mainCamera; //���� ī�޶�
    public UIManager uiManger;

    public InputField stageName; //������ �������� �̸��� �����ϴ� UI
    public Toggle creatorMode; //üũ�Ǹ� �������� ���� ���� �����ϴ� UI
    public List<TextAsset> stage0Txt; //0�������� ������ ��� ����
    public List<TextAsset> stage1Txt; //1�������� ������ ��� ����
    public List<TextAsset> stage2Txt; //2�������� ������ ��� ����
    public List<TextAsset> stage3Txt; //3�������� ������ ��� ����

    public Transform cubeSide3; //3���� ť���� 3���� ��
    public Transform cubeSide6; //3���� ť���� 6���� ��
    public Transform rotateObject; //ȸ���� ���� 3���� ������Ʈ

    public int[] stage; //�� ����� ���� ��������
    public float createHorseTime; //���� �� ��ȯ�� ������ �ɸ��� �ð�
    public int curKind; //���� �����ϰ� �ִ� ���������� ����(0: �簢��, 1: ������, 2: ť��, 3: ����)

    public AudioSource horseAudio; //�� ��� �Ҹ�
    public List<AudioClip> horseSounds; //���� �鿡 ���� �ٸ� �Ҹ� ����

    GameObject[] basic_horse; //�� ������Ʈ(���� ���� ������ ���� �迭�� ����)

    //kind 0, 1
    int[,,] board; //0: ������, 1: ���, 5: ����
    int[,,] initBoard; //������ �ҷ����� �ʱ� ������ ����
    int[,,] masterTempBoard; //������ ������� ���� ������ ���带 ������ ����
    int s, u, v; //���� ĭ(s: ��, u: ����, v: ����)
    int[,] adjSide; //3���� ������ �� ��� ������ ���� ǥ��

    Vector3 createHorsePos; //���� ��ġ�� ���� ��ġ
    Basic_horse[,,] instantHorse; //��ȯ�� 2���� ��

    bool isFlip; //���� �ִϸ��̼��� ���� ������ Ȯ���ϴ� ����
    bool finalClear; //���� Ŭ��� Ȯ���ϴ� ����
    public bool isCreatorMode; //�������� ���� ������� Ȯ��

    Queue<Basic_horse>[] queHorse; //���� ���� ��Ȱ���ϱ� ���� ����
    int flipCount; //���� ������ Ƚ��
    int maxFlip; //�� ������ �ִ� Ƚ��(������)
    int realMaxFlip; //������ �� ������������ �ִ�� ������ �� �ִ� Ƚ��
    int minFlip; //���������� Ŭ�����ϱ� ���� �ּ� ������ Ƚ��
    int[] masterTempFlip; //������ ����� ���� �ø� ���� �ӽ÷� ���� ����

    Vector3[] stdPos; //��ġ�� ���� ȸ�� ������Ʈ�� �� ������Ʈ�� �ʱ갪�� ����
    Quaternion[] stdRot; //������ ���� ȸ�� ������Ʈ�� �� ������Ʈ�� �ʱ갪�� ����

    //�������� �������� ���� ����� ī�޶� ���� ��
    float cameraInitX;
    float cameraInitZoom;

    #endregion

    void Awake()
    {
        Init();
    }

    //�ʱ�ȭ �Լ�
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

    //������ ��������, �������� ������ �Է¹޾� �������� ����
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

        //���� ���� �������� UI ��ư�� ��Ȱ��ȭ
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

    //������ ����� �ؽ�Ʈ ������ �о� ���� �Է� (�� ������ �巡�� �� ������� ����)
    void ReadStageFile(int stage, List<TextAsset> textFile)
    {
        uiManger.ingameMenu.stageTitle.text = "STAGE " + (stage + 1);

        FiletoBoard(textFile[stage].text);
        
        //������ �� �� * 2 + �ּ� �ø�
        maxFlip = (u * v * s) * 17 / 10 + (minFlip % u) * 2;
        realMaxFlip = (maxFlip / 5 + (minFlip / 10)) * 5 + (minFlip % 10);
    }

    //�ؽ�Ʈ ������ ������ ���忡 �Է�
    void FiletoBoard(String txtFile)
    {
        StringReader strRea = new StringReader(txtFile);
        bool first = true;
        int uIndex = 0, sIndex = 0;

        // *�������� ������ ���� ������ �����ϱ�*
        while (strRea != null)
        {
            string line = strRea.ReadLine();

            //�ؽ�Ʈ ������ �� �о����� Ȯ��
            if (line == null)
            {
                break;
            }

            //ù ��° ������ Ȯ��
            if (first)
            {
                //ù ��° �ٿ��� ��, ���� ���� �Է�
                int start = 0;
                first = false;
                switch (curKind)
                {
                    //2����
                    case 0:
                    case 1:
                        s = 1;
                        break;
                    //3����
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

    //3������ �� �鿡 ������ ���� �������� ����
    void Define_AdjacentSide()
    {
        if (curKind == 2)
        {
            if (s == 3)
                adjSide = new int[3, 2];
            else
                adjSide = new int[6, 4];

            //�� 0�� ������ ��
            adjSide[0, 0] = 2;
            adjSide[0, 1] = 1;
            //�� 1�� ������ ��
            adjSide[1, 0] = 0;
            adjSide[1, 1] = 2;
            //�� 2�� ������ ��
            adjSide[2, 0] = 1;
            adjSide[2, 1] = 0;

            if (s == 6)
            {
                //�� 3�� ������ ��
                adjSide[3, 0] = 5;
                adjSide[3, 1] = 4;
                //�� 4�� ������ ��
                adjSide[4, 0] = 3;
                adjSide[4, 1] = 5;
                //�� 5�� ������ ��
                adjSide[5, 0] = 4;
                adjSide[5, 1] = 3;
                //������ 2�鸸 ���ϸ� �������� �ݺ������� ���
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

    //���忡 �ִ� ������ �������� �� ��ȯ
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
                            //���� ���� ���� �ش��ϴ� ���� �ڽ����� ��ȯ
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

        //������ �Ϸ�� ���� �ε� ȭ��
    }

    //���������� ������, ���� ������ �ʿ��� ����
    IEnumerator DeleteHorse()
    {
        yield return new WaitForSeconds(0.7f);
        RealDeleteHorse();
    }

    //���� ť�� �ִ� ������� ������������ ����
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

    //��� ���������� Ŭ�������� ��, �÷����� �� �ִ� ���
    IEnumerator MasterGame(bool next = false)
    {
        //���� ���������� Ŭ�����ϰ� ���� ���������� �����Ϸ��� ��Ȳ�� ��
        if (next)
        {
            CreateMasterBoard(PlayerData.Instance.data.masterCurrentClear[curKind]);
        }
        //������ ������ ���������� �ҷ��� ��
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
        //ó������ ����
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

    //���� Ŭ������ �������� ���� ���� ���̵� ����
    int[] DefineMasterLevel(int curClear)
    {
        int ds = 1, du = 4, dMin = 1, dMax = 2; //������ s,u,v, �ּ� �ø���, �ִ� �ø� �� 
        int[] defineArr = new int[4]; //����� �迭
        switch (curKind)
        {
            case 0:
                // ĭ �� ����
                ds = 1;
                if (curClear >= 56) du = 6;
                else
                {
                    du = 4 + LevelCalculator(5, curClear);
                }

                //�ø� �� ���� ����
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
                // ĭ �� ����
                ds = 1;
                if (curClear >= 80) du = 6;
                else
                {
                    du = 4 + LevelCalculator(6, curClear);
                }

                //�ø� �� ���� ����
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
                // ĭ �� ����
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

                //�ø� �� ���� ����
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

    //�������� ���̵��� ����
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

    //������ ����� ���� ���带 ����
    void CreateMasterBoard(int curClear)
    {
        int boardWhiteCount, boardBlankCount; //���� ������ ���, ������ ����
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

        //���ʿ� 0���� ���� �ʱ�ȭ
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

        //���忡 ��ũ ä���
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

        //���忡 �ø��� �� ���ϱ�
        //�� ����Ʈ ä���
        for (int i = 0; i < boardWhiteCount; i++)
        {
            do
            {
                x = UnityEngine.Random.Range(0, u);
                y = UnityEngine.Random.Range(0, v);
                z = UnityEngine.Random.Range(0, s);
            } while (board[z, x, y] == 5 || board[z, x, y] == 3);
            //��ǥ�� �ߺ� ���Ÿ� ���� �ӽ÷� ���忡 3�� �Է�
            board[z, x, y] = 3;
            flipCdn[i, 0] = z;
            flipCdn[i, 1] = x;
            flipCdn[i, 2] = y;
        }

        for (int i = 0; i < boardWhiteCount; i++)
            board[flipCdn[i, 0], flipCdn[i, 1], flipCdn[i, 2]] = 0;
        

        //����Ʈ�� ������ ���� �ø�
        for (int i = 0; i < boardWhiteCount; i++)
            MasterBoardFlip(flipCdn[i, 1], flipCdn[i, 2], flipCdn[i, 0]);
        
        if(CheckClear()){
            Debug.Log($"flipCdn: [{z}, {x}, {y}]");
            MasterBoardFlip(x, y, z);
        }
        
    }

    //���� ���带 �����ϱ� ���� �ø� �Լ�
    void MasterBoardFlip(int hu, int hv, int hs)
    {
        int tu, tv, ts; //�ӽ÷� ���� ����

        board[hs, hu, hv] = BoardFlipHorse(board[hs, hu, hv]); //������ �� ������

        //������ �� �ֺ��� ������
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
                //adj 3��
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

                //adj 0 ��
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

                //s��
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

                //adj1 ��
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

                //adj 2��
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

    //���� �������� �����ϴ� �ø� �Լ�
    int BoardFlipHorse(int curState)
    {
        if (curState == 1)
            curState = 0;

        else
            curState = 1;

        return curState;
    }

    //������ ����� �� �������� Ŭ���� �� ȣ��
    IEnumerator MasterClear()
    {
        StartCoroutine(DeleteHorse());
        Ingame_Lobby_Setting_OnOff(false);

        uiManger.masterClearSound.Play();

        //�÷��̾� ������ ������Ʈ
        int[] mastercurLevel = DefineMasterLevel(PlayerData.Instance.data.masterCurrentClear[curKind]);
        PlayerData.Instance.data.masterCurrentClear[curKind]++;
        int[] masterNextLevel = DefineMasterLevel(PlayerData.Instance.data.masterCurrentClear[curKind]);
        StartCoroutine(MasterFlipRise(masterNextLevel[3] + masterNextLevel[2]));
        StartCoroutine(MasterScoreRise(mastercurLevel[2] * (mastercurLevel[0] + mastercurLevel[1])));

        MasterSave();

        //���� �������� ����
        yield return new WaitForSeconds(1);
        StartCoroutine(MasterGame(true));
    }

    //������ ��� ������������ �������� �� ȣ��
    void MasterFail()
    {
        StartCoroutine(StageFail());
        PlayerData.Instance.data.isMasterDoing[curKind] = false;
        PlayerData.Instance.data.masterCurrentClear[curKind] = 0;
        PlayerData.Instance.data.masterCurrentScore[curKind] = 0;
        MasterSave();
        masterTempBoard = null;
    }

    //������ ��忡�� ������ �ö� �� ȣ��
    IEnumerator MasterScoreRise(int masterScore)
    {
        bool isMaxAnim = false;
        PlayerData.Instance.data.masterCurrentScore[curKind] += masterScore;
        if (PlayerData.Instance.data.masterMaxScore[curKind] < PlayerData.Instance.data.masterCurrentScore[curKind]) {
            PlayerData.Instance.data.masterMaxScore[curKind] = PlayerData.Instance.data.masterCurrentScore[curKind];
            isMaxAnim = true;
        }

        //���� �ö󰡴� �ִϸ��̼�
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

    //������ ����� �ø��� ������ �ִϸ��̼�
    IEnumerator MasterFlipRise(int masterFlip)
    {
        realMaxFlip += masterFlip;
        for (int i = 1; i <= masterFlip; i++)
        {
            uiManger.ingameMenu.maxFlipCount.text = "" + (realMaxFlip - masterFlip + i);
            yield return new WaitForSeconds(1.1f / masterFlip);
        }
    }

    //���� ���� ���� ������ ����� ���� ���¸� ���
    void MasterSave() {
        //������ ������ �ؽ�Ʈ ���Ϸ� ����
        string path = "Assets/Resources/MasterModeStage/";
        SaveStage(path, "MasterStage" + curKind, realMaxFlip);

        //������ ����� ������ Json ���Ϸ� ����
        PlayerData.Instance.SaveData();

        //�ؽ�Ʈ ������ ����� ������� �ʾ��� ��� ������ ����
        // masterTempBoard = new int[s,u,v];
        // BoardCopy(masterTempBoard, board);
        // masterTempFlip[curKind] = realMaxFlip;
        //----------------------------------------------------------------------------------���� �ĺ�
    }

    //����ũž ���࿡�� ���(����� �ÿ��� ����)
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
    //----------------------------------------------------------------------------------���� �ĺ�

    //����� ������ ����� ���� ���¸� �ҷ�����
    void MasterLoad()
    {
        TextAsset masterFile = Resources.Load("MasterModeStage/MasterStage" + curKind) as TextAsset;
        FiletoBoard(masterFile.text);
        realMaxFlip = minFlip;
    }

    #endregion

    #region PlaceHorse

    //������ ĭ�� ���� ���� ��ǥ �� ī�޶� ��ġ, ������ ����
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

        //ī�޶�� �� �� ���� ���� ����� ������Ʈ�� ȸ���� �� �ִ� ��ũ�� ����
        if (curKind == 2 && s == 6)
        {
            Debug.Log("��ũ�� ����");
            uiManger.ingameMenu.isScroll = true;
            uiManger.ingameMenu.scroll3Dobject.SetActive(true);
        }
        else
        {
            Debug.Log("��ũ�� ����");
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
        //ȸ���� �ʿ��� ������Ʈ�� ȸ�� ������Ʈ�� ��ġ, ����, �� ������Ʈ�� ��ġ, �������� ���
        //���� 3�� �϶�
        if (s == 3)
        {
            cubeSide3.gameObject.SetActive(true);

            //���� ������ ������Ʈ�� ��ġ�� ī�޶� ��ġ ����
            cubeSide3.rotation = Quaternion.Euler(-22.5f, 40, 22.5f);
            cameraInitX = 4.5f;
            mainCamera.transform.position = new Vector3(cameraInitX, 10, 0);

            //�� ���� ĭ ��: (2x2 ~ 6x6), u=v
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

        //���� 6���� ��
        else if (s == 6)
        {
            cubeSide6.gameObject.SetActive(true);

            //ī�޶� ��ġ, ����, �� ����
            cameraInitX = 4.5f;
            mainCamera.transform.position = new Vector3(cameraInitX, 10, 0);
            mainCamera.transform.rotation = Quaternion.Euler(60, -90, 0);
            cameraInitZoom = 60;
            mainCamera.fieldOfView = cameraInitZoom;

            stdRot[0] = Quaternion.Euler(0, 0, 0);
            stdRot[1] = Quaternion.Euler(0, 45, 0);

            //�� ���� ĭ ��: (2x2 ~ 4x4) u=v
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
            //����� �߽��� ä���� ť�� ������Ʈ
            cubeSide6.GetChild(6).localPosition = new Vector3((u - 2) * (-0.5f) - 0.5f, (u - 2) * (-0.5f) - 1, (u - 2) * (-0.5f) - 0.5f);
            cubeSide6.GetChild(6).localScale = new Vector3((u - 2) + 1.5f, (u - 2) + 1.5f, (u - 2) + 1.5f);
            cubeSide6.GetChild(6).gameObject.SetActive(false);
        }
    }

    //kind 3
    void HorsePos3()
    {

    }

    //���忡 �ִ� ������ �������� ������ ��ġ�� �� ��ġ
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
                        yield return new WaitForSeconds(createHorseTime / (s * u * v)); //���� ���� ���� ��ŭ �ð� ���̱�
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

        //��ġ �ִϸ��̼��� ��������� �ø� ���� ����
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

        //�� ��ġ�� ������ UI ��ư Ȱ��ȭ
        UI_Btn_OnOff(true);
    }

    //��ũ��, 3���� ��ü ���� �ʱ�ȭ
    void Set3DRotate(Transform tempObj)
    {
        uiManger.ingameMenu.vScroll.value = 0;
        rotateObject.position = stdPos[0];
        rotateObject.rotation = stdRot[0];
        tempObj.localPosition = stdPos[1];
        tempObj.localRotation = stdRot[1];
    }

    #endregion

    //�� �ϳ��� �������� �ֺ��� �ִ� ���鵵 �Բ� �������� �ϴ� �Լ�
    public IEnumerator Flip_in_Board(int hu, int hv, int hs)
    {
        //���� �ø��� ���� ���� �ƴϰ�, �ΰ��� UI�� �����ְ�, �ΰ��� ���� UI�� ���� ���� �� ����
        //���� ��尡 �ƴ� ���¿��� Ŭ�����ϰų� Ŭ���� ���и� ���� ���� �� ����        
        if (!isFlip && !uiManger.ingameMenu.ingameSettingMenu.activeSelf && uiManger.ingameMenu.gameObject.activeSelf
        && !(CheckClear() && !isCreatorMode) && !IsFail())
        {
            int tu, tv, ts; //�ӽ÷� ���� ����
            int lu, lv, ls; //���� �������� ����� �ִϸ��̼��� Ȯ���ϴ� ����

            isFlip = true;

            //������ ���
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

            board[hs, hu, hv] = instantHorse[hs, hu, hv].FlipHorse(board[hs, hu, hv]); //������ ���� ������      
            lu = hu;
            lv = hv;
            ls = hs;
            yield return new WaitForSeconds(0.1f);

            //������ �� �ֺ��� ���鵵 ������(�������� ������ ���� ���� ����)
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
                    //adj 3��
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

                    //adj 0 ��
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

                    //s��
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

                    //adj1 ��
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

                    //adj 2��
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

            if (isCreatorMode) Debug.Log("���� ���");

            //Ŭ���� üũ
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

            //���� �������� ������ �ִϸ��̼��� ������ ���� �ִϸ��̼� ���� ����
            StartCoroutine(CheckAnim(lu, lv, ls));
        }
    }

    //���������� ������ ���� ������ �ִϸ��̼��� �������� Ȯ��
    IEnumerator CheckAnim(int hu, int hv, int hs)
    {
        while (!instantHorse[hs, hu, hv].AnimPlayCheck())
        {
            yield return new WaitForSeconds(0.1f);
        }
        isFlip = false;
    }

    #region Clear

    //��� ������ ĭ�� 0���� Ȯ���Ͽ� Ŭ���� �ߴ��� üũ
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

    //���� ���������� Ŭ���� ���� �� ����
    IEnumerator StageClear()
    {
        StageManager stageManager = uiManger.lobbyMenu.stageBtns[curKind];

        //���� �κ� �޴��� ������ �ȵǸ� setactive Ȱ��ȭ �� �Լ� ����, ��Ȱ��ȭ ����
        bool isRenew = stageManager.CurrentStageClear(stage[curKind], minFlip, flipCount, maxFlip);

        //BMG �Ͻ� ����
        uiManger.BGMPause();

        //�������� ǥ��
        uiManger.clearMenu.title.text = "STAGE " + (stage[curKind] + 1);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DeleteHorse());
        uiManger.ingameMenu.gameObject.SetActive(false);
        uiManger.clearMenu.clearMenu.SetActive(true);
        ClearUI_Btn_OnOff(false);

        //�ű�� ���� ����
        if (isRenew)
        {
            uiManger.clearMenu.comment.gameObject.SetActive(true);
            //���� ���� ������ ���
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

        //�� ǥ��
        uiManger.ClearStar(stageManager.stageBtns[stage[curKind]].score, isRenew);

        //���� ǥ��
        uiManger.clearMenu.scoreTxt.text = "Score: " + stageScore;

        yield return new WaitForSeconds(1f);
        ClearUI_Btn_OnOff(true);
    }

    #endregion

    #region Fail

    //���������� �����ߴ��� Ȯ��
    public bool IsFail()
    {
        if (flipCount >= realMaxFlip) return true;
        else if (realMaxFlip == 0) return true;
        else return false;
    }

    //���������� �������� �� ����
    IEnumerator StageFail()
    {
        //BMG �Ͻ� ����
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
    //���� ���� ������ �������� �������� �ؽ�Ʈ ���� ����
    public void GenerateStage()
    {
        Debug.Log("Generate: " + stageName.text);
        string path = "Assets/Stages/Stages" + curKind + "/";
        SaveStage(path, "stage" + stageName.text, flipCount);
    }

    //�ؽ�Ʈ ���� ����
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

    //��ư�� ������ ��, �ʱ� ���¿� ���� ���� ����
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
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(0)); //�ΰ��� ���� �޴� ����
        mainCamera.transform.position = new Vector3(cameraInitX, mainCamera.transform.position.y, mainCamera.transform.position.z);
        mainCamera.fieldOfView = cameraInitZoom;
        uiManger.ingameMenu.cameraZoom.value = (cameraInitZoom - 55) / 70 + 0.5f;
    }

    public void CameraVerticalSetting(bool isUp)
    {
        float pos = 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(1)); //�ΰ��� ���� �޴� ����
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
        //ī�޶� x�� ���� 0~10(ĭ�� 0.5)
    }

    public void CameraZoomSetting(float value)
    {
        float setVal = value - 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(2)); //�ΰ��� ���� �޴� ����
        mainCamera.fieldOfView = setVal * 70 + 55;
        //ī�޶� �� ���� 20~90
    }

    //���� UI ��ư Ȱ��ȭ ��Ȱ��ȭ ����
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
