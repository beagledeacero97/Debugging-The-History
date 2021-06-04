using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovimientoBackground : MonoBehaviour
{
    public float velocidad = 0.02f;
    public RawImage fondo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float velocidadFinal = velocidad * Time.deltaTime;
        fondo.uvRect = new Rect(fondo.uvRect.x + velocidadFinal, 0f, 1f, 1f);
    }
}
