Cardfight!! Vanguard Fan Game / Replica Project

Unity 및 C# 기반으로 제작한 2D TCG 게임 프로젝트입니다.
대량의 카드 데이터 파이프라인 구축, Addressables 기반 비동기 리소스 관리, UI 성능 최적화에 중점을 두고 개발했습니다.

시연 및 주요 기능
덱 빌딩 및 카드 필터링 시연 (GIF/Video)

Addressables 기반 비동기 카드 로딩 및 메모리 해제 시연 (GIF/Video)

Tech Stack & Environment
Engine: Unity 2022.3.12f1 (2D)

Language: C#

Key Modules: Addressables, Recyclable Scroll Rect, GSTU (Google Sheets To Unity), JSON Data Parsing

Key Architecture & Features (주요 구현 역량)
1. GSTU(Google Sheets To Unity) 기반 카드 메타데이터 파이프라인
문제점: 수백 장의 카드 데이터 수치 및 밸런스 변경 시 매번 클라이언트 코드를 수정하거나 로컬 파일을 수동으로 업데이트하는 비효율 발생.

해결방안: GSTU 플러그인을 활용하여 구글 스프레드시트의 Raw 데이터를 쿼리하고, 이를 C# Card 객체 데이터베이스로 자동 파싱·변환하는 파이프라인 구축.

성과: 기획 데이터 변경 시 재빌드 없이 에디터 내 동기화만으로 최신 스탯 반영이 가능해져 데이터 관리 및 개발 생산성 향상.

2. Recyclable Scroll Rect를 활용한 덱 빌더 UI 성능 최적화
문제점: 덱 빌더 카드 풀 출력 시 수백 개의 카드 UI GameObject 생성으로 인한 Draw Call 급증 및 Canvas Rebatching 병목 현상.

해결방안: IRecyclableScrollRectDataSource 인터페이스를 구현하여 화면에 노출되는 Cell에만 데이터를 동적 재바인딩(Re-bind)하는 Object Pooling 기반 스크롤 구조 설계.

성과: 수백 장의 카드 데이터를 출력해도 스크롤 시 60 FPS 유지 및 UI 관련 메모리 점유율 대폭 감소.

3. Addressables 기반 비동기 카드 리소스 및 메모리 최적화
문제점: 수백 장의 고화질 카드 스프라이트를 동기 로딩(Resources.Load)할 경우 초기 로딩 지연 및 메모리 워크로드 급증.

해결방안: Addressable Asset System을 도입하여 필요한 시점에 카드 일러스트를 비동기(Async) 로딩하고, 중복 로드를 방지하는 캐싱 기법 적용.

성과: 화면에서 벗어나거나 더 이상 사용하지 않는 리소스를 동적 해제(Release)하여 메모리 누수 방지 및 로딩 스파이크 최소화.

* 📄 [DeckValidator.cs](https://github.com/cientodos/Vanguard/blob/main/Assets/Script/Modules/DeckBuilder/Controllers/DeckValidator.cs)
  * **핵심 역할**: 메인/라이드/엑스트라 덱 제약 조건 및 동일 카드 수량 제한 검증 로직
* 📄 [CardReader.cs](https://github.com/cientodos/Vanguard/blob/main/Assets/Script/Data/Databases/CardReader.cs)
  * **핵심 역할**: GSTU 기반 구글 시트 연동 및 카드 메타데이터 자동 파싱 파이프라인
* 📄 [CardPoolListController.cs](https://github.com/cientodos/Vanguard/blob/main/Assets/Script/Modules/DeckBuilder/CardPoolListController.cs)
  * **핵심 역할**: `IRecyclableScrollRectDataSource` 구현을 통한 UI Virtualization 및 데이터 재바인딩 최적화
* 📄 [CardItemCell.cs](https://github.com/cientodos/Vanguard/blob/main/Assets/Script/Modules/DeckBuilder/CardItemCell.cs)
  * **핵심 역할**: Recyclable UI 셀의 Addressables 비동기 이미지 로딩, 메모리 즉시 해제(`ReleaseSprite`), 예외 처리 및 클릭 이벤트 제어
