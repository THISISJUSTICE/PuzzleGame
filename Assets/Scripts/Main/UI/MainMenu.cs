using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main.UI
{
    public class MainMenu : MonoBehaviour
    {
        #region Variables
        private Main.UI.UIManager _uiManager;

        [SerializeField] private Button _settingBtn;
        [SerializeField] private Button _tutorialBtn;
        [SerializeField] private Button _shareBtn;
        [SerializeField] private Button _exitBtn;

        [SerializeField] private ScoreGroup _scoreGroup;
        [SerializeField] private GooglePlay _googlePlay;
        [SerializeField] private StageButtons _stageButtons;

        private const string subject = "Experience a fun and strategic game! Unleash your strategic prowess as you flip tiles and engage in a game of wits. It's easy to learn for anyone, but mastering it is challenging. Explore various strategies and pave your path to victory. Play now and dive into the excitement!";
        private const string body = "https://play.google.com/store/apps/details?id=com.Commar.Reversi_Puzzle";
        #endregion

        #region Unity Functions
        private void Awake()
        {
            _uiManager = transform.parent.GetComponent<Main.UI.UIManager>();
        }

        #endregion

        public void Init() {
            UISetting();
            _scoreGroup.UISetting();
            _googlePlay.UISetting();
            _stageButtons.UISetting();
        }

        void UISetting() {
            // TODO: 사운드
            _settingBtn.onClick.AddListener(() =>
            {
                _uiManager.SettingMenu.gameObject.SetActive(true);
            });

            _tutorialBtn.onClick.AddListener(() =>
            {
                _uiManager.TutorialMenu.gameObject.SetActive(true);
            });

            _shareBtn.onClick.AddListener(() =>
            {
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
            });

            _exitBtn.onClick.AddListener(() =>
            {
                _uiManager.ExitCheck.gameObject.SetActive(true);
            });
        }

    }
}