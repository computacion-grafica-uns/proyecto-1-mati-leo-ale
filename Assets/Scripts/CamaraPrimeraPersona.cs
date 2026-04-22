using UnityEngine;

public class CamaraPrimeraPersona{
    public Vector3 posicion;
    // Ángulos en radianes
    private float theta; // Movimiento vertical del mouse (Arriba/Abajo)
    private float phi;   // Movimiento horizontal del mouse (Izquierda/Derecha)

    public CamaraPrimeraPersona(Vector3 posicionInicial){
        this.posicion = posicionInicial;
        this.theta = Mathf.PI / 2f; // Arranca mirando al horizonte (90 grados)
        this.phi = 0f; 
    }

    public Matrix4x4 CalcularMatrizVista(float deltaXMouse, float deltaYMouse, float inputAvance, float inputLateral){
        phi -= deltaXMouse;
        theta -= deltaYMouse;
        // Clampeamos theta para no dar vueltas de campana ni quebrarnos el cuello
        theta = Mathf.Clamp(theta, 0.05f, Mathf.PI - 0.05f);

        Vector3 direccionMirada = new Vector3(
            Mathf.Sin(theta) * Mathf.Cos(phi),
            Mathf.Cos(theta),
            Mathf.Sin(theta) * Mathf.Sin(phi)
        ).normalized;

        Vector3 direccionCaminar = new Vector3(direccionMirada.x, 0f, direccionMirada.z).normalized;

        Vector3 derecha = Vector3.Cross(Vector3.up, direccionCaminar).normalized;

        posicion += direccionCaminar * inputAvance;
        posicion += derecha * inputLateral;
        posicion.y = 1.6f;

        Vector3 objetivo = posicion + direccionMirada;

        return MVP.CreateViewMatrix(posicion, objetivo, Vector3.up);
    }
}