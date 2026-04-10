using UnityEngine;

public class CamaraPrimeraPersona{
    public Vector3 posicion;
    // Ángulos en radianes
    private float theta; 
    private float phi;   

    public CamaraPrimeraPersona(Vector3 posicionInicial){
        this.posicion = posicionInicial;
        this.theta = Mathf.PI / 2f; 
        this.phi = 0f; 
    }

    public Matrix4x4 CalcularMatrizVista(float deltaPhi, float deltaTheta, float inputAvance, float inputLateral){
        phi += deltaPhi;
        theta += deltaTheta;
        // Clampeamos theta para limitar el rango de movimiento vertical
        theta = Mathf.Clamp(theta, 0.05f, Mathf.PI - 0.05f);

        Vector3 direccionMirada = new Vector3(
            Mathf.Sin(theta) * Mathf.Cos(phi),
            Mathf.Cos(theta),
            Mathf.Sin(theta) * Mathf.Sin(phi)
        ).normalized;

        Vector3 direccionCaminar = new Vector3(direccionMirada.x, 0f, direccionMirada.z).normalized;

        Vector3 derecha = Vector3.Cross(direccionMirada, Vector3.up).normalized;

        posicion += direccionCaminar * inputAvance;
        posicion += derecha * inputLateral;
        posicion.y = 1.5f;

        Vector3 objetivo = posicion + direccionMirada;

        return MVP.CreateViewMatrix(posicion, objetivo, Vector3.up);
    }
}