using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suelo : MonoBehaviour {

    public Vidas vidas;

    private void OnTriggerEnter(Collider other)
    {
        var pelota = other.attachedRigidbody != null ? other.attachedRigidbody.GetComponent<movPelota>() : null;
        if (pelota != null && pelota.EnJuego) vidas.PerderVida();
    }
}
