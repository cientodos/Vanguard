# 🎴 Cardfight!! Vanguard Fan Game / Replica Project
> Unity 및 C# 기반으로 제작한 2D TCG 게임 프로젝트입니다.
> 대량의 카드 데이터 처리, 비동기 리소스 로딩, UI 성능 최적화에 중점을 두고 개발했습니다.

## 📹 시연 및 주요 기능 (GIF / Video)
- [카드 검색 및 덱 빌딩 시연 GIF]
- [Addressable 기반 카드 비동기 로딩 시연 GIF]

## 🛠 Tech Stack & Environment
- **Engine**: Unity 2022.3.12f1 (2D)
- **Language**: C#
- **Key Modules**: Addressables, Recyclable Scroll Rect, JSON Data Parsing

## ⚙️ Key Architecture & Features (주요 구현 역량)

### 1. Addressables 기반 카드 리소스 비동기 로딩 및 메모리 최적화
- **문제점**: 수백 장의 카드 고화질 스프라이트를 동기 로딩 시 메모리 점유율 급증 및 프레임 드랍 발생.
- **해결방안**: Addressable Asset System을 도입해 카드 일러스트 및 메타데이터를 비동기(Async) 로딩. 
- **성과**: 화면에 표시되지 않는 카드 리소스를 동적 해제(Release)하여 메모리 워크로드 절감 및 로딩 스파이크 방지.

### 2. Recyclable Scroll Rect를 활용한 덱 빌더 UI 성능 최적화
- **문제점**: 덱 빌더에서 수백 개의 카드 UI 오브젝트(Instantiate) 생성 시 Draw Call 및 캔버스 재연산(Canvas Rebatching)으로 인한 성능 저하.
- **해결방안**: Object Pooling 기법 기반의 `Recyclable Scroll Rect`를 도입하여 화면에 노출되는 Cell만 재사용하도록 구조화.
- **성과**: 카드 목록 스크롤 시 60 FPS 유지 및 UI 메모리 점유 대폭 감소.

### 3. DeckManager 및 JSON 기반 덱 데이터 구조화
- **구현 내용**: 메인 덱, 라이드 덱, 엑스트라 덱의 제약 조건 규칙을 검증하는 `DeckVaildator` 구현.
- **데이터 관리**: 카드 ID 및 덱 구성을 JSON으로 직렬화/역직렬화하여 데이터 영속성 확보 및 고레어 카드 정렬 알고리즘 적용.

## 📂 Key Source Code Links
- 📄 [DeckManager.cs](링크주소): 덱 검증 및 라이드/엑스트라 덱 로직 관리
- 📄 [CardResourceLoader.cs](링크주소): Addressables 기반 비동기 카드 이미지 로더
- 📄 [DeckBuilderScrollView.cs](링크주소): Recyclable Scroll Rect 기반 UI 풀링 로직
