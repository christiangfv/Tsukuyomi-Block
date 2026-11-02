using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComenzarPartida : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Vidas.vidas = 3;
            Puntos.puntos = 0;
            SceneManager.LoadScene("Nivel 01");
        }
	}
}
