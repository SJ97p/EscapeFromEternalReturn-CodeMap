# Improvement Notes

## Encoding Cleanup

일부 C# 주석이 깨져 있어 원본 의도를 코드만으로 따라가야 하는 구간이 있습니다. 포트폴리오 공개 전에는 파일 인코딩을 UTF-8로 통일하고, 깨진 주석은 삭제하거나 새 설명으로 교체하는 것이 좋습니다.

## Restricted Zone

금지구역 선택 로직은 현재 랜덤 후보 선택에 가깝습니다. 실제 게임 밸런스에서는 플레이어 위치, 남은 지역 수, 파밍 동선, 보스 지역 등을 가중치로 반영하는 정책 객체로 분리할 수 있습니다.

## Inventory / Storage

아이템 이동 규칙이 UI 드래그, 퀵무브, 장비 장착, 보관함 이동으로 늘어날수록 예외가 많아집니다. `MoveItemCommand` 같은 명령 객체로 이동 의도와 검증 결과를 기록하면 디버깅과 테스트가 쉬워집니다.

## Player Combat

FSM의 우선순위 규칙은 읽기 쉽지만, 상태 전이가 많아지면 전이표가 더 안전합니다. `CanTransition(from, to)` 규칙을 별도 테이블로 빼면 스턴/넉백/사망 같은 강제 상태를 더 명확하게 관리할 수 있습니다.

## Monster / Boss AI

보스 콤보 실행은 재귀적으로 하위 액션을 처리합니다. 패턴이 복잡해질 경우 실행 로그, 중단 조건, 페이즈 전환 조건을 포함한 `BossActionRunner`로 확장하는 것이 좋습니다.

## Data Layer

Repository 계층이 이미 분리되어 있으므로, 다음 단계는 에디터 검증 도구입니다. DB 테이블의 item id, spawn id, recipe id가 실제 ScriptableObject/Prefab 참조와 맞는지 빌드 전 검증하면 런타임 오류를 줄일 수 있습니다.

