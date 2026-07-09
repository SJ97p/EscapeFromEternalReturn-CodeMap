# NewUIManager

## Role

`UIPanelId` 기반으로 UI 패널을 등록하고 공통 Open/Close/Toggle을 처리하는 UI 레지스트리입니다.

## Responsibilities

- 씬 내 `UIPanel` 수집 또는 등록
- `UIPanelId`로 패널 조회
- 패널 열기/닫기/토글 공통 처리
- 버튼이나 시스템 코드가 패널 구체 타입을 직접 참조하지 않도록 분리

## Source

- [NewUIManager.cs](../../src/Assets/00_Scripts/Core/NewUIManager.cs)

## Related

- [Scene Lifecycle & UI Registry](../systems/scene-ui-lifecycle.md)

