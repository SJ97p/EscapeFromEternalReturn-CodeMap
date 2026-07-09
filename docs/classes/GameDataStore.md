# GameDataStore

## Role

런타임에서 자주 참조하는 ScriptableObject 데이터베이스와 공용 프리팹 참조를 제공하는 데이터 허브입니다.

## Responsibilities

- 몬스터/VFX/SFX/BGM/발소리/보이스 데이터베이스 참조 제공
- 아이템 컨테이너 프리팹 참조 제공
- Singleton 기반 접근점 제공
- DB Repository와 별도로 에디터 참조형 데이터를 관리

## Source

- [GameDataStore.cs](../../src/Assets/00_Scripts/DataBase/GameDataStore.cs)

## Related

- [Data Repository](../systems/data-repository.md)

