using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Networking;

public class FaseActual : MonoBehaviour
{
    private string conn;
    private IDbConnection dbconn;
    private IDbCommand dbcmd;
    private IDataReader reader;
    string DatabaseName = "OtraBD.db";

    public Text fase;

    [System.Obsolete]
    void Start()
    {
        string filepath;

        if (Application.platform == RuntimePlatform.Android)
        {
            //Para cargar la BD en Android
            var loadingRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, DatabaseName));
            loadingRequest.SendWebRequest();
            while (!loadingRequest.isDone)
            {
                if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
                {
                    break;
                }
            }
            if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
            {

            }
            else
            {
                if (!File.Exists(Path.Combine(Application.persistentDataPath, DatabaseName)))
                {
                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, DatabaseName), loadingRequest.downloadHandler.data);
                }
            }

            filepath = Path.Combine(Application.persistentDataPath, DatabaseName);
            //Fin Parte de Android
        }
        else
        {
            //Para cargar la BD en el editor de UNITY
            filepath = "D:/UnityProjects/Debugging The History/Assets/StreamingAssets/" + DatabaseName;
        }

        //Abrir conexion con la BD
        conn = "URI=file:" + filepath;

        Debug.Log("Stablishing connection to: " + conn);
        dbconn = new SqliteConnection(conn);
        dbconn.Open();
        //Fin Abrir Conexion con la BD

        dbcmd = dbconn.CreateCommand();
        string query = "SELECT * FROM Escenas WHERE Completada = 'No' AND Conversacion = 'No';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        if(reader.Read())
        {
            fase.text = reader[1].ToString();
        }
        else
        {
            fase.text = "Egipto Infinito";
        }

        reader.Close();
        dbconn.Close();
    }
}
