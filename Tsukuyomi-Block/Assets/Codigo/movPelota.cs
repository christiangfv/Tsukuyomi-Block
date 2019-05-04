using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movPelota : MonoBehaviour {

    public float velocidadInicial = 500f;
    public Rigidbody rigBody;
    bool enJuego;
    Vector3 posicionInicial;
    public Transform barra;

    // Use this for initialization
    void Start () {
        posicionInicial = transform.position;
	}

    public void Reset()
    {
        transform.position = posicionInicial;
        transform.SetParent(barra);
        enJuego = false;
        DetenerMovimiento();
    }

    public void DetenerMovimiento()
    {
            rigBody.isKinematic = true;
            rigBody.velocity = Vector3.zero;
        }
        // Update is called once per frame
        void Update () {
        if (!enJuego && Input.GetButtonDown("Fire1"))
        {
            enJuego = true;
            transform.SetParent(null);
            rigBody.isKinematic = false;
            rigBody.AddForce(new Vector3(velocidadInicial, velocidadInicial, 0));
        }

	}
}
