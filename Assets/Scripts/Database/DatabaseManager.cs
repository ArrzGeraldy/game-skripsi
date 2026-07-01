using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;


public class DatabaseManager : MonoBehaviour
{
    private string dbPath;
    private IDbConnection db;

    void Awake()
    {
        dbPath = "URI=file:" + Application.persistentDataPath + "/game.db";
        db = new SqliteConnection(dbPath);
        db.Open();

        Debug.Log("DB Opened");

        CreateTables();
        SeedLevel();
        SeedProgress();
    }

    void CreateTables()
    {
        IDbCommand cmd = db.CreateCommand();

        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Level (
            id INTEGER PRIMARY KEY,
            name TEXT,
            scene_name TEXT
        );

        CREATE TABLE IF NOT EXISTS LevelProgress (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            level_id INTEGER,
            completed INTEGER,
            high_score INTEGER
        );
        ";

        cmd.ExecuteNonQuery();
        cmd.Dispose();
    }

    void SeedLevel()
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM Level";

        int count = System.Convert.ToInt32(cmd.ExecuteScalar());

        if (count == 0)
        {
            InsertLevel(1, "Gerak Lurus Beraturan (GLB)", "Level_GLB");
            InsertLevel(2, "Gerak Lurus Berubah Beraturan (GLBB)", "Level_GLBB");
            InsertLevel(3, "Gerak Parabola", "Level_1_v2");

            Debug.Log("Level seeded");
        }
    }

    void InsertLevel(int id, string name, string scene)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "INSERT INTO Level (id, name, scene_name) VALUES (@id, @name, @scene)";

        var p1 = cmd.CreateParameter();
        p1.ParameterName = "@id";
        p1.Value = id;

        var p2 = cmd.CreateParameter();
        p2.ParameterName = "@name";
        p2.Value = name;

        var p3 = cmd.CreateParameter();
        p3.ParameterName = "@scene";
        p3.Value = scene;

        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);
        cmd.Parameters.Add(p3);

        cmd.ExecuteNonQuery();
    }

    void SeedProgress()
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM LevelProgress";

        int count = System.Convert.ToInt32(cmd.ExecuteScalar());

        if (count == 0)
        {
            for (int i = 1; i <= 3; i++)
            {
                InsertProgress(i);
            }

            Debug.Log("Progress seeded");
        }
    }

    void InsertProgress(int levelId)
    {
        IDbCommand cmd = db.CreateCommand();
        cmd.CommandText =
        "INSERT INTO LevelProgress (level_id, completed, high_score) VALUES (@id, 0, 0)";

        var p = cmd.CreateParameter();
        p.ParameterName = "@id";
        p.Value = levelId;

        cmd.Parameters.Add(p);

        cmd.ExecuteNonQuery();
    }
}
