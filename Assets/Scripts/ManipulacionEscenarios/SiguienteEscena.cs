using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Networking;

public class SiguienteEscena : MonoBehaviour
{
    private string conn;
    //private IDbConnection dbconn;
    //private IDbCommand dbcmd;
    //private IDataReader reader;
    string DatabaseName = "OtraBD.db";
    private SqliteConnection dbconn;
    private SqliteCommand dbcmd;
    private SqliteDataReader reader;

    bool partidaEmpezada = false;

    public AudioSource reproductor;
    public AudioClip clip;

    [System.Obsolete]
    public void Start()
    {
        string filepath;

        //Debug.Log(Application.persistentDataPath + "/" + DatabaseName);

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
        string query = "SELECT * FROM Escenas WHERE Escena = 'Despierta';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            if(reader[2].ToString() == "Si")
            {
                partidaEmpezada = true;
            }
        }

        reader.Close();
        dbconn.Close();
    }
    public void CambiarEscena()
    {
        StartCoroutine(botonAudio());
    }

    IEnumerator botonAudio()
    {
        reproductor.PlayOneShot(clip);
        yield return new WaitForSeconds(0.6f);

        if (partidaEmpezada == true)
        {
            SceneManager.LoadScene("MenuPrincipal");
        }
        else
        {
            SceneManager.LoadScene("CinematicaEgipto");
        }
    }
}
