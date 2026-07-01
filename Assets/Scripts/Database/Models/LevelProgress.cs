using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;

public class LevelProgress
{
    public int id;
    public int level_id;
    public int completed;
    public int high_score;

    public static LevelProgress GetByLevel(int levelId)
    {
        IDbCommand cmd = DB.Connection().CreateCommand();
        cmd.CommandText = "SELECT * FROM LevelProgress WHERE level_id=@id";

        var p = cmd.CreateParameter();
        p.ParameterName = "@id";
        p.Value = levelId;
        cmd.Parameters.Add(p);

        IDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            LevelProgress data = new LevelProgress
            {
                id = reader.GetInt32(0),
                level_id = reader.GetInt32(1),
                completed = reader.GetInt32(2),
                high_score = reader.GetInt32(3)
            };

            reader.Close();
            return data;
        }

        reader.Close();
        return null;
    }

    public static void UpdateScore(int levelId, int score)
    {
        Debug.Log("UPDATE SCORE: " + score);
        IDbCommand cmd = DB.Connection().CreateCommand();
        cmd.CommandText =
        @"UPDATE LevelProgress 
          SET high_score = CASE 
              WHEN @score > high_score THEN @score 
              ELSE high_score 
          END
          WHERE level_id=@id";

        var p1 = cmd.CreateParameter();
        p1.ParameterName = "@score";
        p1.Value = score;

        var p2 = cmd.CreateParameter();
        p2.ParameterName = "@id";
        p2.Value = levelId;

        cmd.Parameters.Add(p1);
        cmd.Parameters.Add(p2);

        cmd.ExecuteNonQuery();
    }
}