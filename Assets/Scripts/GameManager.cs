using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//�κ� ��ũ�� ��ġ �� ���� �ʱ�ȭ�صα�(�� �ص� ������ ����)

// - ���� ȸ�� ���� ť�꿡���� �� �ᵵ ���� ����(���� �� �ٽ� Ȯ�� �غ���, ���� ���� ���� �� �θ� �ϳ� �� �ʿ��� ���ɼ� ����)

// - �����(���� ȭ�� ��, �� 4������ �������� �������� ������), (�ƿ� �ϳ��� �����ؼ� Ʋ��)

// - ȿ���� �ٽ� ����(���� �Ϸ� �Ŀ��� ������ �Ҹ� Ű���)

// 2. ������ ���(2����)
// - �������� ����


// 3. ���� ��ü
// - ��Ŭ����(���� UI)

// 4. Ư���� ��� �ĺ�
// - ��Ʈ����ó�� ���Ѵ�� ���Ӱ����� ���
// - ��Ƽ �÷��̷� �ٸ� ����� ������ Ǯ�ų� �ϴ� ���� ���
// - 3. ���Ǿ� ���(�⺻ �ʰ� ���� Ȥ�� ����� ���� Ǫ�µ�, �� ���� Ǫ�°� �����ϰ�, �������� �����ڰ� ������ ã�� ���, �� ��ǥȽ�� ����, ��ǥ �� ���Ͻ� ��ȿó��,
// �����ڴ� �ش� ���������� Ŭ�������� ���ϰ� �ϸ� �¸�, �������� �����ڸ� ã�ų� ���������� Ŭ�����ϸ� �¸�)


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

    //������ ��������, �������� ������ �Է¹޾� �������� ����
    public void GameStart(int curstage, int kind) {
        curKind = kind;
        ReadStageFile(curstage, kind);
        stage[kind] = curstage;
        flipCount = 0;

        //���� ���� �������� UI ��ư�� ��Ȱ��ȭ
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

    //������ ����� �ؽ�Ʈ ������ �о� ���� �Է� (�� ������ �巡�� �� ������� ����)
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
            Debug.Log("���� ����");
            finalClear = true;
            return;
        }

        uiManger.ingameMenu.stageTitle.text = "STAGE " + (stage + 1);

        StringReader strRea = new StringReader(textFile[stage].text);
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
                switch (kind) {
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
                else if(sIndex < s - 1)
                {
                    uIndex = 0;
                    sIndex++;
                }

            }
        }

        Define_AdjacentSide();

        //������ �� �� * 2 + �ּ� �ø�
        maxFlip = (u * v * s) * 17 / 10 + (minFlip % u) * 2;
        realMaxFlip = (maxFlip / 5 + (minFlip / 10)) * 5 + (minFlip % 10);
    }

    //3������ �� �鿡 ������ ���� �������� ����
    void Define_AdjacentSide() {
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
                for (int i = 0; i < 6; i++) {
                    for (int j = 0; j < 2; j++) {
                        adjSide[i, j + 2] = 5 - adjSide[i, j];
                    }
                }
            }
        }
    }

    //���忡 �ִ� ������ �������� �� ��ȯ
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

    //������ ĭ�� ���� ���� ��ǥ �� ī�޶� ��ġ, ������ ����
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

        //ī�޶�� �� �� ���� ���� ����� ������Ʈ�� ȸ���� �� �ִ� ��ũ�� ����
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
        else if (s == 6) {   
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
                        yield return new WaitForSeconds(createHorseTime / (s*u*v)); //���� ���� ���� ��ŭ �ð� ���̱�
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
    public IEnumerator Flip_in_Board(int hu, int hv, int hs) {
        //���� �ø��� ���� ���� �ƴϰ�, �ΰ��� UI�� �����ְ�, �ΰ��� ���� UI�� ���� ���� �� ����
        //���� ��尡 �ƴ� ���¿��� Ŭ�����ϰų� Ŭ���� ���и� ���� ���� �� ����        
        if (!isFlip && !uiManger.ingameMenu.ingameSettingMenu.activeSelf && uiManger.ingameMenu.gameObject.activeSelf
        && !(CheckClear() && !isCreatorMode) && !IsFail())
        {
            int tu, tv, ts; //�ӽ÷� ���� ����
            int lu, lv, ls; //���� �������� ����� �ִϸ��̼��� Ȯ���ϴ� ����

            isFlip = true;
            flipCount++;
            uiManger.ingameMenu.flipCount.text = "";
            uiManger.ingameMenu.flipCount.text += flipCount;
            uiManger.ingameMenu.maxFlipCount.text = "";
            uiManger.ingameMenu.maxFlipCount.text += realMaxFlip - flipCount;

            Debug.Log($"s: {hs}, u: {hu}, v: {hv}");

            board[hs, hu, hv] = instantHorse[hs, hu, hv].FlipHorse(board[hs, hu, hv]); //������ ���� ������      
            lu = hu;
            lv = hv;
            ls = hs;
            yield return new WaitForSeconds(0.1f);

            //������ �� �ֺ��� ���鵵 ������(�������� ������ ���� ���� ����)
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
                    //adj 3��
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

                    //adj 0 ��
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
                StartCoroutine(StageClear());
            }
            else if (IsFail()) StartCoroutine(StageFail());
            
            //���� �������� ������ �ִϸ��̼��� ������ ���� �ִϸ��̼� ���� ����
            StartCoroutine(CheckAnim(lu,lv,ls));
        }
    }

    //���������� ������ ���� ������ �ִϸ��̼��� �������� Ȯ��
    IEnumerator CheckAnim(int hu, int hv, int hs) {
        while (!instantHorse[hs, hu, hv].AnimPlayCheck())
        {
            yield return new WaitForSeconds(0.1f);
        }
        isFlip = false;
    }

    #region Clear

    //��� ������ ĭ�� 0���� Ȯ���Ͽ� Ŭ���� �ߴ��� üũ
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

    //���� ���������� Ŭ���� ���� �� ����
    IEnumerator StageClear()
    {
        StageManager stageManager = uiManger.lobbyMenu.stageBtns[curKind];

        //���� �κ� �޴��� ������ �ȵǸ� setactive Ȱ��ȭ �� �Լ� ����, ��Ȱ��ȭ ����
        bool isRenew = stageManager.CurrentStageClear(stage[curKind], minFlip, flipCount, maxFlip);

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
        else return false;
    }

    //���������� �������� �� ����
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
    //���� ���� ������ �������� �������� �ؽ�Ʈ ���� ����
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
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(0)); //�ΰ��� ���� �޴� ����
        mainCamera.transform.position = new Vector3(cameraInitX, mainCamera.transform.position.y, mainCamera.transform.position.z);
        mainCamera.fieldOfView = cameraInitZoom;
        uiManger.ingameMenu.cameraZoom.value = (cameraInitZoom - 55) / 70 + 0.5f;
    }

    public void CameraVerticalSetting(bool isUp) {
        float pos = 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(1)); //�ΰ��� ���� �޴� ����
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
        //ī�޶� x�� ���� 0~10(ĭ�� 0.5)
    }

    public void CameraZoomSetting(float value) {
        float setVal = value - 0.5f;
        StartCoroutine(uiManger.ingameMenu.IngameSettingAlbedo(2)); //�ΰ��� ���� �޴� ����
        mainCamera.fieldOfView = setVal * 70 + 55;
        //ī�޶� �� ���� 20~90
    }

    //���� UI ��ư Ȱ��ȭ ��Ȱ��ȭ ����
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
