# Reversi Puzzle

## 프로젝트 설명

본 프로젝트는 구글 플레이에 업로드된 퍼즐 게임 장르의 개인 프로젝트입니다.
오셀로를 모티브로 제작했고, 타일을 누르면, 주변의 타일도 같이 뒤집히면서 모든 타일을 검은색으로 만들면 클리어하는 게임입니다.

 - 제작 기간 2023.04.07 ~ 2023.07.01.
 - 개발 툴: Unity 3D
 - E-mail: yulop64@naver.com
   

## 게임 다운로드

<https://play.google.com/store/apps/details?id=com.Commar.Reversi_Puzzle>


## 시연사진
    center {
      display: block;
      margin: auto;
    }
<center><img src="https://github.com/THISISJUSTICE/PuzzleGame/assets/105614494/522eeeae-cbf0-4e0a-9bb3-836e15826a84" width="30%" height="30%"></left>
<center><img src="https://github.com/THISISJUSTICE/PuzzleGame/assets/105614494/8d8f501e-a969-411e-9086-22a6793b5d22" width="30%" height="30%"></center>
<center><img src="https://github.com/THISISJUSTICE/PuzzleGame/assets/105614494/3d3895df-bc29-4072-8eb8-ddc002428bb1" width="30%" height="30%"></right>

## 주요 스크립트
### (PuzzleGame/PuzzleGame/Assets/Scripts)

Basic_horse: 각 타일을 뒤집는 애니메이션 및 효과음, 발생하는 값을 설정

PlayerData: 플레이어 게임 정보를 Json 파일의 형태로 로컬에 저장 및 불러오기

StageManager: 각 스테이지의 점수 및 클리어 여부 등을 결정, 스테이지 버튼 생성

GameManager: 
- Stage 파일을 참조하여 Basic_horse를 포함하는 프리팹의 생성 및 관리
- 퍼즐의 규칙을 정하여 각 위치와 뒤집어야 하는 타일을 정의
- 스테이지의 클리어, 실패 관리
- 일반 스테이지 클리어 후 발생하는 마스터 모드 스테이지 발현
- 일반 스테이지 생성 기능
- 인게임 내 일부 UI 기능

UIManager:
- 게임 내 모든 UI 관리
- 구글 플레이 로그인, 대시보드 관리
- 구글 Admob 관리
- 사운드, 배경음 관리
