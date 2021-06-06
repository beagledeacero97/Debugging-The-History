using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class cargarPregunta : MonoBehaviour
{
    private string conn;
    //private IDbConnection dbconn;
    //private IDbCommand dbcmd;
    //private IDataReader reader;
    string DatabaseName = "OtraBD.db";
    private SqliteConnection dbconn;
    private SqliteCommand dbcmd;
    private SqliteDataReader reader;

    public Text pregunta;
    public Button resp1;
    private bool b1Correcta;
    public Button resp2;
    private bool b2Correcta;
    public Button resp3;
    private bool b3Correcta;
    public Button resp4;
    private bool b4Correcta;

    private int nPreguntas = 1;

    public Animator playerAnimator;
    public Animator enemyAnimator;
    public Animator YWAnimator;
    public Animator GOAnimator;
    public Animator transicion;

    public Text playerHP;
    public Text playerATK;
    public Text enemyHP;
    public Text enemyATK;
    public Text Monedero;

    private int Cartera;

    public RawImage gameover;
    public RawImage youwin;
    public RawImage pantNegra;

    public AudioSource reproductorResp1;
    public AudioSource reproductorResp2;
    public AudioSource reproductorResp3;
    public AudioSource reproductorResp4;
    public AudioSource reproductorClara;
    public AudioSource reproductorMonedero;

    public AudioClip clipCorrecto;
    public AudioClip clipIncorrecto;
    public AudioClip clipDisparo;
    public AudioClip clipGolpe;
    public AudioClip clipMoneda;

    private string escenaActual = "";

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        youwin.enabled = false;
        gameover.enabled = false;

        StartCoroutine(PantallaNegroDesvelar());

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
                if(!File.Exists(Path.Combine(Application.persistentDataPath, DatabaseName)))
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
        string query = "SELECT * FROM PreguntasEgipto;";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            nPreguntas += 1;
        }
        reader.Close();

        query = "SELECT * FROM Escenas WHERE Completada = 'No';";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            escenaActual = reader[1].ToString();
            break;
        }
        reader.Close();

        // Se cargan las estadisticas del personaje
        this.CargarEstadisticasClara();

        // Llamada al metodo que se encarga de cargar las preguntas
        this.Cargar();
    }

    public void CargarEstadisticasClara()
    {
        dbcmd = dbconn.CreateCommand();
        string query = "select * from EstadisticasClara;";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            playerHP.text = reader[1].ToString();
            playerATK.text = reader[2].ToString();
            Cartera = int.Parse(reader[3].ToString());
        }
        reader.Close();

        Monedero.text = 0 + "";
    }

    //Metodo que carga las preguntas
    public void Cargar()
    {
        this.ResetearColor();
        //Se genera un numero aleatorio que servira para seleccionar una pregunta de la BD
        int idregunta = UnityEngine.Random.Range(1, nPreguntas);

        //Se generan numeros aleatorios que van del 2 al 5 inclusive para que, a la hora de cargar las 4 respuestas a la pregunta, estas se carguen en un boton aleatorio
        int n1 = UnityEngine.Random.Range(2, 6);
        int n2;
        int n3;
        int n4;

        do
        {
            n2 = UnityEngine.Random.Range(2, 6);
        } while (n1 == n2);

        do
        {
            n3 = UnityEngine.Random.Range(2, 6);
        } while (n1 == n3 || n2 == n3);

        do
        {
            n4 = UnityEngine.Random.Range(2, 6);
        } while (n1 == n4 || n2 == n4 || n3 == n4);

        //Se realiza la consulta a la BD y se carga la pregunta y sus respuestas
        string query = "select * from PreguntasEgipto where id = " + idregunta + ";";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            pregunta.text = pregunta.text + " Lector";
            pregunta.text = reader[1].ToString();
            resp1.GetComponentInChildren<Text>().text = reader[n1].ToString();
            if (n1 == 2)
            {
                b1Correcta = true;
                b2Correcta = false;
                b3Correcta = false;
                b4Correcta = false;
            }
            resp2.GetComponentInChildren<Text>().text = reader[n2].ToString();
            if (n2 == 2)
            {
                b1Correcta = false;
                b2Correcta = true;
                b3Correcta = false;
                b4Correcta = false;
            }
            resp3.GetComponentInChildren<Text>().text = reader[n3].ToString();
            if (n3 == 2)
            {
                b1Correcta = false;
                b2Correcta = false;
                b3Correcta = true;
                b4Correcta = false;
            }
            resp4.GetComponentInChildren<Text>().text = reader[n4].ToString();
            if (n4 == 2)
            {
                b1Correcta = false;
                b2Correcta = false;
                b3Correcta = false;
                b4Correcta = true;
            }
        }

        reader.Close();
    }

    //Metodo que comprueba si se ha acertado la pregunta
    public void ComprobarRespuesta(Button boton)
    {
        resp1.enabled = false;
        resp2.enabled = false;
        resp3.enabled = false;
        resp4.enabled = false;

        bool jugadorAcierta = false;
        //Se comprueba si el boton pulsado contiene la respuesta correcta o no, pintando de verde su fondo si se ha acertado la pregunta y de rojo si se ha fallado
        if (boton.name == "Respuesta 1")
        {
            if (b1Correcta == true)
            {
                reproductorResp1.PlayOneShot(clipCorrecto);
                boton.GetComponent<Image>().color = Color.green;
                jugadorAcierta = true;
            }
            else
            {
                reproductorResp1.PlayOneShot(clipIncorrecto);
                boton.GetComponent<Image>().color = Color.red;
                jugadorAcierta = false;
            }
        }
        if (boton.name == "Respuesta 2")
        {
            if (b2Correcta == true)
            {
                reproductorResp2.PlayOneShot(clipCorrecto);
                boton.GetComponent<Image>().color = Color.green;
                jugadorAcierta = true;
            }
            else
            {
                reproductorResp2.PlayOneShot(clipIncorrecto);
                boton.GetComponent<Image>().color = Color.red;
                jugadorAcierta = false;
            }
        }
        if (boton.name == "Respuesta 3")
        {
            if (b3Correcta == true)
            {
                reproductorResp3.PlayOneShot(clipCorrecto);
                boton.GetComponent<Image>().color = Color.green;
                jugadorAcierta = true;
            }
            else
            {
                reproductorResp3.PlayOneShot(clipIncorrecto);
                boton.GetComponent<Image>().color = Color.red;
                jugadorAcierta = false;
            }
        }
        if (boton.name == "Respuesta 4")
        {
            if (b4Correcta == true)
            {
                reproductorResp4.PlayOneShot(clipCorrecto);
                boton.GetComponent<Image>().color = Color.green;
                jugadorAcierta = true;
            }
            else
            {
                reproductorResp4.PlayOneShot(clipIncorrecto);
                boton.GetComponent<Image>().color = Color.red;
                jugadorAcierta = false;
            }
        }

        StartCoroutine(SiguientePregunta(jugadorAcierta));
    }

    // Este metodo cambia el color de fondo de los botones al color blanco
    public void ResetearColor()
    {
        resp1.GetComponent<Image>().color = Color.white;
        resp2.GetComponent<Image>().color = Color.white;
        resp3.GetComponent<Image>().color = Color.white;
        resp4.GetComponent<Image>().color = Color.white;
    }

    // Este codigo se lanza cuando se contesta a una pregunta y sirve para que la animación de acierto o error se realice antes de que la siguiente pregunta sea cargada
    IEnumerator SiguientePregunta(bool jugadorAcierta)
    {
        // Se recogen las estadisticas de los personajes
        int plyHP, eneHP, plyATK, eneATK;
        eneHP = int.Parse(enemyHP.text.ToString());
        plyATK = int.Parse(playerATK.text.ToString());
        plyHP = int.Parse(playerHP.text.ToString());
        eneATK = int.Parse(enemyATK.text.ToString());

        // Si el jugador acierta se lanza las animaciones pertinentes y se resta vida al enemigo
        if (jugadorAcierta == true)
        {
            playerAnimator.Play("PlayerFightTest");
            reproductorClara.PlayOneShot(clipDisparo);
            yield return new WaitForSeconds(0.70f);
            enemyAnimator.Play("EnemyHitTest");

            eneHP -= plyATK;

            if (eneHP > 0)
                enemyHP.text = eneHP + "";
            else
                enemyHP.text = "Muerto";
        }
        // Si el jugador falla, se lanzan las animaciones pertinentes y se le resta vida al jugador
        else
        {
            enemyAnimator.Play("EnemyFightTest");
            yield return new WaitForSeconds(0.50f);
            playerAnimator.Play("PlayerHitTest");
            reproductorClara.PlayOneShot(clipGolpe);

            plyHP -= eneATK;

            if (plyHP > 0)
                playerHP.text = plyHP + "";
            else
                playerHP.text = "Muerto";
        }

        // Se realiza una espera para dar tiempo a que terminen de realizarse las animaciones en curso
        yield return new WaitForSeconds(1.5f);

        // Si la salud del enemigo llega o baja de cero, se lanzan las animaciones pertinentes y se muestra la pantalla de victoria.
        if (eneHP <= 0)
        {
            enemyAnimator.Play("EnemyDeathTest");
            yield return new WaitForSeconds(1.20f);
            enemyAnimator.Play("EnemyIsDeathTest");
            youwin.enabled = true;

            YWAnimator.Play("YouWinMovimiento");
            yield return new WaitForSeconds(1.30f);
            youwin.rectTransform.anchoredPosition = new Vector3(0, 0, 0);

            this.GanarMonedas();

            string query = "UPDATE EstadisticasClara SET Monedero = '" + (Cartera + 3) + "' WHERE ID = '1' ";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();

            reader.Close();

            query = "UPDATE Escenas SET Completada = 'Si' WHERE Escena = '" + escenaActual + "'; ";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();

            reader.Close();
        }
        // Si es la vida del jugador la que llega a 0, se lanzan las animaciones pertinentes y se muestra la ventana de fin del juego
        else if (plyHP <= 0)
        {
            playerAnimator.Play("PlayerDeathTest");
            yield return new WaitForSeconds(1.20f);
            playerAnimator.Play("PlayerIsDeathTest");
            gameover.enabled = true;

            GOAnimator.Play("GameOverMovimiento");
            yield return new WaitForSeconds(1.30f);
            gameover.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
        }
        // Si ambos personajes aun tienen vida, se procede a cargar la siguiente pregunta
        else
        {
            this.Cargar();
            resp1.enabled = true;
            resp2.enabled = true;
            resp3.enabled = true;
            resp4.enabled = true;
        }
    }

    IEnumerator PantallaNegroDesvelar()
    {
        transicion.Play("DesvelarPantallaEgiptoMovimiento");
        yield return new WaitForSeconds(1.75f);
        pantNegra.enabled = false;
    }

    public void GanarMonedas()
    {
        StartCoroutine(Lapso(3, Monedero));
    }

    IEnumerator Lapso(int monedas, Text texto)
    {
        reproductorMonedero.PlayOneShot(clipMoneda);
        for (int c=1; c<=monedas; c++)
        {
            int m = int.Parse(Monedero.text);
            m += 1;
            Monedero.text = m + "";
            yield return new WaitForSeconds(0.4f);
        }
    }
}