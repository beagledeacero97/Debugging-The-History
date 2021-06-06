using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using UnityEngine.Networking;

public class ControlCB : MonoBehaviour
{
    private string conn;
    private IDbConnection dbconn;
    private IDbCommand dbcmd;
    private IDataReader reader;
    string DatabaseName = "OtraBD.db";
    
    public GameObject diario;
    
    public RawImage mapa;
    public RawImage cargaHUD;
    public RawImage loading;
    public RawImage indicador1;
    public RawImage indicador2;
    public RawImage barraDiario;
    public RawImage barraMejora;

    public Button bVisualizar;
    public Button bRealizarMejora;
    public Button bMejora;
    public Button bDiario;
    public Button viajar;

    public Text tBarraDiario;
    public Text tBarraMejoras;

    public Animator loadingAnimator;
    public Animator punteroAnimator;

    public Dropdown listaConver;

    private List<string> conversaciones = new List<string>();

    public AudioSource reproductor;
    public AudioClip clip;

    public AudioSource reproductor2;
    public AudioClip clip2;

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
        string query = "SELECT * FROM Escenas WHERE Completada = 'Si' AND Conversacion = 'Si';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        while(reader.Read())
        {
            conversaciones.Add(reader[1].ToString());
        }

        reader.Close();
        dbconn.Close();

        listaConver.ClearOptions();
        listaConver.AddOptions(conversaciones);
    }

    // Alterna la pantalla central del menu principal entre el mapa y el diario.
    public void bDiarioClick()
    {
        reproductor.PlayOneShot(clip);
        // Si el mapa esta actualmente activo, se lanza la pantalla de carga y se cambia la visibilidad del mapa y sus elementos a false, a la vez
        // que se activan los elementos del panel de mejora y se hacen visibles al terminar la animaciíon de carga.
        if (mapa.enabled == true)
        {
            bDiario.enabled = false;
            viajar.gameObject.SetActive(false);
            bRealizarMejora.gameObject.SetActive(false);

            diario.transform.localPosition = new Vector3(0f, 2f, 0.0f);

            cargaHUD.enabled = true;
            loading.enabled = true;
            mapa.enabled = false;
            indicador1.enabled = false;
            indicador2.enabled = false;

            punteroAnimator.Play("PunteroDesactivado");

            tBarraMejoras.enabled = false;
            barraMejora.enabled = false;
            bMejora.gameObject.SetActive(false);

            loadingAnimator.Play("CargaHUDMovimiento");
            StartCoroutine(HUDMovimiento("Diario"));
        }

        // Se lleva a cabo el proceso inverso al que realizaria el condicional anterior.
        else
        {
            bDiario.enabled = false;

            diario.transform.localPosition = new Vector3(0f, -120f, 0.0f);

            cargaHUD.enabled = true;
            loading.enabled = true;
            mapa.enabled = true;
            indicador1.enabled = true;
            indicador2.enabled = true;

            punteroAnimator.Play("Puntero");

            tBarraMejoras.enabled = true;
            barraMejora.enabled = true;
            bMejora.gameObject.SetActive(true);

            bRealizarMejora.gameObject.SetActive(true);
            viajar.gameObject.SetActive(true);

            loadingAnimator.Play("CargaHUDMovimiento");
            StartCoroutine(HUDMovimiento("Mapa"));
        }
    }

    // Corrutina que se lanza tras activar la pantalla de carga y que se encarga de cambiar la pantalla visible tras esperar a que la animación termine. 
    IEnumerator HUDMovimiento(string pantalla)
    {
        yield return new WaitForSeconds(1f);

        if (pantalla == "Diario")
        {
            visibilidadElementos(true);

            loadingAnimator.Play("CargaHUD");
            cargaHUD.enabled = false;
            tBarraDiario.text = "Mapa";
        }
        else if (pantalla == "Mapa")
        {
            visibilidadElementos(false);

            loadingAnimator.Play("CargaHUD");
            cargaHUD.enabled = false;
            tBarraDiario.text = "Cuaderno de Bitacora";

            tBarraDiario.enabled = true;
            barraDiario.enabled = true;
            bDiario.gameObject.SetActive(true);
            viajar.gameObject.SetActive(true);
        }

        bDiario.enabled = true;
        bDiario.transform.Rotate(0f, 0f, 180f, Space.Self);
        barraDiario.transform.Rotate(0f, 180f, 0f, Space.Self);
    }

    public void visibilidadElementos(bool v)
    {
        if (v == true)
        {
            listaConver.gameObject.SetActive(true);

        }
        else
        {
            listaConver.gameObject.SetActive(false);

        }
    }

    public void reproducir()
    {
        reproductor2.PlayOneShot(clip2);

        string escena = listaConver.options[listaConver.value].text.ToString();

        dbconn = new SqliteConnection(conn);
        dbconn.Open();

        dbcmd = dbconn.CreateCommand();
        string query = "UPDATE Escenas SET CargaDesdeDiario = 'Si' WHERE Escena = '" + escena +"';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        reader.Close();
        dbconn.Close();

        if(escena == "Despierta")
        {
            SceneManager.LoadScene("CinematicaNave");
        }
        else
        {
            SceneManager.LoadScene("CinematicaEgipto");
        }
    }
}
