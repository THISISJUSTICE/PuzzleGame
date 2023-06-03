using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameSetting : MonoBehaviour
{
    public GameObject background;

    //인게임 UI
    public bool isScroll;
    public Button resetBtn, settingBtn, lobbyBtn; //리셋 버튼, 세팅 버튼, 로비 화면 이동 버튼
    public Text flipCount, maxFlipCount, stageTitle, allClearTitle; //뒤집은 횟수, 최대 횟수, 제목, 올클리어 제목
    
    //오브젝트 회전 스크롤
    public GameObject scroll3Dobject;
    public Scrollbar vScroll; //세로 스크롤

    //인게임 내 세팅 UI(카메라, 소리 세팅)
    public GameObject ingameSettingMenu; //인게임 세팅 메뉴
    public Button cameraReset, cameraUpBtn, cameraDownBtn; //카메라 조정한 것을 리셋하는 버튼, 카메라 위 이동 버튼, 카메라 아래 이동 버튼
    public Scrollbar cameraZoom; //카메라 줌 스크롤
    public Button exitBtn, shareBtn; //나가기 버튼, 공유 버튼
    public Toggle soundCk; //소리 on/off
    Image[] albedoImg;

    bool isActing; //코루틴이 현재 실행 중인지 확인

    public void Init()
    {
        //인게임 UI
        flipCount = transform.GetChild(0).GetComponent<Text>();
        maxFlipCount = transform.GetChild(1).GetComponent<Text>();
        stageTitle = transform.GetChild(2).GetComponent<Text>();
        allClearTitle = transform.GetChild(3).GetComponent<Text>();
        allClearTitle.gameObject.SetActive(false);
        resetBtn = transform.GetChild(4).GetComponent<Button>();
        settingBtn = transform.GetChild(5).GetComponent<Button>();
        lobbyBtn = transform.GetChild(6).GetComponent<Button>();
        
        //오브젝트 회전 스크롤
        scroll3Dobject = transform.GetChild(7).gameObject;
        vScroll = scroll3Dobject.transform.GetChild(1).GetComponent<Scrollbar>();

        //인게임 내 세팅 UI(카메라, 소리 세팅)
        ingameSettingMenu = transform.GetChild(8).gameObject;
        cameraReset = ingameSettingMenu.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        cameraUpBtn = ingameSettingMenu.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        cameraDownBtn = ingameSettingMenu.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        cameraZoom = ingameSettingMenu.transform.GetChild(0).GetChild(3).GetComponent<Scrollbar>();
        cameraZoom.value = 0.5f;
        exitBtn = ingameSettingMenu.transform.GetChild(1).GetComponent<Button>();
        soundCk = ingameSettingMenu.transform.GetChild(2).GetComponent<Toggle>();
        shareBtn = ingameSettingMenu.transform.GetChild(3).GetComponent<Button>();

        albedoImg = new Image[9];
        albedoImg[0] = ingameSettingMenu.GetComponent<Image>();
        albedoImg[1] = cameraReset.image;
        albedoImg[2] = cameraUpBtn.image;
        albedoImg[3] = cameraDownBtn.image;
        albedoImg[4] = cameraZoom.image;
        albedoImg[5] = cameraZoom.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        albedoImg[6] = exitBtn.image;
        albedoImg[7] = soundCk.transform.GetChild(0).GetComponent<Image>();
        albedoImg[8] = shareBtn.image;

        isActing = false;
    }

    //카메라 설정을 수정할 때마다 보기 편하도록 세팅 메뉴가 투명해짐
    public IEnumerator IngameSettingAlbedo(int type) {
        if (!isActing)
        {
            //필요에 따라 다른 버튼 투명도도 조정
            albedoImg[0].color = new Color(1, 1, 1, 0.3f);
            for (int i = 1; i < albedoImg.Length; i++)
                albedoImg[i].color = new Color(1, 1, 1, 0.6f);
            if (type == 0) albedoImg[1].color = new Color(1, 1, 1, 0.8f);
            else if (type == 1)
            {
                albedoImg[2].color = new Color(1, 1, 1, 0.8f);
                albedoImg[3].color = new Color(1, 1, 1, 0.8f);
            }
            else {
                albedoImg[4].color = new Color(1, 1, 1, 0.8f);
                albedoImg[5].color = new Color(1, 1, 1, 0.8f);
            }
            isActing = true;
            yield return new WaitForSeconds(2);
            for(int i=0; i<albedoImg.Length; i++)
                albedoImg[i].color = new Color(1, 1, 1, 1);
            isActing = false;
        }
    }

    private void OnEnable()
    {
        background.SetActive(true);
        if (isScroll) scroll3Dobject.SetActive(true);

    }

    private void OnDisable()
    {
        background.SetActive(false);
        scroll3Dobject.SetActive(false);
        ingameSettingMenu.SetActive(false);
    }

}
