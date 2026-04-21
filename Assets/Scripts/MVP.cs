using UnityEngine;
using System;

public static class MVP {
    public static Matrix4x4 CreateModelMatrix(Vector3 newPosition, Vector3 newRotation, Vector3 newScale){
        Matrix4x4 positionMatrix = new Matrix4x4(
            new Vector4(1f, 0f, 0f, newPosition.x),
            new Vector4(0f, 1f, 0f, newPosition.y),
            new Vector4(0f, 0f, 1f, newPosition.z),
            new Vector4(0f, 0f, 0f, 1f)
        );
        positionMatrix = positionMatrix.transpose;

        Matrix4x4 rotationX = new Matrix4x4(
            new Vector4(1f, 0f, 0f, 0f),
            new Vector4(0f, Mathf.Cos(newRotation.x), -Mathf.Sin(newRotation.x), 0f),
            new Vector4(0f, Mathf.Sin(newRotation.x), Mathf.Cos(newRotation.x), 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );

        Matrix4x4 rotationY = new Matrix4x4(
            new Vector4(Mathf.Cos(newRotation.y), 0f, Mathf.Sin(newRotation.y), 0f),
            new Vector4(0f, 1f, 0f, 0f),
            new Vector4(-Mathf.Sin(newRotation.y), 0f, Mathf.Cos(newRotation.y), 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );

        Matrix4x4 rotationZ = new Matrix4x4(
            new Vector4(Mathf.Cos(newRotation.z), -Mathf.Sin(newRotation.z), 0f, 0f),
            new Vector4(Mathf.Sin(newRotation.z), Mathf.Cos(newRotation.z), 0f, 0f),
            new Vector4(0f, 0f, 1f, 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );

        Matrix4x4 rotationMatrix = rotationZ * rotationY * rotationX;
        rotationMatrix = rotationMatrix.transpose;

        Matrix4x4 scaleMatrix = new Matrix4x4(
            new Vector4(newScale.x, 0f, 0f, 0f),
            new Vector4(0f, newScale.y, 0f, 0f),
            new Vector4(0f, 0f, newScale.z, 0f),
            new Vector4(0f, 0f, 0f, 1f)
        );
        scaleMatrix = scaleMatrix.transpose;

        Matrix4x4 finalMatrix = positionMatrix;
        finalMatrix *= rotationMatrix;
        finalMatrix *= scaleMatrix;

        return finalMatrix;
    }

    public static Matrix4x4 CreateViewMatrix(Vector3 position, Vector3 target, Vector3 up){
        Vector3 forward = (target - position).normalized;
        up = up.normalized;
        /* Regla de la mano derecha */
        Vector3 right = Vector3.Cross(up, forward).normalized;
        up = Vector3.Cross(forward, right).normalized;

        Matrix4x4 traslacionInversa = new Matrix4x4(
            new Vector4(1f, 0f, 0f, -position.x),
            new Vector4(0f, 1f, 0f, -position.y),
            new Vector4(0f, 0f, 1f, -position.z),
            new Vector4(0f, 0f, 0f, 1)
        );
        traslacionInversa = traslacionInversa.transpose;

        Matrix4x4 rotacionInversa = new Matrix4x4(
            new Vector4(right.x, right.y, right.z, 0),
            new Vector4(up.x, up.y, up.z, 0),
            new Vector4(-forward.x, -forward.y, -forward.z, 0),
            new Vector4(0f, 0f, 0f, 1f)
        );
        rotacionInversa = rotacionInversa.transpose;
        
        return rotacionInversa * traslacionInversa;
    }

    public static Matrix4x4 CreateProjectionMatrix(float fovGrados, float aspect, float near, float far){
        /*   
        * Tuvimos que modificar la matriz clásica de los libros para que 
        * funcione bien con el hardware actual y no se rompa la escena:
        * 1. Eje Y Invertido (-1): Unity usa el origen de la pantalla arriba 
        * a la izquierda (no abajo). Le agregamos el -1 al cálculo de Y para 
        * no ver el monoambiente patas arriba.
        * 2. Reversed-Z: La fórmula clásica calcula la profundidad de -1 a 1. 
        * DirectX la calcula al revés (de 1 a 0). Cambiamos la ecuación de la 
        * 3ra fila para adaptar el rango y evitar que las paredes del fondo 
        * desaparezcan de la nada (Z-Clipping).
        */

        // Convertimos el FOV a radianes
        float fovRadianes = fovGrados * (float)(Math.PI / 180.0);

        float tanHalfFov = (float)Math.Tan(fovRadianes / 2f);

        Matrix4x4 projectionMatrix = new Matrix4x4(
            new Vector4(1f / (aspect * tanHalfFov), 0f, 0f, 0f),
            new Vector4(0f, -1f / tanHalfFov, 0f, 0f),
            new Vector4(0f, 0f, near / (far - near), (near * far) / (far - near)),
            new Vector4(0f, 0f, -1f, 0f)
        );

        return projectionMatrix.transpose;
    }
}