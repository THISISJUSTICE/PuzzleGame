using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameSetting : MonoBehaviour
{
    public GameObject background;

    //�ΰ��� UI
    public bool isScroll;
    public Button resetBtn, settingBtn, lobbyBtn; //���� ��ư, ���� ��ư, �κ� ȭ�� �̵� ��ư
    public Text flipCount, maxFlipCount, stageTitle, allClearTitle; //������ Ƚ��, �ִ� Ƚ��, ����, ��Ŭ���� ����
    
    //������Ʈ ȸ�� ��ũ��
    public GameObject scroll3Dobject;
    public Scrollbar vScroll; //���� ��ũ��

    //�ΰ��� �� ���� UI(ī�޶�, �Ҹ� ����)
    public GameObject ingameSettingMenu; //�ΰ��� ���� �޴�
    public Button cameraReset, cameraUpBtn, cameraDownBtn; //ī�޶� ������ ���� �����ϴ� ��ư, ī�޶� �� �̵� ��ư, ī�޶� �Ʒ� �̵� ��ư
    public Scrollbar cameraZoom; //ī�޶� �� ��ũ��
    public Button exitBtn, shareBtn; //������ ��ư, ���� ��ư
    public Toggle soundCk; //�Ҹ� on/off
    Image[] albedoImg;

    bool isActing; //�ڷ�ƾ�� ���� ���� ������ Ȯ��

    public void Init()
    {
        //�ΰ��� UI
        flipCount = transform.GetChild(0).GetComponent<Text>();
        maxFlipCount = transform.GetChild(1).GetComponent<Text>();
        stageTitle = transform.GetChild(2).GetComponent<Text>();
        allClearTitle = transform.GetChild(3).GetComponent<Text>();
        allClearTitle.gameObject.SetActive(false);
        resetBtn = transform.GetChild(4).GetComponent<Button>();
        settingBtn = transform.GetChild(5).GetComponent<Button>();
        lobbyBtn = transform.GetChild(6).GetComponent<Button>();
        
        //������Ʈ ȸ�� ��ũ��
        scroll3Dobject = transform.GetChild(7).gameObject;
        vScroll = scroll3Dobject.transform.GetChild(1).GetComponent<Scrollbar>();

        //�ΰ��� �� ���� UI(ī�޶�, �Ҹ� ����)
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

    //ī�޶� ������ ������ ������ ���� ���ϵ��� ���� �޴��� ��������
    public IEnumerator IngameSettingAlbedo(int type) {
        if (!isActing)
        {
            //�ʿ信 ���� �ٸ� ��ư ������ ����
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
