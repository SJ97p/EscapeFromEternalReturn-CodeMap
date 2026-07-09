# GameSceneManager

## Role

씬 전환과 씬 진입 컨텍스트 전달을 중앙에서 관리합니다.

## Responsibilities

- 씬 전환 요청 처리
- 다음 씬으로 전달할 컨텍스트 관리
- 씬 로드 후 해당 씬 컨트롤러 라이프사이클과 연결
- 씬 간 직접 참조를 줄이는 중간 지점 역할

## Source

- [GameSceneManager.cs](../../src/Assets/00_Scripts/Core/GameSceneManager.cs)

## Related

- [Scene Lifecycle & UI Registry](../systems/scene-ui-lifecycle.md)

