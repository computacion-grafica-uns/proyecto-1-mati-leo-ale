using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using UnityEngine;

public class CargadorObjetos 
{
    public Vector3[] vertices;
    public int[] triangles;

    public void ProcesarArchivo(string fileName)
    {

        // Start is called before the first frame update
        string path = "Assets/Modelos3D/" + fileName + ".obj";

        //para ver si anda
        Debug.Log(path);
        Debug.Log(File.Exists(path));

        StreamReader reader = new StreamReader(path);
        string fileData = (reader.ReadToEnd());



        reader.Close();
        Debug.Log(fileData);


        List<Vector3> verticesLista = new List<Vector3>();
        List<int> carasLista = new List<int>();

        string[] lines = fileData.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("v "))
            {
                // 1. Limpiamos espacios extras y saltos de línea invisibles (\r)
                string lineaLimpia = lines[i].Trim();

                // 2. Spliteamos eliminando entradas vacías
                string[] partes = lineaLimpia.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                // Ahora partes[1] siempre será el primer número, sin importar cuántos espacios hubo
                float x = float.Parse(partes[1], CultureInfo.InvariantCulture);
                float y = float.Parse(partes[2], CultureInfo.InvariantCulture);
                float z = float.Parse(partes[3], CultureInfo.InvariantCulture);

                verticesLista.Add(new Vector3(x, y, z));
            }
            else if (lines[i].StartsWith("f "))
            {
                string[] partes = lines[i].Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                // Cantidad de índices en esta cara (sin contar la "f")
                int numVerticesEnCara = partes.Length - 1;

                if (numVerticesEnCara == 3)
                {
                    // CASO TRIÁNGULO: (Indices 1, 2, 3)
                    for (int j = 1; j <= 3; j++)
                    {
                        int indice = int.Parse(partes[j].Split('/')[0]) - 1;
                        carasLista.Add(indice);
                    }
                }
                else if (numVerticesEnCara == 4)
                {
                    // CASO QUAD: Lo dividimos en dos triángulos (1-2-3 y 1-3-4)
                    int v1 = int.Parse(partes[1].Split('/')[0]) - 1;
                    int v2 = int.Parse(partes[2].Split('/')[0]) - 1;
                    int v3 = int.Parse(partes[3].Split('/')[0]) - 1;
                    int v4 = int.Parse(partes[4].Split('/')[0]) - 1;

                    // Triángulo 1
                    carasLista.Add(v1);
                    carasLista.Add(v2);
                    carasLista.Add(v3);

                    // Triángulo 2
                    carasLista.Add(v1);
                    carasLista.Add(v3);
                    carasLista.Add(v4);
                }
            }
        }
        vertices = verticesLista.ToArray();
        triangles = carasLista.ToArray();

    }
    public Vector3 CalcularCentro() //para centarr la cama en el origen, calculo el centro con vertice mas lejano y mas cercano y promedio
    {
        Vector3 min = vertices[0];
        Vector3 max = vertices[0];

        for (int i = 1; i < vertices.Length; i++)
        {
            min = Vector3.Min(min, vertices[i]);
            max = Vector3.Max(max, vertices[i]);
        }
        return (min + max) / 2f;
    }


}