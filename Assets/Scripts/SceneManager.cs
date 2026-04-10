using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    private List<DatosObjeto> objetosACargar = new List<DatosObjeto>();    
    private List<GameObject> objetosInstanciados = new List<GameObject>();
    private GameObject lienzo;
    private CamaraOrbital camaraOrbital;

    void Start(){
        CrearEscena();
        foreach (DatosObjeto datos in objetosACargar){
            InstanciarYConfigurarObjeto(datos);
        }
        CreateLienzo();
        camaraOrbital = new CamaraOrbital(Vector3.zero, 15f);
        ActualizarMatrices(0, 0);
    }

    void Update(){
        float deltaPhi = 0f;
        float deltaTheta = 0f;

        if (Input.GetKey(KeyCode.RightArrow)) deltaPhi = deltaPhi + 0.01f;
        if (Input.GetKey(KeyCode.LeftArrow))  deltaPhi = deltaPhi - 0.01f;

        if (Input.GetKey(KeyCode.UpArrow))    deltaTheta = deltaTheta - 0.01f; 
        if (Input.GetKey(KeyCode.DownArrow))  deltaTheta = deltaTheta + 0.01f;

        if (deltaPhi != 0 || deltaTheta != 0) ActualizarMatrices(deltaPhi, deltaTheta);
    }
    
    private void CrearEscena(){
        objetosACargar.Add(new DatosObjeto {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Puerta1",
            posicion = new Vector3(-0.3f, 1.5f, 0),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.2f, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_puerta2",
            posicion = new Vector3(0.2f, 2.5f, 0),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.8f, 1, 1),
            colorPrincipal = Color.grey
        });
     
        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Puerta3",
            posicion = new Vector3(2.1f, 1.5f, 0),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(3, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Vecino1",
            posicion = new Vector3(-0.5f, 1.5f, 4.9f),
            rotacion = new Vector3(0, 90, 0),
            escala = new Vector3(10, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Baño1",
            posicion = new Vector3(1.2f, 1.5f, 0.2f),
            rotacion = new Vector3(0, 90, 0),
            escala = new Vector3(0.2f, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Baño2",
            posicion = new Vector3(1.2f, 2.5f, 0.7f),
            rotacion = new Vector3(0, 90, 0),
            escala = new Vector3(0.8f, 1, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Baño3",
            posicion = new Vector3(1.2f, 1.5f, 1.85f),
            rotacion = new Vector3(0, 90, 0),
            escala = new Vector3(1.5f, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Cocina",
            posicion = new Vector3(2.45f, 1.5f, 2.5f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(2.3f, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Ventana1",
            posicion = new Vector3(-0.3f, 1.5f, 9.8f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.2f, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Ventana2",
            posicion = new Vector3(1.6f, 2.5f, 9.8f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(3.6f, 1, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Ventana3",
            posicion = new Vector3(3.5f, 1.5f, 9.8f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.2f, 3, 1),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Vecino2",
            posicion = new Vector3(3.7f, 1.5f, 4.9f),
            rotacion = new Vector3(0, 90, 0),
            escala = new Vector3(10, 3, 1),
            colorPrincipal = Color.grey
        });
    }

    private void CreateLienzo(){
        lienzo = new GameObject("Lienzo");
        lienzo.AddComponent<Camera>();
        lienzo.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        lienzo.GetComponent<Camera>().backgroundColor = Color.black;
    }

    private void InstanciarYConfigurarObjeto(DatosObjeto datos){
        CargadorObjetos parser = new CargadorObjetos();
        parser.ProcesarArchivo(datos.nombreArchivo);
        
        GameObject nuevoObjeto = new GameObject(datos.nombreGameObject);
        nuevoObjeto.AddComponent<MeshFilter>().mesh = new Mesh();
        nuevoObjeto.AddComponent<MeshRenderer>();
        
        Color[] coloresDeVertices = new Color[parser.vertices.Length];
        for (int i = 0; i < coloresDeVertices.Length; i++){
            coloresDeVertices[i] = datos.colorPrincipal; 
        }

        nuevoObjeto.GetComponent<MeshFilter>().mesh.vertices = parser.vertices;
        nuevoObjeto.GetComponent<MeshFilter>().mesh.triangles = parser.triangles;
        nuevoObjeto.GetComponent<MeshFilter>().mesh.colors = coloresDeVertices;
        nuevoObjeto.GetComponent<MeshFilter>().mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        Material materialUnico = new Material(Shader.Find("MyShader"));
        Matrix4x4 matrizModelado = MVP.CreateModelMatrix(datos.posicion, datos.rotacion * Mathf.Deg2Rad, datos.escala);
        materialUnico.SetMatrix("_ModelMatrix", matrizModelado);
        nuevoObjeto.GetComponent<MeshRenderer>().material = materialUnico;

        objetosInstanciados.Add(nuevoObjeto);
    }

    private void ActualizarMatrices(float deltaPhi, float deltaTheta){
        Matrix4x4 matrizVista = camaraOrbital.CalcularMatrizVista(deltaPhi, deltaTheta);
        foreach(GameObject o in objetosInstanciados){
            o.GetComponent<MeshRenderer>().material.SetMatrix("_ViewMatrix", matrizVista); 
        }
    }

}
