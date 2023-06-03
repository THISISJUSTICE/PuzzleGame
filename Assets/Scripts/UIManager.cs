using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// - ���� ȭ��(���� ��ư, ȿ����), �ݱ� ���

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    //public PlayerData playerData;

    public class MainMenu {
        public GameObject mainMenu; //���� ���� ȭ��
        public Button exitBtn, settingButton; //������ ��ư, ���� ��ư
        public Button quadBtn, hexBtn, cubeBtn, hiveBtn; //�簢 ��������, ���� ��������, 3���� ť�� ��������, ���� ��������
        public Text nickName, score, rank; //�г���, ����, ��ŷ

        public MainMenu(GameObject mainMenu) {
            this.mainMenu = mainMenu;
            Init();
        }

        void Init() {
            exitBtn = mainMenu.transform.GetChild(0).GetComponent<Button>();
            settingButton = mainMenu.transform.GetChild(1).GetComponent<Button>();
            nickName = mainMenu.transform.GetChild(2).GetChild(0).GetComponent<Text>();
            score = mainMenu.transform.GetChild(2).GetChild(1).GetComponent<Text>();
            rank = mainMenu.transform.GetChild(2).GetChild(2).GetComponent<Text>();
            quadBtn = mainMenu.transform.GetChild(3).GetChild(0).GetComponent<Button>();
            hexBtn = mainMenu.transform.GetChild(3).GetChild(1).GetComponent<Button>();
            cubeBtn = mainMenu.transform.GetChild(3).GetChild(2).GetComponent<Button>();
            hiveBtn = mainMenu.transform.GetChild(3).GetChild(3).GetComponent<Button>();
        }

    }
    public MainMenu mainMenu; //���� ���� ȭ��

    public class LobbyMenu {
        public GameObject lobbyMenu; //�������� �κ� ȭ��
        public StageManager[] stageBtns; //�������� ��ư �׷�
        public Button homeBtn, settingBtn, backBtn; //Ȩ ��ư, ���� ��ư, �ڷΰ��� ��ư

        public LobbyMenu(GameObject lobbyMenu) {
            this.lobbyMenu = lobbyMenu;
            Init();
        }

        void Init() {
            stageBtns = new StageManager[4];
            for (int i = 0; i < 4; i++)
            {
                stageBtns[i] = lobbyMenu.transform.GetChild(i).GetComponent<StageManager>();
            }
            homeBtn = lobbyMenu.transform.GetChild(4).GetChild(0).GetComponent<Button>();
            settingBtn = lobbyMenu.transform.GetChild(4).GetChild(2).GetComponent<Button>();
            backBtn = lobbyMenu.transform.GetChild(4).GetChild(3).GetComponent<Button>();
        }

    }
    public LobbyMenu lobbyMenu; //�������� �κ� ȭ��

    public IngameSetting ingameMenu; //���� ���� �� ȭ��

    public class ClearMenu {
        public GameObject clearMenu;
        public Text title, scoreTxt, comment;
        public Button exitBtn, backBtn, nextBtn;
        public Image[] scoreStar;
        public Animator animStar;

        public ClearMenu(GameObject clearMenu) {
            this.clearMenu = clearMenu;
            Init();
        }

        void Init() {
            exitBtn = clearMenu.transform.GetChild(0).GetComponent<Button>();
            title = clearMenu.transform.GetChild(1).GetComponent<Text>();
            comment = clearMenu.transform.GetChild(2).GetChild(0).GetComponent<Text>();
            scoreTxt = clearMenu.transform.GetChild(2).GetChild(1).GetComponent<Text>();              
            backBtn = clearMenu.transform.GetChild(2).GetChild(2).GetComponent<Button>();
            nextBtn = clearMenu.transform.GetChild(2).GetChild(3).GetComponent<Button>();
            animStar = clearMenu.transform.GetChild(2).GetChild(4).GetComponent<Animator>(); 

            scoreStar = new Image[3];
            for (int i = 0; i < scoreStar.Length; i++)
                scoreStar[i] = animStar.transform.GetChild(i).GetChild(1).GetComponent<Image>();
        }
    }
    public ClearMenu clearMenu; //Ŭ���� ȭ��

    public class FailMenu {
        public GameObject failMenu;
        public Button exitBtn, backBtn;
        public Animator textAnim;

        public FailMenu(GameObject failMenu) {
            this.failMenu = failMenu;
            Init();
        }

        void Init() {
            exitBtn = failMenu.transform.GetChild(1).GetComponent<Button>();
            backBtn = failMenu.transform.GetChild(2).GetComponent<Button>();
            textAnim = failMenu.transform.GetChild(3).GetComponent<Animator>();
        }
    }
    public FailMenu failMenu;

    public class SettingMenu
    {
        public GameObject settingMenu;
        public Button exitBtn, shareBtn;
        public Toggle soundCk;

        public SettingMenu(GameObject settingMenu)
        {
            this.settingMenu = settingMenu;
            Init();
        }

        void Init()
        {
            exitBtn = settingMenu.transform.GetChild(0).GetComponent<Button>();
            soundCk = settingMenu.transform.GetChild(1).GetComponent<Toggle>();
            shareBtn = settingMenu.transform.GetChild(2).GetComponent<Button>();          
        }
    }
    public SettingMenu settingMenu; //���� ȭ��

    public class ExitMenu  {
        public GameObject exitMenu;
        public Button yesBtn, noBtn;

        public ExitMenu(GameObject exitMenu) {
            this.exitMenu = exitMenu;
            Init();
        }

        void Init() {
            yesBtn = exitMenu.transform.GetChild(0).GetComponent<Button>();
            noBtn = exitMenu.transform.GetChild(1).GetComponent<Button>();
        }
    }
    public ExitMenu exitMenu; //������ â (yes or no)

    GameObject loadingUI;

    private void Awake()
    {
        Init();
        UISetting();
    }

    void Init()
    {
        mainMenu = new MainMenu(transform.Find("MainMenu").gameObject);
        lobbyMenu = new LobbyMenu(transform.Find("LobbyMenu").gameObject);
        ingameMenu = transform.Find("IngameMenu").GetComponent<IngameSetting>();
        ingameMenu.Init();     
        clearMenu = new ClearMenu(transform.Find("ClearMenu").gameObject);
        failMenu = new FailMenu(transform.Find("FailMenu").gameObject);
        settingMenu = new SettingMenu(transform.Find("SettingMenu").gameObject);
        exitMenu = new ExitMenu(transform.Find("ExitCheck").gameObject);
        loadingUI = transform.Find("LoadingUI").gameObject;
        loadingUI.gameObject.SetActive(true);
    }

    void UISetting() {
        SetMainMenu();
        SetLobbyMenu();
        SetIngameMenu();
        SetClearMenu();
        SetFailMenu();
        SetSettingMenu();
        SetExitMenu();
    }

    private void Start()
    {
        LateTask();
    }

    //Task Ȥ�� ��������Ʈ�� �ٲٱ�
    void LateTask() {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < lobbyMenu.stageBtns[i].stageBtns.Length; j++)
            {
                lobbyMenu.stageBtns[i].stageBtns[j].opened.onClick.AddListener(DoBack);
            }
            lobbyMenu.stageBtns[i].gameObject.SetActive(false);
        }
        lobbyMenu.lobbyMenu.SetActive(false);
        ingameMenu.ingameSettingMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(false);
        settingMenu.settingMenu.SetActive(false);
        clearMenu.clearMenu.SetActive(false);
        failMenu.failMenu.SetActive(false);
        exitMenu.exitMenu.SetActive(false);
        loadingUI.gameObject.SetActive(false);
    }

    #region MainMenu
    //���� �޴� UI ����
    void SetMainMenu()
    {
        mainMenu.exitBtn.onClick.AddListener(DoExit);
        mainMenu.settingButton.onClick.AddListener(DoSetting);
        MainMenuText();
        mainMenu.quadBtn.onClick.AddListener(() => DoStage(0)); //Quadangle
        mainMenu.hexBtn.onClick.AddListener(() => DoStage(1)); //Hexagon
        mainMenu.cubeBtn.onClick.AddListener(() => DoStage(2)); //Cube
        mainMenu.hiveBtn.onClick.AddListener(() => DoStage(3)); //Hive
    }

    //���� �޴� UI �ؽ�Ʈ ����
    void MainMenuText() {
        //�÷��̾� �����Ϳ��� �ҷ�����
        mainMenu.nickName.text = PlayerData.Instance.data.userName;
        mainMenu.score.text = "Score: " + PlayerData.Instance.TotalScore();
        mainMenu.rank.text = "Rank : " + 1;
    }

    //��ư
    void DoExit()
    {
        exitMenu.exitMenu.SetActive(true);
    }

    void DoSetting() {
        settingMenu.settingMenu.SetActive(true);
    }

    //�������� ��ư 4 ����(���� �� ���� ����)
    void DoStage(int kind) {
        mainMenu.mainMenu.SetActive(false);
        gameManager.GameStart(PlayerData.Instance.data.clearStage[kind] + 1, kind); //�÷��̾� �����Ϳ��� ���� ������ �Ű� ���� �Է�
        ingameMenu.gameObject.SetActive(true);

        ingameMenu.allClearTitle.gameObject.SetActive(false);
        ingameMenu.stageTitle.gameObject.SetActive(true);
        ingameMenu.flipCount.gameObject.SetActive(true);
        ingameMenu.maxFlipCount.gameObject.SetActive(true);
    }

    #endregion

    #region LobbyMenu
    //�κ� �޴� UI ����
    void SetLobbyMenu() {
        lobbyMenu.settingBtn.onClick.AddListener(DoSetting);
        lobbyMenu.homeBtn.onClick.AddListener(DoHome);
        lobbyMenu.backBtn.onClick.AddListener(DoBack);
    }

    //Ȩ ��ư ������ ��(���� ���� ���� ���������� �ʱ�ȭ�ǰ�, Ȩ ȭ������ �̵�)
    void DoHome() {
        lobbyMenu.stageBtns[gameManager.curKind].gameObject.SetActive(false);
        lobbyMenu.lobbyMenu.SetActive(false);
        mainMenu.mainMenu.SetActive(true);
        gameManager.RealDeleteHorse();
    }

    //�ڷ� ���� ��ư(�κ񿡼� ���� ȭ������ �ٽ� �Ѿ)
    void DoBack() {
        int kind = gameManager.curKind;
        lobbyMenu.stageBtns[gameManager.curKind].gameObject.SetActive(false);
        lobbyMenu.lobbyMenu.SetActive(false);
        lobbyMenu.stageBtns[kind].gameObject.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
    }

    #endregion

    #region IngameMenu
    //�ΰ��� ȭ�� ����
    void SetIngameMenu() {
        //�޴� ��ư, �ø� ī��Ʈ, Ÿ��Ʋ, ���� ��ư, ���� ��ư
        ingameMenu.lobbyBtn.onClick.AddListener(DoLobby);
        ingameMenu.settingBtn.onClick.AddListener(() => ingameMenu.ingameSettingMenu.SetActive(true));
        ingameMenu.resetBtn.onClick.AddListener(() => gameManager.ResetBoard());

        //������Ʈ ȸ��
        ingameMenu.vScroll.onValueChanged.AddListener(gameManager.RotateVertical3Dobject);

        //�ΰ��� ���� �޴�
        ingameMenu.cameraReset.onClick.AddListener(gameManager.CameraReset);
        ingameMenu.cameraUpBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(true));
        ingameMenu.cameraDownBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(false));
        ingameMenu.cameraZoom.onValueChanged.AddListener(gameManager.CameraZoomSetting);
        ingameMenu.exitBtn.onClick.AddListener(() => ingameMenu.ingameSettingMenu.SetActive(false));
        //���� ��ư
        //ingameMenu.shareBtn.onClick.AddListener();
        //�Ҹ� �¿���
        //ingameMenu.soundCk.isOn;
    }

    void DoLobby() {
        ingameMenu.gameObject.SetActive(false);
        lobbyMenu.lobbyMenu.SetActive(true);
        lobbyMenu.stageBtns[gameManager.curKind].gameObject.SetActive(true);
    }


    #endregion

    #region ClearMenu

    void SetClearMenu() {
        clearMenu.backBtn.onClick.AddListener(DoClearBack);
        clearMenu.nextBtn.onClick.AddListener(DoClearNext);
        clearMenu.exitBtn.onClick.AddListener(DoClearExit);
    }

    //�������� Ŭ���� �� ��� ������ ���������� �ٷ� �� ����
    void DoClearBack() {
        DoStageBack();
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
    }

    //��� ������ ���������� �ٽ� ����
    void DoStageBack() {
        int kind = gameManager.curKind; //���� �������� ������ ��� ����
        gameManager.GameStart(gameManager.stage[kind], kind);
        ClearInit();
    }

    //�������� Ŭ���� �� ���� ���������� ����
    void DoClearNext() {
        int kind = gameManager.curKind; //���� �������� ������ ��� ����
        gameManager.stage[kind]++;
        gameManager.GameStart(gameManager.stage[kind], kind);
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        ClearInit();
    }

    //�������� Ŭ���� �� �κ� ȭ������ �̵�
    void DoClearExit() {
        DoStageBack();
        clearMenu.clearMenu.SetActive(false);
        DoLobby();
        ClearInit();
    }

    //Ŭ���� UI �ʱ�ȭ
    void ClearInit() {
        clearMenu.scoreTxt.text = "Score: ";
        for (int i=0; i<3; i++) {
            clearMenu.scoreStar[i].fillAmount = 0;
        }
    }

    //������ ���� ���� ǥ���ϰ� �ִϸ��̼� ����
    public void ClearStar(int score, bool isRenew)
    {
        switch (score)
        {
            case 100:
                //�� 3��
                for (int i = 0; i < 3; i++) 
                    clearMenu.scoreStar[i].fillAmount = 1;               
                break;
            case int n when (n < 100 && n >= 75):
                //�� 2.5��
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 1;
                clearMenu.scoreStar[2].fillAmount = 0.5f;
                break;
            case int n when (n < 75 && n >= 50):
                //�� 2��
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 1;
                clearMenu.scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 50 && n >= 25):
                //�� 1.5��
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 0.5f;
                clearMenu.scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 25 && n >= 15):
                //�� 1��
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 0;
                clearMenu.scoreStar[2].fillAmount = 0;
                break;
            case 5:
                //�� 0.5��
                clearMenu.scoreStar[0].fillAmount = 0.5f;
                clearMenu.scoreStar[1].fillAmount = 0;
                clearMenu.scoreStar[2].fillAmount = 0;
                break;
        }

        for (int i=0; i<3; i++) {
            clearMenu.scoreStar[i].gameObject.SetActive(false);
        }

        if (isRenew)
        {
            clearMenu.animStar.Play("Appear");
        }
        for (int i = 0; i < 3; i++)
            clearMenu.scoreStar[i].gameObject.SetActive(true);
    }

    #endregion

    #region FailMenu

    void SetFailMenu() {
        failMenu.exitBtn.onClick.AddListener(DoFailExit);
        failMenu.backBtn.onClick.AddListener(DoFailBack);
    }

    void DoFailExit() {
        DoStageBack();
        failMenu.failMenu.SetActive(false);
        DoLobby();
    }

    void DoFailBack() {
        DoStageBack();
        failMenu.failMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
    }

    #endregion

    #region SettingMenu
    void SetSettingMenu()
    {
        settingMenu.exitBtn.onClick.AddListener(() => settingMenu.settingMenu.SetActive(false));
        //���� ��ư
        //settingMenu.shareBtn.onClick.AddListener();
        //�Ҹ� �¿���
        //settingMenu.soundCk.isOn
    }

    #endregion

    #region ExitMenu
    void SetExitMenu() {
        exitMenu.noBtn.onClick.AddListener(() => exitMenu.exitMenu.SetActive(false));
        exitMenu.yesBtn.onClick.AddListener(() => Application.Quit());
    }

    #endregion

}
