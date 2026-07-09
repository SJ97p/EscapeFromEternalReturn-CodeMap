# RestrictedZoneController

## Role

시간 흐름에 따라 경고 지역과 금지구역을 선택하고 상태 변경을 이벤트로 알리는 컨트롤러입니다.

## Responsibilities

- 금지구역 사이클 시작/중단
- 안전 시간과 경고 시간을 나누어 타이머 이벤트 발행
- 후보 지역 중 경고 지역 선택
- 경고 지역을 확정 금지구역으로 전환
- UI/Zone 시스템이 구독할 수 있는 상태 변경 이벤트 제공

## Source

- [RestrictedZoneController.cs](../../src/Assets/00_Scripts/ZoneControllers/RestrictedZoneController.cs)

## Related

- [Restricted Zone & Region](../systems/restricted-zone.md)

