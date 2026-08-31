# 탐험에서 가져온 아이템을 다음 출발까지 남기는 저장 구조

## 저장 대상은 레시피가 아니라 플레이어가 정리한 전리품이었습니다

제작 레시피는 ScriptableObject로 관리했습니다. 반면 플레이어가 탈출 후 직접 정리한 인벤토리·창고·장비창 상태는 다음 실행에서도 그대로 남아야 했습니다. 런타임에서는 2차원 슬롯 배열이 편하지만, SQLite에는 어느 세이브 슬롯의 어느 저장공간에 어떤 아이템이 몇 개, 어느 좌표에 있는지가 필요합니다.

그래서 런타임 `Storage`를 SQLite row와 직접 묶지 않고, 저장 시점에만 `StorageData`로 바꾸기로 했습니다.

## 세 저장공간을 세이브 슬롯으로 내보내고 복원했습니다

`NewStorageManager`는 인벤토리(5×2), 창고(7×10), 장비창(5×1)을 각각 런타임 `Storage`로 관리합니다. 저장할 때는 비어 있지 않은 슬롯만 `StorageData`로 변환하고, 인벤토리·창고·장비창이라는 `StorageType`과 세이브 ID, 아이템 ID, 수량, X/Y 좌표를 함께 남깁니다.

`SaveManager`는 현재 세이브 슬롯의 기존 저장 데이터를 지운 뒤, 새로 모은 `StorageData`를 `StorageRepository`를 통해 SQLite에 다시 넣습니다. 불러올 때는 세이브 ID로 데이터를 가져와 `StorageType`별로 세 런타임 저장공간에 분배합니다.

```mermaid
flowchart LR
    Runtime[인벤토리 / 창고 / 장비창] --> Export[비어 있지 않은 슬롯만 Export]
    Export --> DTO[StorageData]
    DTO --> Repo[StorageRepository]
    Repo --> DB[(SQLite 세이브 슬롯)]
    DB --> Load[SaveId로 조회]
    Load --> Restore[StorageType별 런타임 Storage 복원]
```

## 코드에서 확인할 수 있는 지점

- `NewStorageManager`: 세 런타임 저장공간 생성과 저장/복원 연결
- `Storage.ExportToStorageData`: 슬롯 배열에서 저장 DTO 생성
- `SaveManager`: 현재 세이브 슬롯 저장·로드 흐름 관리
- `StorageRepository`: `StorageData`와 SQLite `StorageItem` row 변환
- `StorageRepository.GetBySaveId`: 세이브 슬롯 단위 조회

이렇게 레시피 데이터와 플레이어 보유 상태를 분리해 두었기 때문에, 제작 규칙은 저장 DB에 의존하지 않고도 플레이어가 가져온 전리품만 독립적으로 복원할 수 있습니다.
