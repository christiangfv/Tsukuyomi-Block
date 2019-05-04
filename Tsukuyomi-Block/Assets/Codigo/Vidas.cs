using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vidas : MonoBehaviour
{

    public static int vidas = 3;
    public Text textoVidas;
    public movPelota pelota;
    public MovBarra barra;

    // Use this for initialization
    void Start()
    {
        ActualizarMarcadorVidas();
    }

    void ActualizarMarcadorVidas()
    {
        textoVidas.text = "Vidas: " + Vidas.vidas;
    }

    public void PerderVida()
    {
        Vidas.vidas--;
        ActualizarMarcadorVidas();

        barra.Reset();
        pelota.Reset();
    }

}