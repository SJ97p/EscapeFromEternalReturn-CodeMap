# StorageRepository

## Role

SQLite에 저장된 Storage row와 런타임 저장 DTO인 `StorageData` 사이를 매핑하는 Repository입니다.

## Responsibilities

- DB 연결을 `DBLoader`에서 받아 사용
- 전체 Storage 데이터 조회
- 세이브 슬롯별 Storage 데이터 조회
- 저장 전 기존 세이브 슬롯 데이터 삭제
- `StorageData`를 SQLite row로 변환해 저장
- SQLite row를 `StorageData`로 변환

## Source

- [StorageRepository.cs](../../src/Assets/00_Scripts/DataBase/StorageRepository.cs)

## Related

- [Storage Persistence](../systems/storage-persistence.md)

