# StorageRepository

## Role

SQLite에 저장된 Storage row와 런타임 저장 DTO인 `StorageData` 사이를 매핑합니다.

## Class Diagram

```mermaid
classDiagram
    class StorageRepository {
        -DBLoader _dbLoader
        -string _dbKey
        +StorageRepository(DBLoader dbLoader)
        +IEnumerable~StorageData~ GetAll()
        +StorageData GetById(int id)
        +StorageData MapFromRow(StorageItem row)
        +void Update(StorageData data)
        +void Delete(int data)
        +void DeleteAllBySaveId(int saveId)
        +void Add(StorageData data)
        +IEnumerable~StorageData~ GetBySaveId(int saveId)
    }

    class DBLoader {
        +Dictionary~string, SQLiteConnection~ dbConnections
        -string[] _cachedDbFiles
        +void ConnectSQLite()
        +SQLiteConnection GetConnection(string key)
    }

    class StorageData {
        +int SaveId
        +StorageType StorageType
        +int ItemId
        +int Quantity
        +int X
        +int Y
    }

    class StorageItem {
        +int SaveId
        +int StorageType
        +int ItemId
        +int Quantity
        +int X
        +int Y
    }

    StorageRepository --> DBLoader
    StorageRepository --> StorageData
    StorageRepository --> StorageItem : map row
```

## Design Point

Repository가 SQLite 접근을 감싸고, 런타임 시스템은 `StorageData` 단위로 저장/로드 흐름을 다룹니다.

## Source

- [StorageRepository.cs](../../src/Assets/00_Scripts/DataBase/StorageRepository.cs)

