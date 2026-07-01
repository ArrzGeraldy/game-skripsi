using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public static class DB
{
    private static IDbConnection conn;

    public static void Init()
    {
        string path = "URI=file:" + Application.persistentDataPath + "/game.db";
        conn = new SqliteConnection(path);
        conn.Open();
    }

    public static IDbConnection Connection()
    {
        Debug.Log("DB connection request");
        return conn;
    }

    public static void Close()
    {
        conn?.Close();
        conn?.Dispose();
    }
}