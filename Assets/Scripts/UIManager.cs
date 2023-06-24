using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variable Declaration
    public GameManager gameManager;
    
    public class MainMenu {
        public GameObject mainMenu; //���� ���� ȭ��
        public Button exitBtn, settingBtn, tutorialBtn, shareBtn; //������ ��ư, ���� ��ư, ���� ��ư, ���� ��ư
        public Button rectBtn, hexBtn, cubeBtn; //�簢 ��������, ���� ��������, 3���� ť�� ��������
        public Text nickName, score, rank; //�г���, ����, ��ŷ
        public Text[] stageScore, stageLevel; //�� �������� �� ����, Ŭ���� ��
        public GameObject tutorialMenu; //Ʃ�丮�� �޴�
        public Button[] tutorial; //Ʃ�丮�� �޴� 1, �޴� 2

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
    
    public struct Sounds{
        public AudioSource btnSound; //��ư ������ �Ҹ�
        public AudioSource clearSound; //Ŭ���� �� �߻��ϴ� �Ҹ�
        public AudioSource clearStarSound; //Ŭ���� �� ���� �������� �Ҹ�
        public AudioSource achieve100Sound; //���� �� �߻��ϴ� �Ҹ�
        public AudioSource failSound; //���� �� �߻��ϴ� �Ҹ�
        public AudioSource masterClearSound; //������ ��忡�� Ŭ���� �� �߻��ϴ� �Ҹ�
        public AudioSource scoreSound; //������ �ö� ������ �߻��ϴ� �Ҹ�
    }
    public Sounds sounds; //ȿ����

    bool isSaveCoolDown = false; //���� ��� �ð�

    public BGM_Player bgm_Player; //��� ����
    float pauseTime; //�ΰ��ӿ��� ��������� �Ͻ� �������� ��, ���� �÷��� �ð��� �����ϴ� ����

    //���� ��ư: ����, ��ũ
    private const string subject = "Experience a fun and strategic game! Unleash your strategic prowess as you flip tiles and engage in a game of wits. It's easy to learn for anyone, but mastering it is challenging. Explore various strategies and pave your path to victory. Play now and dive into the excitement!";
	private const string body = "https://play.google.com/store/apps/details?id=com.CEREALLAB.FruitsLoop&showAllReviews=true"; //���� �ʿ�
    
    #endregion

    private void Awake()
    {
        Init();
        UISetting();
    }

    //���� ������ ������ ����
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

    //���߿� ������ �۾�
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
        //ȿ����, ������� ������ �����ѵ��� ����
        if(PlayerData.Instance.data.soundCk) DoSoundMute(true);
        else DoSoundVolume(PlayerData.Instance.data.soundVolume);

        if(PlayerData.Instance.data.bgmCk) DoBGM_Mute(true);
        else DoBGM_Volume(PlayerData.Instance.data.bgmVolume);

        //������ ���� UI ��Ȱ��ȭ
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

    //����� ���� �ʱ�ȭ
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
    //���� �޴� UI ����
    void SetMainMenu()
    {
        mainMenu.exitBtn.onClick.AddListener(DoExit);
        mainMenu.settingBtn.onClick.AddListener(DoSetting);
        mainMenu.rectBtn.onClick.AddListener(() => DoStage(0)); //Rectangle
        mainMenu.hexBtn.onClick.AddListener(() => DoStage(1)); //Hexagon
        mainMenu.cubeBtn.onClick.AddListener(() => DoStage(2)); //Cube
        mainMenu.shareBtn.onClick.AddListener(DoShare);
        
        //Ʃ�丮��
        mainMenu.tutorialBtn.onClick.AddListener(() => DoTutorialMenu(mainMenu.tutorialMenu.gameObject, mainMenu.tutorial[0].gameObject, true));
        mainMenu.tutorial[0].onClick.AddListener(() => DoTutorialMenu(mainMenu.tutorial[0].gameObject, mainMenu.tutorial[1].gameObject));
        mainMenu.tutorial[1].onClick.AddListener(() => DoTutorialMenu(mainMenu.tutorial[1].gameObject));
    }

    //���� �޴� UI �ؽ�Ʈ ����
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

    //�������� ��ư 4 ����(���� �� ���� ����)
    void DoStage(int kind) {
        mainMenu.mainMenu.SetActive(false);
        gameManager.GameStart(PlayerData.Instance.data.clearStage[kind] + 1, kind); //�÷��̾� �����Ϳ��� ���� ������ �Ű� ���� �Է�
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

    //Ʃ�丮�� �޴��� ���� �������� �̵�
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
        MainMenuText();

        BGMPlay(0);
        gameManager.RealDeleteHorse();
        sounds.btnSound.Play();
    }

    //�ڷ� ���� ��ư(�κ񿡼� ���� ȭ������ �ٽ� �Ѿ)
    void DoBack() {
        int kind = gameManager.curKind;
        lobbyMenu.lobbyMenu.SetActive(false);
        lobbyMenu.stageBtns[kind].gameObject.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        sounds.btnSound.Play();
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
        ingameMenu.hScroll.onValueChanged.AddListener(gameManager.RotateHorizontal3Dobject);

        //�ΰ��� ���� �޴�
        ingameMenu.cameraReset.onClick.AddListener(gameManager.CameraReset);
        ingameMenu.cameraUpBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(true));
        ingameMenu.cameraDownBtn.onClick.AddListener(() => gameManager.CameraVerticalSetting(false));
        ingameMenu.cameraZoom.onValueChanged.AddListener(gameManager.CameraZoomSetting);
        ingameMenu.exitBtn.onClick.AddListener(() => ingameMenu.ingameSettingMenu.SetActive(false));

        //��ư �Ҹ� ����
        ingameMenu.settingBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.resetBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.cameraReset.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.cameraUpBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.cameraDownBtn.onClick.AddListener(() => sounds.btnSound.Play());
        ingameMenu.exitBtn.onClick.AddListener(() => sounds.btnSound.Play());

        //�Ҹ� �¿���, ����
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

        //�Ҹ� ����
        clearMenu.backBtn.onClick.AddListener(() => sounds.btnSound.Play());
        clearMenu.nextBtn.onClick.AddListener(() => sounds.btnSound.Play());
        clearMenu.exitBtn.onClick.AddListener(() => sounds.btnSound.Play());
    }

    //�������� Ŭ���� �� ��� ������ ���������� �ٷ� �� ����
    void DoClearBack() {
        DoStageBack();
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        sounds.btnSound.Play();
    }

    //��� ������ ���������� �ٽ� ����
    void DoStageBack() {
        int kind = gameManager.curKind; //���� �������� ������ ��� ����
        gameManager.GameStart(gameManager.stage[kind], kind);
        ClearInit();
        sounds.btnSound.Play();
    }

    //�������� Ŭ���� �� ���� ���������� ����
    void DoClearNext() {
        int kind = gameManager.curKind; //���� �������� ������ ��� ����
        gameManager.stage[kind]++;
        gameManager.GameStart(gameManager.stage[kind], kind);
        clearMenu.clearMenu.SetActive(false);
        ingameMenu.gameObject.SetActive(true);
        ClearInit();
        sounds.btnSound.Play();
    }

    //�������� Ŭ���� �� �κ� ȭ������ �̵�
    void DoClearExit() {
        clearMenu.clearMenu.SetActive(false);
        DoLobby();
        lobbyMenu.backBtn.gameObject.SetActive(false);
        ClearInit();
        sounds.btnSound.Play();
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

        //�Ҹ� ����
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
        
        //�Ҹ� �¿���, ����
        settingMenu.soundCk.onValueChanged.AddListener(SoundMute);
        settingMenu.soundScroll.onValueChanged.AddListener(SoundVolume);

        //����� �¿���, ����
        settingMenu.bgmCK.onValueChanged.AddListener(BGM_Mute);
        settingMenu.bgmScroll.onValueChanged.AddListener(BGM_Volume);
    }

    //on�� true�� mute
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

    //���� ��Ϳ� ������ �ʵ��� Mute�� Volume ������ ������ ����
    void DoSoundMute(bool on){
        sounds.btnSound.mute = on;
        sounds.clearSound.mute = on;
        sounds.clearStarSound.mute = on;
        sounds.achieve100Sound.mute = on;
        sounds.failSound.mute = on;
        gameManager.horseAudio.mute = on;

        //�ٸ� ����ȭ�鿡�� ����� ����ȭ
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

        //�ٸ� ����ȭ�鿡�� ����� ����ȭ
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

        //�ٸ� ����ȭ�鿡�� ����� ����ȭ
        settingMenu.bgmCK.isOn = on;
        ingameMenu.bgmCK.isOn = on;
    }

    void DoBGM_Volume(float value){
        bgm_Player.bgmAudio.volume = value;

        //�ٸ� ����ȭ�鿡�� ����� ����ȭ
        settingMenu.bgmScroll.value = value;
        ingameMenu.bgmScroll.value = value;
    }

    //���� ��ȭ�� Ȯ���ؼ� ��ȭ�� ���߸� ������ ����
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
