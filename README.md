# NKStudio Custom Toolbar

Unity Editor의 자주 사용하는 기능을 메인 툴바에 추가하는 모듈형 Editor 플러그인입니다.

Unity 2022.3 LTS부터 Unity 6.5까지 같은 설정과 수명주기 규칙을 사용합니다. Unity 6.3 이상에서는 공식 `MainToolbarElement` API를 사용하고, 이전 버전에서만 Legacy Host가 툴바 Zone 접근을 담당합니다.

## 제공 기능

- Enter Play Mode Options 토글
- 첫 번째 Build Settings 씬 실행
- 지정 폴더의 씬 선택 메뉴
- Time Scale 슬라이더와 Reset 메뉴
- FMOD Debug Overlay 토글
- Unity Localization Locale 선택 메뉴

FMOD와 Localization 모듈은 해당 패키지가 설치된 경우에만 컴파일됩니다. 공통 Toolbar 코어에는 두 패키지에 대한 직접 의존성이 없습니다.

## 요구 사항

- Unity 2022.3 이상
- FMOD Debug 기능: FMOD for Unity 2.2 이상, 3.0 미만
- Localization 기능: Unity Localization 1.0 이상, 2.0 미만

FMOD와 Localization은 선택 사항입니다.

## 설치

현재 프로젝트에서는 다음 위치에 포함되어 있습니다.

```text
Assets/Plugins/CustomToolbar
```

다른 프로젝트에서 사용할 때는 다음 중 하나를 선택합니다.

1. `CustomToolbar` 폴더 전체를 대상 프로젝트의 `Assets/Plugins` 아래에 복사합니다.
2. 이 폴더를 프로젝트 외부에 둔 뒤 Unity Package Manager의 **Install package from disk**에서 `package.json`을 선택합니다.

폴더 일부만 복사하면 아이콘 GUID와 선택적 asmdef 참조가 깨질 수 있으므로 반드시 전체 폴더를 복사해야 합니다.

## 설정

설정 화면은 다음 위치에 있습니다.

```text
Edit > Project Settings > NKStudio > Custom Toolbar
```

설정은 `ProjectSettings/CustomToolbarSettings.asset`에 프로젝트 단위로 저장됩니다.

- **Visible Modules**: 기능별 툴바 표시 여부
- **Scene Search Folders**: 씬 선택 메뉴가 검색할 `Assets` 내부 폴더
- **Minimum Time Scale**: Time Scale 최솟값
- **Maximum Time Scale**: Time Scale 최댓값
- **Reset Time Scale**: Reset 메뉴가 적용할 값

씬 검색 폴더에는 `Assets` 또는 그 하위의 실제 폴더만 사용할 수 있습니다. Time Scale 값은 `Minimum <= Reset <= Maximum` 범위로 자동 보정됩니다.

## Play Mode 정책

| 기능 | Edit Mode | Play Mode |
| --- | --- | --- |
| Enter Play Mode Options | 활성 | 비활성 |
| Scene Switcher | 활성 | 비활성 |
| FMOD Debug | 활성 | 비활성 |
| Localization | 활성 | 활성 |
| Time Scale | 활성 | 활성 |

## 모듈 확장

추가 기능은 `IToolbarModule`을 구현한 뒤 정적 초기화 지점에서 명시적으로 등록합니다. 리플렉션 기반 자동 탐색은 사용하지 않습니다.

```csharp
using UnityEditor;
using NKStudio.CustomToolbar;

[InitializeOnLoad]
internal sealed class ExampleToolbarModule : IToolbarModule
{
    private const string ModuleId = "Company.Toolbar.Example";

    static ExampleToolbarModule()
    {
        ToolbarModuleRegistry.Register(new ExampleToolbarModule());
    }

    public string Id => ModuleId;

    public void Refresh(ToolbarRefreshReason reason)
    {
        // 현재 Editor 상태를 읽어 자신의 Toolbar UI를 갱신합니다.
    }
}
```

Unity 2022.3~6.2에서도 UI를 제공해야 하는 모듈은 `ILegacyToolbarModule`을 함께 구현하고 `BuildLegacy(LegacyToolbarBuilder)`에서 원하는 Zone에 요소를 추가합니다. Unity 6.3 이상 UI는 기능 클래스의 정적 메서드에 `MainToolbarElement`를 선언하고, 상태 변경 시 `MainToolbar.Refresh(path)`를 사용합니다.

중복된 모듈 ID는 구성 오류로 처리됩니다. 모듈 내부에서 `playModeStateChanged`나 `sceneListChanged`를 다시 구독하지 말고 전달된 `ToolbarRefreshReason`을 사용해야 합니다.

## 구조

```text
Editor/
├── ToolbarRegister/   공통 Registry와 버전별 Host
├── Settings/          Project Settings와 저장 데이터
├── Modules/           기능별 Modern/Legacy 구현
└── Utility/           공통 Editor UI 유틸리티
Tests/Editor/           Registry와 Settings EditMode 테스트
```

## 검증 환경

- Unity 2022.3.62f3
- Unity 6000.0.23f1
- Unity 6000.3.10f1
- Unity 6000.5.5f1

Test Framework가 설치된 프로젝트에서는 `NKStudio.CustomToolbar.Editor.Tests` EditMode 테스트 어셈블리가 활성화됩니다.

## AI 스킬

Toolbar 확장 코드를 AI에게 작성시킬 때는 다음 스킬 문서를 제공할 수 있습니다.

```text
Packages/com.nkstudio.custom-toolbar/Skills/extend-custom-toolbar/SKILL.md
```

예시 요청:

```text
위 SKILL.md를 읽고 이 프로젝트에 새 Custom Toolbar 모듈을 추가해줘.
```

스킬은 `ProjectSettings/ProjectVersion.txt`를 먼저 확인하고 Unity 2022.3~6000.2 Legacy API 또는 Unity 6000.3+ Main Toolbar API 중 현재 프로젝트에 맞는 구현만 선택하도록 구성되어 있습니다. Codex 스킬로 자동 검색하려면 `extend-custom-toolbar` 폴더 전체를 사용하는 Codex의 skills 디렉터리에 복사해야 합니다.

## 변경 기록

버전별 변경 사항은 [CHANGELOG.md](CHANGELOG.md)를 참고하세요.
