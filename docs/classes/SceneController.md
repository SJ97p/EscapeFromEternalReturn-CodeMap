# SceneController

## Role

모든 씬 컨트롤러가 따르는 공통 라이프사이클 기반 클래스입니다.

## Responsibilities

- 씬 진입/종료 흐름의 공통 규격 제공
- 씬별 컨트롤러가 같은 방식으로 초기화되도록 구조 통일
- `GameSceneManager`가 씬 전환 후 컨텍스트를 전달할 수 있는 기반 제공

## Source

- [SceneController.cs](../../src/Assets/00_Scripts/Core/SceneController.cs)

## Related

- [Scene Lifecycle & UI Registry](../systems/scene-ui-lifecycle.md)

