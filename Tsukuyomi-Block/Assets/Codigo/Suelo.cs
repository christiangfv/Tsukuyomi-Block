using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suelo : MonoBehaviour {

    public Vidas vidas;

    private void OnTriggerEnter()
    {
        vidas.PerderVida();
    }
}
