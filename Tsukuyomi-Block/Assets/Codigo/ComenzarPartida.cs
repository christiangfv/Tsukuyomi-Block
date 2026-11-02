using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComenzarPartida : MonoBehaviour {
    void Start() { InterfazPartida.CrearMenu(); }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            Partida.ComenzarNuevaPartida();
        }
	}
}
