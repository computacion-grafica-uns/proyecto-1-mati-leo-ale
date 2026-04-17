using UnityEngine;

public class CamaraPrimeraPersona{
    public Vector3 posicion;
    private float anguloGiro;   

    public CamaraPrimeraPersona(Vector3 posicionInicial){
        this.posicion = posicionInicial;
        this.anguloGiro = 0f; 
    }

    public Matrix4x4 CalcularMatrizVista(float deltaGiro, float inputAvance, float inputLateral){
        anguloGiro += deltaGiro;

        Vector3 direccionCaminar = new Vector3(
            Mathf.Sin(anguloGiro),
            0f,
            Mathf.Cos(anguloGiro)
        ).normalized;

        // Producto cruz para sacar el vector lateral
        Vector3 derecha = Vector3.Cross(Vector3.up, direccionCaminar).normalized;

        posicion += direccionCaminar * inputAvance;
        posicion += derecha * inputLateral;
        posicion.y = 1.7f;

        Vector3 objetivo = posicion + direccionCaminar;

        return MVP.CreateViewMatrix(posicion, objetivo, Vector3.up);
    }
}