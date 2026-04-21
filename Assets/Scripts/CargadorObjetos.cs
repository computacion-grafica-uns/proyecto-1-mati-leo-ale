using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class CargadorObjetos 
{
    public Vector3[] vertices;
    public int[] triangles;
    public Color[] colors;

    public void ProcesarArchivo(string fileName, Dictionary<string, Color> paleta, Color colorPorDefecto)
    {
        // Start is called before the first frame update
        string path = "Assets/Modelos3D/" + fileName + ".obj";

        //para ver si anda
        Debug.Log(path);
        Debug.Log(File.Exists(path));

        StreamReader reader = new StreamReader(path);
        string fileData = reader.ReadToEnd();

        reader.Close();
        Debug.Log(fileData);

        List<Vector3> verticesLista = new List<Vector3>();
        List<int> carasLista = new List<int>();

        // Lista paralela a los vértices para guardar el color de cada uno
        List<Color> coloresLista = new List<Color>();

        string materialActual = ""; // Guarda el estado del "pincel"

        string[] lines = fileData.Split('\n');
        
        for (int i = 0; i < lines.Length; i++)
        {
            // Limpiamos espacios extras y saltos de línea invisibles (\r)
            string lineaLimpia = lines[i].Trim();

            if (lineaLimpia.StartsWith("usemtl "))
            {
                // Cambiamos el pincel al nuevo material
                string[] partes = lineaLimpia.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length > 1) {
                    materialActual = partes[1];
                }
            }
            else if (lines[i].StartsWith("v "))
            {

                // 2. Spliteamos eliminando entradas vacías
                string[] partes = lineaLimpia.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                // Ahora partes[1] siempre será el primer número, sin importar cuántos espacios hubo
                float x = float.Parse(partes[1], CultureInfo.InvariantCulture);
                float y = float.Parse(partes[2], CultureInfo.InvariantCulture);
                float z = float.Parse(partes[3], CultureInfo.InvariantCulture);

                verticesLista.Add(new Vector3(x, y, z));

                // Inicializamos el vértice con el color por defecto
                coloresLista.Add(colorPorDefecto);
            }
            else if (lines[i].StartsWith("f "))
            {
                string[] partes = lineaLimpia.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                // Cantidad de índices en esta cara (sin contar la "f")
                int numVerticesEnCara = partes.Length - 1;

                // Averiguamos qué color tiene el pincel actual revisando el diccionario
                Color colorDeEstaCara = colorPorDefecto;
                if (paleta != null && paleta.ContainsKey(materialActual)) {
                    colorDeEstaCara = paleta[materialActual];
                }

                // Función local para no repetir código: extrae el índice y pinta el vértice
                void ProcesarYColorearVertice(string datoVertice) {
                    int indice = int.Parse(datoVertice.Split('/')[0]) - 1;
                    carasLista.Add(indice);
                    // Sobrescribimos el color de este vértice
                    coloresLista[indice] = colorDeEstaCara; 
                }

                if (numVerticesEnCara == 3)
                {
                    ProcesarYColorearVertice(partes[1]);
                    ProcesarYColorearVertice(partes[2]);
                    ProcesarYColorearVertice(partes[3]);
                }
                else if (numVerticesEnCara == 4)
                {
                    // Triángulo 1 del Quad
                    ProcesarYColorearVertice(partes[1]);
                    ProcesarYColorearVertice(partes[2]);
                    ProcesarYColorearVertice(partes[3]);

                    // Triángulo 2 del Quad
                    ProcesarYColorearVertice(partes[1]);
                    ProcesarYColorearVertice(partes[3]);
                    ProcesarYColorearVertice(partes[4]);
                }
            }
        }
        
        CentrarObjeto(verticesLista);
        
        vertices = verticesLista.ToArray();
        triangles = carasLista.ToArray();
        colors = coloresLista.ToArray();
    }

    private void CentrarObjeto(List<Vector3> verticesLista)
    {
        Vector3 min = verticesLista[0];
        Vector3 max = verticesLista[0];

        foreach (Vector3 v in verticesLista)
        {
            if (v.x < min.x) min.x = v.x;
            if (v.y < min.y) min.y = v.y;
            if (v.z < min.z) min.z = v.z;

            if (v.x > max.x) max.x = v.x;
            if (v.y > max.y) max.y = v.y;
            if (v.z > max.z) max.z = v.z;
        }

        Vector3 centro = new Vector3(
            (min.x + max.x) / 2f,
            (min.y + max.y) / 2f,
            (min.z + max.z) / 2f
        );

        for (int i = 0; i < verticesLista.Count; i++)
        {
            verticesLista[i] = verticesLista[i] - centro;
        }
    }
}