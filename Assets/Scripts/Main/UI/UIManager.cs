using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Variable
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private SettingMenu _settingMenu;
        [SerializeField] private ExitCheck _exitCheck;
        [SerializeField] private TutorialMenu _tutorialMenu;

        public MainMenu MainMenu { get { return _mainMenu; } }
        public SettingMenu SettingMenu { get { return _settingMenu; } }
        public ExitCheck ExitCheck { get { return _exitCheck; } }
        public TutorialMenu TutorialMenu { get { return _tutorialMenu; } }
        #endregion

        #region Unity Functions
        private void Awake()
        {
            
        }

        private void Start()
        {
            _mainMenu.Init();
            _settingMenu.Init();
            _exitCheck.Init();
        }
        #endregion

    }
}