using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puntos : MonoBehaviour {

    public static int puntos = 0;
    public Text textoPuntos;

	// Use this for initialization
	void Start () {
		
	}

    void ActualizarMarcadorPuntos()
    {
        textoPuntos.text = "Puntos: " + Puntos.puntos;
    }
	// Update is called once per frame

    public void ganarPunto()
    {
        Puntos.puntos++;
        ActualizarMarcadorPuntos();
    }
}
