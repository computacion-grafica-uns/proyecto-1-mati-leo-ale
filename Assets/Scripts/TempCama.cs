using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;

public class TempCama : MonoBehaviour
{
    public CargadorObjetos fileReader;
    private GameObject miCamara;
    private GameObject cama;
    private Color[] colores;

    Vector3[] vertices;
    int[] triangulos;


    void Start()
    {
        fileReader=new CargadorObjetos();
        fileReader.ProcesarArchivo("cubo");
        vertices = fileReader.vertices;
        triangulos = fileReader.triangles;

        cama = new GameObject("Cubito");
        cama.AddComponent<MeshFilter>();
        cama.AddComponent<MeshRenderer>();
        cama.GetComponent<MeshFilter>().mesh = new Mesh();

        Matrix4x4 modelMatrix2 = Matrix4x4.identity;

        cama.GetComponent<Renderer>().material.SetMatrix("_ModelMatrix", modelMatrix2);

        CreateModel();
        UpdateMesh();
        CreateMaterial();
        CreateCamara();
    }



    private void CreateCamara()
    {
        miCamara = new GameObject("Camara");
        miCamara.AddComponent<Camera>();

        miCamara.transform.position = new Vector3(0, 40, 100);
        miCamara.transform.rotation = Quaternion.Euler(20, 180, 0);

        //decimos q el color de la camara sea un color solido
        miCamara.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        //establecemos color negro
        miCamara.GetComponent<Camera>().backgroundColor = Color.black;
    }

    private void CreateMaterial()
    {
        //creamos un nuevo material
        Material material = new Material(Shader.Find("ShaderBasico"));

        //asignamos el material al MeshRenderer del objetoCuadrado

        cama.GetComponent<MeshRenderer>().material = material;

    }

    private void CreateModel()
    {
        colores = new Color[vertices.Length];

        Vector3 centro = fileReader.CalcularCentro();

        for (int i = 0; i < vertices.Length; i++)
        {
            // Mapeamos las coordenadas X, Y, Z de la cama a valores entre 0 y 1
            // Basado en tu archivo .obj: X (40 a 60), Y (-1 a 10), Z (-45 a -15)
            vertices[i] = vertices[i] - centro; // traslado la malla
            float r = (vertices[i].x - 40f) / 20f; // Rojo según el ancho
            float g = (vertices[i].y + 1f) / 11f;  // Verde según la altura
            float b = (vertices[i].z + 45f) / 30f; // Azul según el largo

            colores[i] = new Color(r, g, b);
        }
    }

    // Update is called once per frame
    void UpdateMesh()
    {
        cama.GetComponent<MeshFilter>().mesh.vertices = vertices;
        cama.GetComponent<MeshFilter>().mesh.triangles = triangulos;
        cama.GetComponent<MeshFilter>().mesh.colors = colores;
    }
}

