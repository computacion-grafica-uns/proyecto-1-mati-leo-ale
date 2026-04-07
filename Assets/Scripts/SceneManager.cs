using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    private List<DatosObjeto> objetosACargar = new List<DatosObjeto>();    
    private List<GameObject> objetosInstanciados = new List<GameObject>();
    private GameObject camara;
    private Material material;

    void Start(){
        material = new Material(Shader.Find("MyShader"));
        
        CrearEscena();

        foreach (DatosObjeto datos in objetosACargar){
            InstanciarYConfigurarObjeto(datos);
        }

        CreateCamera();
    }
    
    private void CrearEscena(){
        objetosACargar.Add(new DatosObjeto {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared",
            posicion = new Vector3(0, -1.5f, 0),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(5, 1, 1),
            colorPrincipal = Color.blue
        });

        objetosACargar.Add(new DatosObjeto {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared2",
            posicion = new Vector3(0, 1.5f, 0),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(5, 1, 1),
            colorPrincipal = Color.yellow
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared2",
            posicion = new Vector3(0, 4.5f, 0),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(5, 1, 1),
            colorPrincipal = Color.blue
        });
    }

    private void CreateCamera(){
        camara = new GameObject("Camara");
        camara.AddComponent<Camera>();
        camara.transform.position = new Vector3(0,0,-5);
        camara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        camara.GetComponent<Camera>().backgroundColor = Color.black;
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

        Material materialUnico = new Material(material);
        Matrix4x4 matrizModelado = MVP.CreateModelMatrix(datos.posicion, datos.rotacion * Mathf.Deg2Rad, datos.escala);
        materialUnico.SetMatrix("_ModelMatrix", matrizModelado);
        nuevoObjeto.GetComponent<MeshRenderer>().material = materialUnico;

        objetosInstanciados.Add(nuevoObjeto);
    }

}
