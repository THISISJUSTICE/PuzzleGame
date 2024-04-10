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
    public Text maxFlipCount, stageTitle, masterScore, masterMaxScore; //최대 횟수, 제목, 마스터 모드 점수, 마스터 모드 최고 점수

    public GameObject masterGroup, crownIcon; //마스터 모드 UI 그룹, 최고 점수 옆의 왕관 아이콘
    
    //오브젝트 회전 스크롤
    public GameObject scroll3Dobject;
    public Scrollbar vScroll, hScroll; //세로 스크롤, 가로 스크롤

    //인게임 내 세팅 UI(카메라, 소리 세팅)
    public GameObject ingameSettingMenu; //인게임 세팅 메뉴
    public Button cameraReset, cameraUpBtn, cameraDownBtn; //카메라 조정한 것을 리셋하는 버튼, 카메라 위 이동 버튼, 카메라 아래 이동 버튼
    public Scrollbar cameraZoom; //카메라 줌 스크롤
    public Button exitBtn; //나가기 버튼
    public Toggle soundCk, bgmCK; //효과음, 배경음 on/off
    public GameObject soundoffImg; //효과음 토글 off 이미지
    public Scrollbar soundScroll, bgmScroll; //효과음 볼륨, 배경음 볼륨
    Image[] albedoImg;

    bool isActing; //코루틴이 현재 실행 중인지 확인

    public void Init()
    {
        //인게임 UI
        maxFlipCount = transform.GetChild(0).GetComponent<Text>();
        stageTitle = transform.GetChild(1).GetComponent<Text>();
        resetBtn = transform.GetChild(2).GetComponent<Button>();
        settingBtn = transform.GetChild(3).GetComponent<Button>();
        lobbyBtn = transform.GetChild(4).GetComponent<Button>();
        masterGroup = transform.GetChild(5).gameObject;
        masterScore = masterGroup.transform.GetChild(1).GetComponent<Text>();
        crownIcon = masterGroup.transform.GetChild(2).gameObject;
        masterMaxScore = masterGroup.transform.GetChild(3).GetComponent<Text>();
        
        //오브젝트 회전 스크롤
        scroll3Dobject = transform.GetChild(6).gameObject;
        vScroll = scroll3Dobject.transform.GetChild(1).GetComponent<Scrollbar>();
        hScroll = scroll3Dobject.transform.GetChild(0).GetComponent<Scrollbar>();

        //인게임 내 세팅 UI(카메라, 소리 세팅)
        ingameSettingMenu = transform.GetChild(7).gameObject;
        cameraReset = ingameSettingMenu.transform.GetChild(1).GetChild(0).GetComponent<Button>();
        cameraUpBtn = ingameSettingMenu.transform.GetChild(1).GetChild(1).GetComponent<Button>();
        cameraDownBtn = ingameSettingMenu.transform.GetChild(1).GetChild(2).GetComponent<Button>();
        cameraZoom = ingameSettingMenu.transform.GetChild(1).GetChild(3).GetComponent<Scrollbar>();
        cameraZoom.value = 0.5f;
        exitBtn = ingameSettingMenu.transform.GetChild(2).GetComponent<Button>();
        soundCk = ingameSettingMenu.transform.GetChild(3).GetComponent<Toggle>();
        soundoffImg = soundCk.transform.GetChild(1).GetChild(0).gameObject;
        soundScroll = ingameSettingMenu.transform.GetChild(4).GetComponent<Scrollbar>();
        bgmCK = ingameSettingMenu.transform.GetChild(5).GetComponent<Toggle>();
        bgmScroll = ingameSettingMenu.transform.GetChild(6).GetComponent<Scrollbar>();

        //투명도 설정용
        albedoImg = new Image[17];
        albedoImg[0] = ingameSettingMenu.GetComponent<Image>();
        albedoImg[1] = cameraReset.image;
        albedoImg[2] = cameraUpBtn.image;
        albedoImg[3] = cameraDownBtn.image;
        albedoImg[4] = cameraZoom.image;
        albedoImg[5] = cameraZoom.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        albedoImg[6] = exitBtn.image;
        albedoImg[7] = soundCk.transform.GetChild(0).GetComponent<Image>();
        albedoImg[8] = soundCk.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        albedoImg[9] = soundCk.transform.GetChild(1).GetChild(1).GetComponent<Image>();
        albedoImg[10] = soundScroll.GetComponent<Image>();
        albedoImg[11] = soundScroll.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        albedoImg[12] = bgmCK.transform.GetChild(0).GetComponent<Image>();
        albedoImg[13] = bgmCK.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        albedoImg[14] = bgmCK.transform.GetChild(1).GetComponent<Image>();
        albedoImg[15] = bgmScroll.GetComponent<Image>();
        albedoImg[16] = bgmScroll.transform.GetChild(0).GetChild(0).GetComponent<Image>();

        isActing = false;
    }

    //카메라 설정을 수정할 때마다 보기 편하도록 세팅 메뉴가 투명해짐
    public IEnumerator IngameSettingAlbedo(int type) {
        if (!isActing)
        {
            //필요에 따라 다른 버튼 투명도도 조정
            albedoImg[0].color = new Color(1, 1, 1, 0.3f);
            for (int i = 1; i < albedoImg.Length; i++)
                albedoImg[i].color = new Color(albedoImg[i].color.r, albedoImg[i].color.g, albedoImg[i].color.b, 0.6f);
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
                albedoImg[i].color = new Color(albedoImg[i].color.r, albedoImg[i].color.g, albedoImg[i].color.b, 1);
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
