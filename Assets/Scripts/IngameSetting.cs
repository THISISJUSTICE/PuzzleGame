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
    public Text maxFlipCount, stageTitle, masterScore, masterMaxScore; //�ִ� Ƚ��, ����, ������ ��� ����, ������ ��� �ְ� ����

    public GameObject masterGroup, crownIcon; //������ ��� UI �׷�, �ְ� ���� ���� �հ� ������
    
    //������Ʈ ȸ�� ��ũ��
    public GameObject scroll3Dobject;
    public Scrollbar vScroll, hScroll; //���� ��ũ��, ���� ��ũ��

    //�ΰ��� �� ���� UI(ī�޶�, �Ҹ� ����)
    public GameObject ingameSettingMenu; //�ΰ��� ���� �޴�
    public Button cameraReset, cameraUpBtn, cameraDownBtn; //ī�޶� ������ ���� �����ϴ� ��ư, ī�޶� �� �̵� ��ư, ī�޶� �Ʒ� �̵� ��ư
    public Scrollbar cameraZoom; //ī�޶� �� ��ũ��
    public Button exitBtn; //������ ��ư
    public Toggle soundCk, bgmCK; //ȿ����, ����� on/off
    public GameObject soundoffImg; //ȿ���� ��� off �̹���
    public Scrollbar soundScroll, bgmScroll; //ȿ���� ����, ����� ����
    Image[] albedoImg;

    bool isActing; //�ڷ�ƾ�� ���� ���� ������ Ȯ��

    public void Init()
    {
        //�ΰ��� UI
        maxFlipCount = transform.GetChild(0).GetComponent<Text>();
        stageTitle = transform.GetChild(1).GetComponent<Text>();
        resetBtn = transform.GetChild(2).GetComponent<Button>();
        settingBtn = transform.GetChild(3).GetComponent<Button>();
        lobbyBtn = transform.GetChild(4).GetComponent<Button>();
        masterGroup = transform.GetChild(5).gameObject;
        masterScore = masterGroup.transform.GetChild(1).GetComponent<Text>();
        crownIcon = masterGroup.transform.GetChild(2).gameObject;
        masterMaxScore = masterGroup.transform.GetChild(3).GetComponent<Text>();
        
        //������Ʈ ȸ�� ��ũ��
        scroll3Dobject = transform.GetChild(6).gameObject;
        vScroll = scroll3Dobject.transform.GetChild(1).GetComponent<Scrollbar>();
        hScroll = scroll3Dobject.transform.GetChild(0).GetComponent<Scrollbar>();

        //�ΰ��� �� ���� UI(ī�޶�, �Ҹ� ����)
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

        //���� ������
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

    //ī�޶� ������ ������ ������ ���� ���ϵ��� ���� �޴��� ��������
    public IEnumerator IngameSettingAlbedo(int type) {
        if (!isActing)
        {
            //�ʿ信 ���� �ٸ� ��ư ������ ����
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
