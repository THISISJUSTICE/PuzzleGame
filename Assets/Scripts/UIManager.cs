using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// - 세팅 화면(공유 버튼, 효과음), 닫기 기능

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    //public PlayerData playerData;

    public class MainMenu {
        public GameObject mainMenu; //게임 시작 화면
        public Button exitBtn, settingButton; //나가기 버튼, 설정 버튼
        public Button quadBtn, hexBtn, cubeBtn, hiveBtn; //사각 스테이지, 육각 스테이지, 3차원 큐브 스테이지, 벌집 스테이지
        public Text nickName, score, rank; //닉네임, 점수, 랭킹

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
    public MainMenu mainMenu; //게임 시작 화면

    public class LobbyMenu {
        public GameObject lobbyMenu; //스테이지 로비 화면
        public StageManager[] stageBtns; //스테이지 버튼 그룹
        public Button homeBtn, settingBtn, backBtn; //홈 버튼, 설정 버튼, 뒤로가기 버튼

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
    public LobbyMenu lobbyMenu; //스테이지 로비 화면

    public IngameSetting ingameMenu; //게임 시작 중 화면

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
    public ClearMenu clearMenu; //클리어 화면

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
    public SettingMenu settingMenu; //세팅 화면

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
    public ExitMenu exitMenu; //나가기 창 (yes or no)

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

    //Task 혹은 델리게이트로 바꾸기
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
    //메인 메뉴 UI 설정
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

    //메인 메뉴 UI 텍스트 설정
    void MainMenuText() {
        //플레이어 데이터에서 불러오기
        mainMenu.nickName.text = PlayerData.Instance.data.userName;
        mainMenu.score.text = "Score: " + PlayerData.Instance.TotalScore();
        mainMenu.rank.text = "Rank : " + 1;
    }

    //버튼
    void DoExit()
    {
        exitMenu.exitMenu.SetActive(true);
    }

    void DoSetting() {
        settingMenu.settingMenu.SetActive(true);
    }

    //스테이지 버튼 4 종류(누를 시 게임 시작)
    void DoStage(int kind) {
        mainMenu.mainMenu.SetActive(false);
        gameManager.GameStart(PlayerData.Instance.data.clearStage[kind] + 1, kind); //플레이어 데이터에서 받은 정보로 매개 변수 입력
        ingameMenu.gameObject.SetActive(true);

        ingameMenu.allClearTitle.gameObject.SetActive(false);
        ingameMenu.stageTitle.gameObject.SetActive(true);
        ingameMenu.flipCount.gameObject.SetActive(true);
        ingameMenu.maxFlipCount.gameObject.SetActive(true);
    }

    #endregion

    #region LobbyMenu
    //로비 메뉴 UI 설정
    void SetLobbyMenu() {
        lobbyMenu.settingBtn.onClick.AddListener(DoSetting);
        lobbyMenu.homeBtn.onClick.AddListener(DoHome);
        lobbyMenu.backBtn.onClick.AddListener(DoBack);
    }

    //홈 버튼 눌렀을 때(현재 진행 중인 스테이지가 초기화되고, 홈 화면으로 이동)
    void DoHome() {
        lobbyMenu.stageBtns[gameManager.curKind].gameObject.SetActive(false);
        lobbyMenu.lobbyMenu.SetActive(false);
        mainMenu.mainMenu.SetActive(true);
        gameManager.RealDeleteHorse();
    }

    //뒤로 가기 버튼(로비에서 게임 화면으로 다시 넘어감)
    void DoBack() {
        int kind = gameManager.curKind;
        lobbyMenu.stageBtns[gameManager.curKind].gameObject.SetActive(false);
        lobbyMenu.lobbyMenu.SetActive(false);
        lobbyMenu.stageBtns[kind].gameObject.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
    }

    #endregion

    #region IngameMenu
    //인게임 화면 세팅
    void SetIngameMenu() {
        //메뉴 버튼, 플립 카운트, 타이틀, 설정 버튼, 리셋 버튼
        ingameMenu.lobbyBtn.onClick.AddListener(DoLobby);
        ingameMenu.settingBtn.onClick.AddListener(() => ingameMenu.ingameSettingMenu.SetActive(true));
        ingameMenu.resetBtn.onClick.AddListener(() => gameManager.ResetBoard());

        //오브젝트 회전
        ingameMenu.vScroll.onValueChanged.AddListener(gameManager.RotateVertical3Dobject);

        //인게임 세팅 메뉴
        ingameMenu.cameraReset.onClick.AddListener(gameManager.CameraReset);
        ingameMenu.cameraUpBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(true));
        ingameMenu.cameraDownBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(false));
        ingameMenu.cameraZoom.onValueChanged.AddListener(gameManager.CameraZoomSetting);
        ingameMenu.exitBtn.onClick.AddListener(() => ingameMenu.ingameSettingMenu.SetActive(false));
        //공유 버튼
        //ingameMenu.shareBtn.onClick.AddListener();
        //소리 온오프
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

    //스테이지 클리어 후 방금 진행한 스테이지를 바로 또 진행
    void DoClearBack() {
        DoStageBack();
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
    }

    //방금 진행한 스테이지를 다시 시작
    void DoStageBack() {
        int kind = gameManager.curKind; //현재 스테이지 종류를 담는 변수
        gameManager.GameStart(gameManager.stage[kind], kind);
        ClearInit();
    }

    //스테이지 클리어 후 다음 스테이지로 진행
    void DoClearNext() {
        int kind = gameManager.curKind; //현재 스테이지 종류를 담는 변수
        gameManager.stage[kind]++;
        gameManager.GameStart(gameManager.stage[kind], kind);
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        ClearInit();
    }

    //스테이지 클리어 후 로비 화면으로 이동
    void DoClearExit() {
        DoStageBack();
        clearMenu.clearMenu.SetActive(false);
        DoLobby();
        ClearInit();
    }

    //클리어 UI 초기화
    void ClearInit() {
        clearMenu.scoreTxt.text = "Score: ";
        for (int i=0; i<3; i++) {
            clearMenu.scoreStar[i].fillAmount = 0;
        }
    }

    //점수에 따른 별을 표시하고 애니메이션 실행
    public void ClearStar(int score, bool isRenew)
    {
        switch (score)
        {
            case 100:
                //별 3개
                for (int i = 0; i < 3; i++) 
                    clearMenu.scoreStar[i].fillAmount = 1;               
                break;
            case int n when (n < 100 && n >= 75):
                //별 2.5개
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 1;
                clearMenu.scoreStar[2].fillAmount = 0.5f;
                break;
            case int n when (n < 75 && n >= 50):
                //별 2개
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 1;
                clearMenu.scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 50 && n >= 25):
                //별 1.5개
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 0.5f;
                clearMenu.scoreStar[2].fillAmount = 0;
                break;
            case int n when (n < 25 && n >= 15):
                //별 1개
                clearMenu.scoreStar[0].fillAmount = 1;
                clearMenu.scoreStar[1].fillAmount = 0;
                clearMenu.scoreStar[2].fillAmount = 0;
                break;
            case 5:
                //별 0.5개
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
        //공유 버튼
        //settingMenu.shareBtn.onClick.AddListener();
        //소리 온오프
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
