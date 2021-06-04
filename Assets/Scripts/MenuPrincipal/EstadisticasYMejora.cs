using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class EstadisticasYMejora : MonoBehaviour
{
    private string conn;
    //private IDbConnection dbconn;
    //private IDbCommand dbcmd;
    //private IDataReader reader;
    private string DatabaseName = "OtraBD.db";
    private SqliteConnection dbconn;
    private SqliteCommand dbcmd;
    private SqliteDataReader reader;

    private int HP;
    private int ATK;
    private int HPInicio;
    private int ATKInicio;
    private int Monedero;
    private int Coste;
    private int CosteTotal = 0;
    private int ContadorMejoraHP;
    private int ContadorMejoraATK;

    public Text tHP;
    public Text tATK;
    public Text tMonedero;
    public Text tCoste;

    public Button masHP;
    public Button menosHP;
    public Button masATK;
    public Button menosATK;
    public Button realizarMejora;

    // Start is called before the first frame update
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
        string query = "select * from EstadisticasClara;";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            HP = int.Parse(reader[1].ToString());
            ATK = int.Parse(reader[2].ToString());
            Monedero = int.Parse(reader[3].ToString());
            Coste = int.Parse(reader[4].ToString());
            ContadorMejoraHP = int.Parse(reader[5].ToString());
            ContadorMejoraATK = int.Parse(reader[6].ToString());
        }
        reader.Close();

        tHP.text = HP + "";
        tATK.text = ATK + "";
        tMonedero.text = Monedero + "";
        tCoste.text = 0 + "";

        HPInicio = HP;
        ATKInicio = ATK;
    }
    
    public void AumentarHP()
    {
        HP += ContadorMejoraHP;
        tHP.text = HP + "";

        CosteTotal = CosteTotal + Coste;
        tCoste.text = CosteTotal + "";
        Coste = Coste + 1;

        if (CosteTotal > Monedero)
        {
            tCoste.color = Color.red;
        }
    }

    public void DisminuirHP()
    {
        if(HP>HPInicio)
        {
            HP -= ContadorMejoraHP;
            tHP.text = HP + "";

            Coste = Coste - 1;
            CosteTotal = CosteTotal - Coste;
            tCoste.text = CosteTotal + "";
        }

        if (CosteTotal <= Monedero)
        {
            tCoste.color = Color.white;
        }
    }

    public void AumentarATK()
    {
        ATK += ContadorMejoraATK;
        tATK.text = ATK + "";

        CosteTotal = CosteTotal + Coste;
        tCoste.text = CosteTotal + "";
        Coste = Coste + 1;

            if (CosteTotal > Monedero)
            {
            tCoste.color = Color.red;
            }
        }

    public void DisminuirATK()
    {
        if (ATK > ATKInicio)
        {
            ATK -= ContadorMejoraATK;
            tATK.text = ATK + "";

            Coste = Coste - 1;
            CosteTotal = CosteTotal - Coste;
            tCoste.text = CosteTotal + "";

            if (CosteTotal <= Monedero)
            {
                tCoste.color = Color.white;
            }
        }
    }

    public void Mejorar()
    {
        if(CosteTotal<=Monedero)
        {
            Monedero = Monedero - CosteTotal;

            string query = "UPDATE EstadisticasClara SET HP = '" + HP + "', ATK = '" + ATK + "', Monedero = '" + Monedero + "', CosteMejora = '" + Coste + "' WHERE ID = '1' ";
            dbcmd.CommandText = query;
            reader = dbcmd.ExecuteReader();
            reader.Close();

            HPInicio = HP;
            ATKInicio = ATK;
            CosteTotal = 0;
            tCoste.text = 0 + "";
            tMonedero.text = Monedero + "";
        }

        
    }
}
