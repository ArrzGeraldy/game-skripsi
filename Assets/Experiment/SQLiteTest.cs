using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mono.Data.Sqlite;
using System.Data;

public class SQLiteTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start sql");

        string path = "URI=file:" + Application.persistentDataPath + "/test.db";

        using(IDbConnection db = new SqliteConnection(path))
        {
            db.Open();
            Debug.Log("SQLITE CONNECTED OK!");
            IDbCommand cmd = db.CreateCommand();
            cmd.CommandText = "SELECT 1";
            var result = cmd.ExecuteScalar();
             Debug.Log("Query result: " + result);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
