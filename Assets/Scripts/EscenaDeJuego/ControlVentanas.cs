using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Networking;

public class ControlVentanas : MonoBehaviour
{
    private string conn;
    //private IDbConnection dbconn;
    //private IDbCommand dbcmd;
    //private IDataReader reader;
    string DatabaseName = "OtraBD.db";
    private SqliteConnection dbconn;
    private SqliteCommand dbcmd;
    private SqliteDataReader reader;

    public RawImage pantNegra;
    public Animator transicion;

    private string escenaActual = "";
    private string escenaSiguiente = "MenuPrincipal";

    [System.Obsolete]
    public void Start()
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

        string query = "SELECT * FROM Escenas WHERE Completada = 'No';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            escenaActual = reader[1].ToString();
            escenaSiguiente = reader[4].ToString();
            break;
        }
        reader.Close();
        dbconn.Close();
    }

    public void CambiarEscena(string escena)
    {
        SceneManager.LoadScene(escena);
    }

    public void YWContinuar()
    {
        StartCoroutine(PantallaNegroOcultar(escenaSiguiente));
    }

    public void GOVolver()
    {
        StartCoroutine(PantallaNegroOcultar("MenuPrincipal"));
    }
    public void GOReintentar()
    {
        if(escenaActual == "")
        {
            escenaActual = "Egipto 1-1";
        }
        StartCoroutine(PantallaNegroOcultar(escenaActual));
    }
    IEnumerator PantallaNegroOcultar(string escena)
    {
        pantNegra.enabled = true;
        transicion.Play("OcultarPantallaEgiptoMovimiento");
        yield return new WaitForSeconds(1.5f);
        CambiarEscena(escena);
    }
}
