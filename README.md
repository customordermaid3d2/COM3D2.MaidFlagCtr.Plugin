# COM3D2.MaidFlagCtr.Plugin

BepInEx plugin
메이드 플레그 수정

![2021-06-08 17 46 26](https://user-images.githubusercontent.com/20321215/121155615-ed736b00-c882-11eb-8332-4b5c076060f2.png)


# 설치 위치

COM3D2\BepInEx\plugins


# 필요한거

- BepInEx https://github.com/BepInEx/BepInEx  

- WindowRectUtill https://github.com/customordermaid3d2/COM3D2.WindowRectUtill  
- COM3D2 API edit version https://github.com/customordermaid3d2/COM3D2_Plugins  


# 참고

## All Maid Flag Setting  

메이드 성격별로 플레그를 수집함  
All Maid Flag Setting 버튼 누를시, 모든 메이드에게 자기 성격에 맞는 수집한 플레그를 전부 집어넣음  
플래그를 수정하는 관계로, 플레그를 이용한 이벤트나 스케즐 방시중 완료 여부 등 온갖 특이사항이 발생할수 있음  
사용전 백업 권장  

All Maid Flag save 누를시 아래 두개 파일로 성격목록이 저장됨  
COM3D2.MaidFlagCtr.Plugin-flagsOld.json  
COM3D2.MaidFlagCtr.Plugin-flags.json  
게임 시작시 자동으로 불러옴  
버튼 누르지 앟아도 게임 정상 종료시에도 자동 저장됨  