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

    void Awake()
    {
        if (GetComponent<Partida>() == null) gameObject.AddComponent<Partida>();
    }

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
        if (Vidas.vidas <= 0 || (Partida.Actual != null && Partida.Actual.Finalizada)) return;
        Vidas.vidas = Mathf.Max(0, Vidas.vidas - 1);
        ActualizarMarcadorVidas();

        if (Vidas.vidas == 0) {
            Partida.Actual.Perder();
            return;
        }

        barra.Reset();
        pelota.Reset();
    }

}