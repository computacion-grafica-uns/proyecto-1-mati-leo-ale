using UnityEngine;

public class CamaraOrbital{
    private Vector3 objetivo;
    private float rho;       // Distancia al objetivo
    // Ángulos en radianes
    private float theta;     // Ángulo polar (desde el eje Y hacia abajo)
    private float phi;       // Ángulo azimutal (giro en el plano XZ)

    public CamaraOrbital(Vector3 objetivoInicial, float rhoInicial){
        this.objetivo = objetivoInicial;
        this.rho = rhoInicial;
        // Arrancamos con un ángulo intermedio
        this.theta = Mathf.PI / 4f; 
        this.phi = 0f; 
    }

    public Matrix4x4 CalcularMatrizVista(float deltaPhi, float deltaTheta){
        phi = phi + deltaPhi;
        theta = theta + deltaTheta; 
        // Protegemos theta para evitar el Gimbal Lock y no atravesar el piso
        float limiteMinimo = 0.05f;
        float limiteMaximo = (Mathf.PI / 2f) - 0.05f;
        theta = Mathf.Clamp(theta, limiteMinimo, limiteMaximo);
        // Usamos coordenadas esfericas
        float x = rho * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = rho * Mathf.Cos(theta);
        float z = rho * Mathf.Sin(theta) * Mathf.Sin(phi);

        Vector3 nuevaPosicion = new Vector3(
            objetivo.x + x, 
            objetivo.y + y, 
            objetivo.z + z
        );

        return MVP.CreateViewMatrix(nuevaPosicion, objetivo, Vector3.up);
    }
}