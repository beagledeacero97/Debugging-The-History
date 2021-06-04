using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMP : MonoBehaviour
{
    public RawImage mapa;
    public GameObject mejoras;
    public RawImage cargaHUD;

    public RawImage loading;
    public RawImage indicador1;
    public RawImage indicador2;
    public RawImage barraDiario;
    public RawImage barraMejoras;
    public RawImage monederoMoneda;
    public RawImage costeMoneda;

    public Animator loadingAnimator;
    public Animator punteroAnimator;

    public Button viajar;
    public Button bMejoras;
    public Button bDiario;
    public Button bRealizarMejora;

    public Button masHP;
    public Button masATK;
    public Button menosHP;
    public Button menosATK;

    public Text contadorHP;
    public Text contadorATK;
    public Text estado;
    public Text tHP;
    public Text tATK;
    public Text tCoste;
    public Text contadorCoste;
    public Text contadorMonedero;
    public Text tBarraDiario;
    public Text tBarraMejoras;


    public SpriteRenderer claraImagen;

    public void Start()
    {
        // Invisibiliza todos los elementos relacionados con el panel de Mejora del Personaje.
        cargaHUD.enabled = false;
        visibilidadElementos(false);
    }

    // Tiene lugar al pulsar sobre el boton de cambio de pantalla entre mejoras y mapa.
    public void bMejClick()
    {
        // Si el mapa esta actualmente activo, se lanza la pantalla de carga y se cambia la visibilidad del mapa y sus elementos a false, a la vez
        // que se activan los elementos del panel de mejora y se hacen visibles al terminar la animaciíon de carga.
        if (mapa.enabled == true)
        {
            bMejoras.enabled = false;
            viajar.gameObject.SetActive(false);

            mejoras.transform.localPosition = new Vector3(0f, 2f, 0.0f);

            cargaHUD.enabled = true;
            loading.enabled = true;
            mapa.enabled = false;
            indicador1.enabled = false;
            indicador2.enabled = false;

            punteroAnimator.Play("PunteroDesactivado");

            tBarraDiario.enabled = false;
            barraDiario.enabled = false;
            bDiario.gameObject.SetActive(false);

            loadingAnimator.Play("CargaHUDMovimiento");
            StartCoroutine(HUDMovimiento("Mejorar"));
        }
        // Se lleva a cabo el proceso inverso al que realizaria el condicional anterior.
        else
        {
            bMejoras.enabled = false;

            mejoras.transform.localPosition = new Vector3(0f, -120f, 0.0f);

            cargaHUD.enabled = true;
            loading.enabled = true;
            mapa.enabled = true;
            indicador1.enabled = true;
            indicador2.enabled = true;

            punteroAnimator.Play("Puntero");

            loadingAnimator.Play("CargaHUDMovimiento");
            StartCoroutine(HUDMovimiento("Mapa"));
        }
    }

    // Corrutina que se lanza tras activar la pantalla de carga y que se encarga de cambiar la pantalla visible tras esperar a que la animación termine. 
    IEnumerator HUDMovimiento(string pantalla)
    {
        yield return new WaitForSeconds(1f);
        
        if(pantalla == "Mejorar")
        {
            visibilidadElementos(true);

            loadingAnimator.Play("CargaHUD");
            cargaHUD.enabled = false;
            tBarraMejoras.text = "Mapa";
        }
        else if (pantalla == "Mapa")
        {
            visibilidadElementos(false);

            loadingAnimator.Play("CargaHUD");
            cargaHUD.enabled = false;
            tBarraMejoras.text = "Mejoras";

            tBarraDiario.enabled = true;
            barraDiario.enabled = true;
            bDiario.gameObject.SetActive(true);
            viajar.gameObject.SetActive(true);
        }
        bMejoras.enabled = true;
        bMejoras.transform.Rotate(0f, 0f, 180f, Space.Self);
        barraMejoras.transform.Rotate(0f, 180f, 0f, Space.Self);
    }

    // Cambia la visibilidad de los elementos de la pantalla mejoras.
    public void visibilidadElementos(bool v)
    {
        if(v == true)
        {
            masHP.gameObject.SetActive(true);
            masATK.gameObject.SetActive(true);
            menosHP.gameObject.SetActive(true);
            menosATK.gameObject.SetActive(true);

            contadorHP.enabled = true;
            contadorATK.enabled = true;
            estado.enabled = true;
            tHP.enabled = true;
            tATK.enabled = true;
            tCoste.enabled = true;
            contadorCoste.enabled = true;
            contadorMonedero.enabled = true;

            monederoMoneda.enabled = true;
            costeMoneda.enabled = true;

            claraImagen.enabled = true;
        }
        else
        {
            masHP.gameObject.SetActive(false);
            masATK.gameObject.SetActive(false);
            menosHP.gameObject.SetActive(true);
            menosATK.gameObject.SetActive(true);

            contadorHP.enabled = false;
            contadorATK.enabled = false;
            estado.enabled = false;
            tHP.enabled = false;
            tATK.enabled = false;
            tCoste.enabled = false;
            contadorCoste.enabled = false;
            contadorMonedero.enabled = false;

            monederoMoneda.enabled = false;
            costeMoneda.enabled = false;

            claraImagen.enabled = false;
        }
    }
}
