using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class Level
{
    public int id;
    public string name;
    public string scene_name;

    // ===== ORM STYLE METHODS =====

    public static List<Level> GetAll()
    {

        Debug.Log(DB.Connection() == null ? "DB NULL" : "DB OK");
        IDbCommand cmd = DB.Connection().CreateCommand();
        cmd.CommandText = "SELECT * FROM Level";
        List<Level> list = new List<Level>();

        IDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            list.Add(new Level
            {
                id = reader.GetInt32(0),
                name = reader.GetString(1),
                scene_name = reader.GetString(2)
            });
        }

        reader.Close();
        cmd.Dispose();

        return list;
    }

    public static Level Find(int id)
    {
        IDbCommand cmd = DB.Connection().CreateCommand();
        cmd.CommandText = "SELECT * FROM Level WHERE id=@id";

        var p = cmd.CreateParameter();
        p.ParameterName = "@id";
        p.Value = id;
        cmd.Parameters.Add(p);

        IDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            Level level = new Level
            {
                id = reader.GetInt32(0),
                name = reader.GetString(1),
                scene_name = reader.GetString(2)
            };

            reader.Close();
            return level;
        }

        reader.Close();
        return null;
    }
}