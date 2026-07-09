using System.IO;
using SQLite;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace HBDinosaur_ER_Project.Database
{
    public class DBLoader : MonoBehaviour
    {
        public Dictionary<string, SQLiteConnection> dbConnections = new();
        private string[] _cachedDbFiles; // 경로 재탐색 방지를 위한 캐싱

        private string[] GetDatabaseFiles()
        {
            if (_cachedDbFiles != null && _cachedDbFiles.Length > 0)
                return _cachedDbFiles;

            string dbFolderPath = Path.Combine(Application.streamingAssetsPath, "DB");

            if (!Directory.Exists(dbFolderPath))
            {
                Debug.LogWarning("DB 폴더가 없습니다: " + dbFolderPath);
                return Array.Empty<string>();
            }

            _cachedDbFiles = Directory.GetFiles(dbFolderPath, "*.db", SearchOption.TopDirectoryOnly);

            foreach (string file in _cachedDbFiles)
            {
                Debug.Log("DB 파일 발견: " + file);
            }
            return _cachedDbFiles;
        }

        public void ConnectSQLite()
        {
            string[] dbFiles = GetDatabaseFiles();
            foreach (string file in dbFiles)
            {
                string key = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
                if (dbConnections.ContainsKey(key))
                {
                    // [보완] 이미 키가 있어도, 핸들이 깨졌거나 닫힌 상태라면 통로를 뚫어줍니다.
                    if (dbConnections[key] == null || dbConnections[key].Handle == IntPtr.Zero)
                    {
                        dbConnections[key] = new SQLiteConnection(file);
                        Debug.Log($"[DBLoader] 복구 - 끊어진 DB 연결 재활성화: {key}");
                    }
                    continue;
                }
                dbConnections[key] = new SQLiteConnection(file);
                Debug.Log($"DB 연결 완료: {key}");
            }
        }

        // ====================================================================
        // [핵심 수정] 저장소 레포지토리가 이 함수를 부를 때 닫혀있으면 자동으로 엽니다.
        // ====================================================================
        public SQLiteConnection GetConnection(string key)
        {
            key = key.ToLowerInvariant();

            if (dbConnections.TryGetValue(key, out var connection))
            {
                // 변수는 존재하지만 내부 SQLite 스트림 핸들이 닫힌(unopened) 상태인지 체크
                if (connection == null || connection.Handle == IntPtr.Zero)
                {
                    Debug.LogWarning($"[DBLoader] {key} DB 핸들이 닫혀있음을 감지! 강제 재연결을 시도합니다.");

                    // 안전하게 한 번 더 밀어버리고 다시 연결
                    connection?.Close();

                    string dbFolderPath = Path.Combine(Application.streamingAssetsPath, "DB");
                    string targetPath = Path.Combine(dbFolderPath, key + ".db");

                    if (File.Exists(targetPath))
                    {
                        connection = new SQLiteConnection(targetPath);
                        dbConnections[key] = connection; // 딕셔너리 갱신
                    }
                    else
                    {
                        Debug.LogError($"[DBLoader] 재연결 실패: {targetPath} 파일이 없습니다.");
                        return null;
                    }
                }
                return connection;
            }

            // 만약 딕셔너리에 아예 등록 조차 안 되어 있다면 전체 다시 연결 시도
            Debug.LogWarning($"해당 키의 DB 연결이 없어 ConnectSQLite를 재가동합니다: {key}");
            ConnectSQLite();

            if (dbConnections.TryGetValue(key, out var reconnected))
                return reconnected;

            return null;
        }

        private void OnDestroy()
        {
            // 진짜로 게임이 꺼지거나 오브젝트가 파괴될 때만 안전하게 해제
            foreach (var connection in dbConnections.Values)
            {
                connection?.Close();
            }

            dbConnections.Clear();
        }
    }
}