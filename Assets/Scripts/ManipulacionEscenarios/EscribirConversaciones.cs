using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Networking;

public class EscribirConversaciones : MonoBehaviour
{
    private string conn;
    //private IDbConnection dbconn;
    //private IDbCommand dbcmd;
    //private IDataReader reader;
    string DatabaseName = "OtraBD.db";
    private SqliteConnection dbconn;
    private SqliteCommand dbcmd;
    private SqliteDataReader reader;

    public Button boton;
    public Text textoBoton;
    public Text cuadro;
    public Text personaje;

    private bool espera = false;
    private List<string> frases = new List<string>();
    private List<string> narrador = new List<string>();
    private string escenaActual;
    private string escenaSiguiente;

    private int c = 0;

    [System.Obsolete]
    private void Start()
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
        string query = "SELECT * FROM Escenas WHERE CargaDesdeDiario = 'Si';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        if(reader.Read())
        {
            escenaActual = reader[1].ToString();
            escenaSiguiente = "MenuPrincipal";
            reader.Close();

            query = "SELECT * FROM Conversaciones WHERE Escena = '" + escenaActual + "';";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                frases.Add(reader[2].ToString());
                narrador.Add(reader[3].ToString());
            }
            reader.Close();

            boton.enabled = false;

            query = "UPDATE Escenas SET CargaDesdeDiario = 'No' WHERE Escena = '" + escenaActual + "';";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();
            reader.Close();
        }
        else
        {
            reader.Close();

            query = "SELECT * FROM Escenas WHERE Completada = 'No' AND Conversacion = 'Si';";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();

            reader.Read();
            escenaActual = reader[1].ToString();
            escenaSiguiente = reader[4].ToString();
            reader.Close();

            query = "SELECT * FROM Conversaciones WHERE Escena = '" + escenaActual + "';";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                frases.Add(reader[2].ToString());
                narrador.Add(reader[3].ToString());
            }
            reader.Close();

            boton.enabled = false;

            query = "UPDATE Escenas SET Completada = 'Si' WHERE Escena = '" + escenaActual + "';";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();
            reader.Close();
        }

        dbconn.Close();

        this.escribir();
        StartCoroutine(parpadeo());
    }

    public void escribir()
    {
        boton.enabled = true;
        espera = true;

        if(c<frases.Count)
        {
            personaje.text = narrador[c];
            cuadro.text = frases[c];
            c += 1;
        }
        else
        {
            CambiarEscena(escenaSiguiente);
        }
    }

    IEnumerator parpadeo()
    {
        while(espera == true)
        {
            if(textoBoton.text == "")
            {
                yield return new WaitForSeconds(0.75f);
                textoBoton.text = ">";
            }
            else
            {
                yield return new WaitForSeconds(0.75f);
                textoBoton.text = "";
            }
        }
        textoBoton.text = "";
    }

    public void CambiarEscena(string escena)
    {
        SceneManager.LoadScene(escena);
    }
}
