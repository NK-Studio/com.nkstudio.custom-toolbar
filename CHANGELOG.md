# Changelog

이 문서는 [Keep a Changelog](https://keepachangelog.com/ko/1.1.0/) 형식을 따르며 버전은 [Semantic Versioning](https://semver.org/)을 사용합니다.

## [2.0.0] - 2026-08-11

### Added

- `IToolbarModule`, `ILegacyToolbarModule`, `ToolbarModuleRegistry` 기반의 명시적 모듈 확장 API
- `Project Settings > NKStudio > Custom Toolbar` 설정 화면
- 기능 표시 여부, 씬 검색 폴더, Time Scale 범위 및 Reset 값 설정
- Unity 2022.3~6.2용 Time Scale 슬라이더
- 모듈 등록과 설정 검증을 위한 EditMode 테스트
- Unity 2022.3, 6000.0, 6000.3, 6000.5 컴파일 검증
- 프로젝트 Unity 버전을 확인해 올바른 모듈 API를 선택하는 `extend-custom-toolbar` AI 스킬

### Changed

- Unity 6.3 이상 구현을 공식 `MainToolbar.Refresh` 수명주기로 전환
- Unity 2022.3~6.2 리플렉션을 단일 Legacy Host로 제한
- FMOD와 Localization을 선택적 독립 어셈블리로 분리
- 씬 목록 갱신을 1초 폴링에서 Build Settings 및 설정 변경 이벤트 방식으로 변경
- 모든 코드를 `NKStudio.CustomToolbar` 네임스페이스로 정리
- FMOD Debug, Enter Play Mode, Scene Switcher를 Play Mode에서 일관되게 비활성화

### Fixed

- 반복적인 툴바 재생성 시 UI와 Editor 이벤트가 중복 등록될 수 있던 문제
- Build Settings에 활성 씬이 없을 때 첫 씬 버튼이 잘못 동작할 수 있던 문제
- 중복 이름의 씬과 여러 검색 폴더 처리
- Domain Reload 설정과 툴바 레이아웃 변경 이후 Legacy Toolbar가 복원되지 않을 수 있던 문제

### Removed

- `ToolbarExtensionRegistry.LeftToolbarGUI`
- `ToolbarEventHandler`
- 기능별 익명 `playModeStateChanged` 구독
- Unity 6.3 이상 `MainToolbarWindow` 리플렉션과 생성된 UI 이름 탐색

### Migration

- 기존 `ToolbarExtensionRegistry.LeftToolbarGUI` 확장 코드는 `IToolbarModule` 구현과 `ToolbarModuleRegistry.Register()` 호출로 전환해야 합니다.
- 기존 씬 검색 경로 상수는 Project Settings의 **Scene Search Folders**로 이동했습니다.

## [1.0.0]

### Added

- Enter Play Mode Options 토글
- 첫 Build Settings 씬 실행 및 씬 선택 메뉴
- FMOD Debug Overlay 토글
- Localization Locale 선택 메뉴
- Unity 6용 Time Scale 슬라이더
- Unity 2022 및 Unity 6 Toolbar 지원
