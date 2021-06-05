using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Networking;

public class SiguienteEscenaRetardo : MonoBehaviour
{
    private string conn;
    private IDbConnection dbconn;
    private IDbCommand dbcmd;
    private IDataReader reader;
    string DatabaseName = "OtraBD.db";

    public RawImage transicion;
    public Animator animator;

    public AudioSource reproductor;
    public AudioClip clip;
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "TransicionViaje")
            StartCoroutine(PantallaNegroDesvelar());
        else
            StartCoroutine(PantallaNegroDesvelarMP());
    }
    public void CambiarEscenaRetardo(string escena)
    {
        StartCoroutine(Esperar(escena));
    }

    public void CambiarEscenaMenuPrincipal(string escena)
    {
        StartCoroutine(EsperarMP(escena));
    }

    IEnumerator Esperar(string escena)
    {
        yield return new WaitForSeconds(2.5f);
        animator.Play("PantallaNegraMovimiento");
        yield return new WaitForSeconds(2.2f);
        SceneManager.LoadScene(escena);
    }

    IEnumerator EsperarMP(string escena)
    {
        reproductor.PlayOneShot(clip);
        animator.Play("PantallaNegraMovimiento");
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene(escena);
    }

    [System.Obsolete]
    IEnumerator PantallaNegroDesvelar()
    {
        animator.Play("PantallaNegraDesvelar");

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
        string query = "SELECT * FROM Escenas WHERE Completada = 'No'";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        string escenaSiguiente = "Egipto 1-1";

        if (reader.Read())
        {
            escenaSiguiente = reader[1].ToString();
        }

        reader.Close();
        dbconn.Close();

        yield return new WaitForSeconds(1.75f);
        CambiarEscenaRetardo(escenaSiguiente);
    }

    IEnumerator PantallaNegroDesvelarMP()
    {
        animator.Play("PantallaNegraDesvelar");
        yield return new WaitForSeconds(1.75f);
    }
}
