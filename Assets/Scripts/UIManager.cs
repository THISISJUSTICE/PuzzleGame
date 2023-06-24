using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variable Declaration
    public GameManager gameManager;
    
    public class MainMenu {
        public GameObject mainMenu; //게임 시작 화면
        public Button exitBtn, settingBtn, tutorialBtn, shareBtn; //나가기 버튼, 설정 버튼, 설명 버튼, 공유 버튼
        public Button rectBtn, hexBtn, cubeBtn; //사각 스테이지, 육각 스테이지, 3차원 큐브 스테이지
        public Text nickName, score, rank; //닉네임, 점수, 랭킹
        public Text[] stageScore, stageLevel; //각 스테이지 별 점수, 클리어 수
        public GameObject tutorialMenu; //튜토리얼 메뉴
        public Button[] tutorial; //튜토리얼 메뉴 1, 메뉴 2

        public MainMenu(GameObject mainMenu) {
            this.mainMenu = mainMenu;
            Init();
        }

        void Init() {
            exitBtn = mainMenu.transform.GetChild(0).GetComponent<Button>();
            settingBtn = mainMenu.transform.GetChild(1).GetComponent<Button>();
            tutorialBtn = mainMenu.transform.GetChild(2).GetComponent<Button>();
            shareBtn = mainMenu.transform.GetChild(3).GetComponent<Button>();
            nickName = mainMenu.transform.GetChild(4).GetChild(0).GetComponent<Text>();
            score = mainMenu.transform.GetChild(4).GetChild(1).GetComponent<Text>();
            rank = mainMenu.transform.GetChild(4).GetChild(2).GetComponent<Text>();
            rectBtn = mainMenu.transform.GetChild(5).GetChild(0).GetComponent<Button>();
            hexBtn = mainMenu.transform.GetChild(5).GetChild(1).GetComponent<Button>();
            cubeBtn = mainMenu.transform.GetChild(5).GetChild(2).GetComponent<Button>();
            tutorialMenu = mainMenu.transform.GetChild(6).gameObject;

            stageScore = new Text[3];
            stageLevel = new Text[3];
            tutorial = new Button[2];

            for(int i=0; i<3; i++){
                stageScore[i] = mainMenu.transform.GetChild(5).GetChild(i).GetChild(2).GetChild(1).GetComponent<Text>();
                stageLevel[i] = mainMenu.transform.GetChild(5).GetChild(i).GetChild(2).GetChild(0).GetChild(0).GetComponent<Text>();
            }

            for(int i=0; i<2; i++)
                tutorial[i] = tutorialMenu.transform.GetChild(i).GetComponent<Button>();
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
            stageBtns = new StageManager[3];
            for (int i = 0; i < 3; i++)
            {
                stageBtns[i] = lobbyMenu.transform.GetChild(i).GetComponent<StageManager>();
            }
            homeBtn = lobbyMenu.transform.GetChild(3).GetChild(0).GetComponent<Button>();
            settingBtn = lobbyMenu.transform.GetChild(3).GetChild(2).GetComponent<Button>();
            backBtn = lobbyMenu.transform.GetChild(3).GetChild(3).GetComponent<Button>();
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
            title = clearMenu.transform.GetChild(1).GetComponent<Text>();
            comment = clearMenu.transform.GetChild(2).GetComponent<Text>();
            scoreTxt = clearMenu.transform.GetChild(3).GetComponent<Text>();              
            exitBtn = clearMenu.transform.GetChild(4).GetComponent<Button>();
            backBtn = clearMenu.transform.GetChild(5).GetComponent<Button>();
            nextBtn = clearMenu.transform.GetChild(6).GetComponent<Button>();
            animStar = clearMenu.transform.GetChild(7).GetComponent<Animator>(); 

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
        public Button exitBtn;
        public Toggle soundCk, bgmCK;
        public GameObject soundoffImg;
        public Scrollbar soundScroll, bgmScroll;

        public SettingMenu(GameObject settingMenu)
        {
            this.settingMenu = settingMenu;
            Init();
        }

        void Init()
        {
            exitBtn = settingMenu.transform.GetChild(1).GetChild(0).GetComponent<Button>();
            soundCk = settingMenu.transform.GetChild(1).GetChild(1).GetComponent<Toggle>();
            soundoffImg = soundCk.transform.GetChild(1).GetChild(0).gameObject;
            soundScroll = settingMenu.transform.GetChild(1).GetChild(2).GetComponent<Scrollbar>();
            bgmCK = settingMenu.transform.GetChild(1).GetChild(3).GetComponent<Toggle>();
            bgmScroll = settingMenu.transform.GetChild(1).GetChild(4).GetComponent<Scrollbar>();          
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
    
    public struct Sounds{
        public AudioSource btnSound; //버튼 누르는 소리
        public AudioSource clearSound; //클리어 시 발생하는 소리
        public AudioSource clearStarSound; //클리어 시 별이 떨어지는 소리
        public AudioSource achieve100Sound; //만점 시 발생하는 소리
        public AudioSource failSound; //실패 시 발생하는 소리
        public AudioSource masterClearSound; //마스터 모드에서 클리어 시 발생하는 소리
        public AudioSource scoreSound; //점수가 올라갈 때마다 발생하는 소리
    }
    public Sounds sounds; //효과음

    bool isSaveCoolDown = false; //저장 대기 시간

    public BGM_Player bgm_Player; //배경 음악
    float pauseTime; //인게임에서 배경음악을 일시 정지했을 때, 음악 플레이 시간을 저장하는 변수

    //공유 버튼: 설명, 링크
    private const string subject = "Experience a fun and strategic game! Unleash your strategic prowess as you flip tiles and engage in a game of wits. It's easy to learn for anyone, but mastering it is challenging. Explore various strategies and pave your path to victory. Play now and dive into the excitement!";
	private const string body = "https://play.google.com/store/apps/details?id=com.CEREALLAB.FruitsLoop&showAllReviews=true"; //변경 필요
    
    #endregion

    private void Awake()
    {
        Init();
        UISetting();
    }

    //설정 변경이 있으면 저장
    IEnumerator SaveCycle(){
        if(!isSaveCoolDown){
            PlayerData.Instance.data.soundVolume = settingMenu.soundScroll.value;
            PlayerData.Instance.data.soundCk = settingMenu.soundCk.isOn;
            PlayerData.Instance.data.bgmVolume = settingMenu.bgmScroll.value;
            PlayerData.Instance.data.bgmCk = settingMenu.bgmCK.isOn;
            Debug.Log($"SaveCycle [sound: {PlayerData.Instance.data.soundVolume}, ck: {PlayerData.Instance.data.soundCk}]");
            PlayerData.Instance.SaveData();
            yield return new WaitForSeconds(4);
            isSaveCoolDown = true;
            isSaveCoolDown = false;
        }       
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

    //나중에 진행할 작업
    void LateTask() {
        AudioInit();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < lobbyMenu.stageBtns[i].stageBtns.Length; j++)
            {
                lobbyMenu.stageBtns[i].stageBtns[j].opened.onClick.AddListener(DoBack);
            }
            lobbyMenu.stageBtns[i].gameObject.SetActive(false);
            lobbyMenu.stageBtns[i].masterStageBtn.opened.onClick.AddListener(DoBack);
        }
        //효과음, 배경음을 이전에 세팅한데로 세팅
        if(PlayerData.Instance.data.soundCk) DoSoundMute(true);
        else DoSoundVolume(PlayerData.Instance.data.soundVolume);

        if(PlayerData.Instance.data.bgmCk) DoBGM_Mute(true);
        else DoBGM_Volume(PlayerData.Instance.data.bgmVolume);

        //세팅이 끝난 UI 비활성화
        mainMenu.tutorialMenu.SetActive(false);
        for(int i =0; i<2; i++) mainMenu.tutorial[i].gameObject.SetActive(false);
        lobbyMenu.lobbyMenu.SetActive(false);
        ingameMenu.ingameSettingMenu.SetActive(false);
        ingameMenu.masterGroup.SetActive(false);
        ingameMenu.gameObject.SetActive(false);
        settingMenu.settingMenu.SetActive(false);
        clearMenu.clearMenu.SetActive(false);
        failMenu.failMenu.SetActive(false);
        exitMenu.exitMenu.SetActive(false);
        MainMenuText();
        loadingUI.gameObject.SetActive(false);

        BGMPlay(0);
    }

    //오디오 설정 초기화
    void AudioInit(){
        sounds.btnSound = GetComponent<AudioSource>();
        sounds.clearSound = clearMenu.clearMenu.GetComponent<AudioSource>();
        sounds.clearStarSound = clearMenu.clearMenu.transform.GetChild(7).GetComponent<AudioSource>();
        sounds.achieve100Sound = clearMenu.clearMenu.transform.GetChild(7).GetChild(2).GetComponent<AudioSource>();
        sounds.failSound = failMenu.failMenu.GetComponent<AudioSource>();
        gameManager.horseAudio = gameManager.GetComponent<AudioSource>();
        sounds.masterClearSound = ingameMenu.masterGroup.GetComponent<AudioSource>();
        sounds.scoreSound = ingameMenu.masterScore.GetComponent<AudioSource>();

        //bgm
        bgm_Player.bgmAudio = bgm_Player.GetComponent<AudioSource>();
        pauseTime = 0;
    }

    #region MainMenu
    //메인 메뉴 UI 설정
    void SetMainMenu()
    {
        mainMenu.exitBtn.onClick.AddListener(DoExit);
        mainMenu.settingBtn.onClick.AddListener(DoSetting);
        mainMenu.rectBtn.onClick.AddListener(() => DoStage(0)); //Rectangle
        mainMenu.hexBtn.onClick.AddListener(() => DoStage(1)); //Hexagon
        mainMenu.cubeBtn.onClick.AddListener(() => DoStage(2)); //Cube
        mainMenu.shareBtn.onClick.AddListener(DoShare);
        
        //튜토리얼
        mainMenu.tutorialBtn.onClick.AddListener(() => DoTutorialMenu(mainMenu.tutorialMenu.gameObject, mainMenu.tutorial[0].gameObject, true));
        mainMenu.tutorial[0].onClick.AddListener(() => DoTutorialMenu(mainMenu.tutorial[0].gameObject, mainMenu.tutorial[1].gameObject));
        mainMenu.tutorial[1].onClick.AddListener(() => DoTutorialMenu(mainMenu.tutorial[1].gameObject));
    }

    //메인 메뉴 UI 텍스트 설정
    void MainMenuText() {
        mainMenu.nickName.text = PlayerData.Instance.data.userName;
        mainMenu.score.text = "Score: " + PlayerData.Instance.TotalScore();
        mainMenu.rank.text = "Rank : " + (PlayerData.Instance.data.rank);

        for(int i=0; i<3; i++){
            mainMenu.stageScore[i].text = "" + (PlayerData.Instance.data.stageTotalScore[i] + PlayerData.Instance.data.masterMaxScore[i]);
            if(PlayerData.Instance.data.masterMaxScore[i] > 0)
                mainMenu.stageLevel[i].text = "LV. M";  
            else
                mainMenu.stageLevel[i].text = "LV. " + (PlayerData.Instance.data.clearStage[i] + 1);
        }
    }

    void DoExit()
    {
        exitMenu.exitMenu.SetActive(true);
        sounds.btnSound.Play();
    }

    void DoSetting() {
        settingMenu.settingMenu.SetActive(true);
        sounds.btnSound.Play();
    }

    //스테이지 버튼 4 종류(누를 시 게임 시작)
    void DoStage(int kind) {
        mainMenu.mainMenu.SetActive(false);
        gameManager.GameStart(PlayerData.Instance.data.clearStage[kind] + 1, kind); //플레이어 데이터에서 받은 정보로 매개 변수 입력
        ingameMenu.gameObject.SetActive(true);

        ingameMenu.stageTitle.gameObject.SetActive(true);
        ingameMenu.maxFlipCount.gameObject.SetActive(true);
        sounds.btnSound.Play();
    }

    void DoShare(){
#if UNITY_ANDROID && !UNITY_EDITOR
		using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent")) 
		using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent")) {
			intentObject.Call<AndroidJavaObject>("setAction", intentObject.GetStatic<string>("ACTION_SEND"));
			intentObject.Call<AndroidJavaObject>("setType", "text/plain");
			intentObject.Call<AndroidJavaObject>("putExtra", intentObject.GetStatic<string>("EXTRA_SUBJECT"), subject);
			intentObject.Call<AndroidJavaObject>("putExtra", intentObject.GetStatic<string>("EXTRA_TEXT"), body);
			using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity")) 
			using (AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via"))
			currentActivity.Call("startActivity", jChooser);
		}
#endif
        sounds.btnSound.Play();
    }

    //튜토리얼 메뉴의 다음 페이지로 이동
    void DoTutorialMenu(GameObject self, GameObject next = null, bool menu = false){
        if(menu){
            self.SetActive(true);
            sounds.btnSound.Play();
        }
        else
            self.SetActive(false);

        if(next == null){
            mainMenu.tutorialMenu.SetActive(false);
            return;
        }
          
        next.SetActive(true);
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
        MainMenuText();

        BGMPlay(0);
        gameManager.RealDeleteHorse();
        sounds.btnSound.Play();
    }

    //뒤로 가기 버튼(로비에서 게임 화면으로 다시 넘어감)
    void DoBack() {
        int kind = gameManager.curKind;
        lobbyMenu.lobbyMenu.SetActive(false);
        lobbyMenu.stageBtns[kind].gameObject.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        sounds.btnSound.Play();
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
        ingameMenu.hScroll.onValueChanged.AddListener(gameManager.RotateHorizontal3Dobject);

        //인게임 세팅 메뉴
        ingameMenu.cameraReset.onClick.AddListener(gameManager.CameraReset);
        ingameMenu.cameraUpBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(true));
        ingameMenu.cameraDownBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(false));
        ingameMenu.cameraZoom.onValueChanged.AddListener(gameManager.CameraZoomSetting);
        ingameMenu.exitBtn.onClick.AddListener(() => ingameMenu.ingameSettingMenu.SetActive(false));

        //버튼 소리 세팅
        ingameMenu.settingBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.resetBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.cameraReset.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.cameraUpBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.cameraDownBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.exitBtn.onClick.AddListener(() => sounds.btnSound.Play());

        //소리 온오프, 볼륨
        ingameMenu.soundCk.onValueChanged.AddListener(SoundMute);
        ingameMenu.soundScroll.onValueChanged.AddListener(SoundVolume);
        ingameMenu.bgmCK.onValueChanged.AddListener(BGM_Mute);
        ingameMenu.bgmScroll.onValueChanged.AddListener(BGM_Volume);
    }

    void DoLobby() {
        ingameMenu.gameObject.SetActive(false);
        lobbyMenu.lobbyMenu.SetActive(true);
        lobbyMenu.backBtn.gameObject.SetActive(true);
        lobbyMenu.stageBtns[gameManager.curKind].gameObject.SetActive(true);
        sounds.btnSound.Play();
    }


    #endregion

    #region ClearMenu

    void SetClearMenu() {
        clearMenu.backBtn.onClick.AddListener(DoClearBack);
        clearMenu.nextBtn.onClick.AddListener(DoClearNext);
        clearMenu.exitBtn.onClick.AddListener(DoClearExit);

        //소리 세팅
        clearMenu.backBtn.onClick.AddListener(() => sounds.btnSound.Play());
        clearMenu.nextBtn.onClick.AddListener(() => sounds.btnSound.Play());
        clearMenu.exitBtn.onClick.AddListener(() => sounds.btnSound.Play());
    }

    //스테이지 클리어 후 방금 진행한 스테이지를 바로 또 진행
    void DoClearBack() {
        DoStageBack();
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        sounds.btnSound.Play();
    }

    //방금 진행한 스테이지를 다시 시작
    void DoStageBack() {
        int kind = gameManager.curKind; //현재 스테이지 종류를 담는 변수
        gameManager.GameStart(gameManager.stage[kind], kind);
        ClearInit();
        sounds.btnSound.Play();
    }

    //스테이지 클리어 후 다음 스테이지로 진행
    void DoClearNext() {
        int kind = gameManager.curKind; //현재 스테이지 종류를 담는 변수
        gameManager.stage[kind]++;
        gameManager.GameStart(gameManager.stage[kind], kind);
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        ClearInit();
        sounds.btnSound.Play();
    }

    //스테이지 클리어 후 로비 화면으로 이동
    void DoClearExit() {
        clearMenu.clearMenu.SetActive(false);
        DoLobby();
        lobbyMenu.backBtn.gameObject.SetActive(false);
        ClearInit();
        sounds.btnSound.Play();
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
            float[] starFill = new float[3];
            for(int i=0; i<3; i++) starFill[i] = clearMenu.scoreStar[i].fillAmount;
            clearMenu.animStar.Play("Appear");
            StartCoroutine(StarSoundPlay(score, starFill));
        }
        for (int i = 0; i < 3; i++)
            clearMenu.scoreStar[i].gameObject.SetActive(true);
    }

    IEnumerator StarSoundPlay(int score, float[] starFill){
        for(int i=0; i<3; i++){
            if(starFill[i] > 0) {
                yield return new WaitForSeconds(0.275f);
                sounds.clearStarSound.Play();
            }
        }
        if(score == 100) {
            yield return new WaitForSeconds(0.3f);
            sounds.achieve100Sound.Play();
        }
    }

    #endregion

    #region FailMenu

    void SetFailMenu() {
        failMenu.exitBtn.onClick.AddListener(DoFailExit);
        failMenu.backBtn.onClick.AddListener(DoFailBack);

        //소리 세팅
        failMenu.exitBtn.onClick.AddListener(() => sounds.btnSound.Play());
        failMenu.backBtn.onClick.AddListener(() => sounds.btnSound.Play());
    }

    void DoFailExit() {
        failMenu.failMenu.SetActive(false);
        DoLobby();
        lobbyMenu.backBtn.gameObject.SetActive(false);
        sounds.btnSound.Play();
    }

    void DoFailBack() {
        DoStageBack();
        failMenu.failMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        sounds.btnSound.Play();
    }

    #endregion

    #region SettingMenu
    void SetSettingMenu()
    {
        settingMenu.exitBtn.onClick.AddListener(() => settingMenu.settingMenu.SetActive(false));
        settingMenu.exitBtn.onClick.AddListener(() => sounds.btnSound.Play());
        
        //소리 온오프, 볼륨
        settingMenu.soundCk.onValueChanged.AddListener(SoundMute);
        settingMenu.soundScroll.onValueChanged.AddListener(SoundVolume);

        //배경음 온오프, 볼륨
        settingMenu.bgmCK.onValueChanged.AddListener(BGM_Mute);
        settingMenu.bgmScroll.onValueChanged.AddListener(BGM_Volume);
    }

    //on이 true면 mute
    void SoundMute(bool on){
        DoSoundMute(on);
        if(on)
            DoSoundVolume(0); 
        else {
            DoSoundVolume(0.1f);
            sounds.btnSound.Play();
        }
        StopCoroutine(SettingChangeCheck());
        StartCoroutine(SettingChangeCheck());
    }

    void SoundVolume(float value){
        DoSoundVolume(value);
        if(value == 0) DoSoundMute(true);
        else DoSoundMute(false);
        StopCoroutine(SettingChangeCheck());
        StartCoroutine(SettingChangeCheck());
    }

    //무한 재귀에 빠지지 않도록 Mute와 Volume 변경을 별도로 실행
    void DoSoundMute(bool on){
        sounds.btnSound.mute = on;
        sounds.clearSound.mute = on;
        sounds.clearStarSound.mute = on;
        sounds.achieve100Sound.mute = on;
        sounds.failSound.mute = on;
        gameManager.horseAudio.mute = on;

        //다른 세팅화면에서 눌렀어도 동기화
        settingMenu.soundCk.isOn = on;
        ingameMenu.soundCk.isOn = on;
        settingMenu.soundoffImg.SetActive(!on);
        ingameMenu.soundoffImg.SetActive(!on);
    }

    void DoSoundVolume(float value){
        sounds.btnSound.volume = value;
        sounds.clearSound.volume = value;
        sounds.clearStarSound.volume = value;
        sounds.achieve100Sound.volume = value;
        sounds.failSound.volume = value;
        gameManager.horseAudio.volume = value;

        //다른 세팅화면에서 눌렀어도 동기화
        settingMenu.soundScroll.value = value;
        ingameMenu.soundScroll.value = value;
    }

    void BGM_Mute(bool on){
        DoBGM_Mute(on);
        sounds.btnSound.Play();
        if(on){
            DoBGM_Volume(0);
        }
        else DoBGM_Volume(0.1f);
        StopCoroutine(SettingChangeCheck());
        StartCoroutine(SettingChangeCheck());
    }

    void BGM_Volume(float value){
        DoBGM_Volume(value);
        if(value == 0) DoBGM_Mute(true);
        else DoBGM_Mute(false);
        StopCoroutine(SettingChangeCheck());
        StartCoroutine(SettingChangeCheck());
    }

    void DoBGM_Mute(bool on){
        bgm_Player.bgmAudio.mute = on;

        //다른 세팅화면에서 눌렀어도 동기화
        settingMenu.bgmCK.isOn = on;
        ingameMenu.bgmCK.isOn = on;
    }

    void DoBGM_Volume(float value){
        bgm_Player.bgmAudio.volume = value;

        //다른 세팅화면에서 눌렀어도 동기화
        settingMenu.bgmScroll.value = value;
        ingameMenu.bgmScroll.value = value;
    }

    //설정 변화를 확인해서 변화가 멈추면 데이터 저장
    IEnumerator SettingChangeCheck(){   
        yield return new WaitForSeconds(5);
        StartCoroutine(SaveCycle());
    }

    public void BGMPlay(int type){
        bgm_Player.bgmAudio.clip = bgm_Player.bgms[type];
        if(type == 0)
            bgm_Player.bgmAudio.Play();      
        else{
            bgm_Player.bgmAudio.time = pauseTime;
            bgm_Player.bgmAudio.Play();
        }
    }

    public void BGMPause(){
        pauseTime = bgm_Player.bgmAudio.time;
        bgm_Player.bgmAudio.Pause();
    }
    
    #endregion

    #region ExitMenu
    void SetExitMenu() {
        exitMenu.noBtn.onClick.AddListener(() => exitMenu.exitMenu.SetActive(false));
        exitMenu.yesBtn.onClick.AddListener(() => Application.Quit());

        exitMenu.noBtn.onClick.AddListener(() => sounds.btnSound.Play());
        exitMenu.yesBtn.onClick.AddListener(() => sounds.btnSound.Play());
    }

    #endregion

}
