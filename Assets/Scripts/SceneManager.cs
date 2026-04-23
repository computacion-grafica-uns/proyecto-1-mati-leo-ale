using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    private List<DatosObjeto> objetosACargar = new List<DatosObjeto>();    
    private List<GameObject> objetosInstanciados = new List<GameObject>();
    private List<GameObject> paredes = new List<GameObject>();
    private GameObject techo;
    private bool paredesVisibles = true;
    private bool techoVisible = true;
    private GameObject lienzo;
    private CamaraOrbital camaraOrbital;
    private CamaraPrimeraPersona camaraPrimeraPersona;
    private enum modoCamara {orbital, primeraPersona};
    private modoCamara camaraActiva = modoCamara.orbital;

    void Start(){
        CrearEscena();
        foreach (DatosObjeto datos in objetosACargar){
            InstanciarYConfigurarObjeto(datos);
        }
        CreateLienzo();
        camaraOrbital = new CamaraOrbital(new Vector3(1.6f, 0f, 4.9f), 12f);
        camaraPrimeraPersona = new CamaraPrimeraPersona(Vector3.zero);
        float aspect = (float)Screen.width / (float)Screen.height;
        Matrix4x4 matrizVistaInicial = camaraOrbital.CalcularMatrizVista(0, 0);
        Matrix4x4 matrizProyeccionInicial = MVP.CreateProjectionMatrix(60f, aspect, 0.1f, 1000f);
        ActualizarMatrices(matrizVistaInicial, matrizProyeccionInicial);
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.C)){
            if (camaraActiva == modoCamara.orbital) {
                camaraActiva = modoCamara.primeraPersona;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else {
                camaraActiva = modoCamara.orbital;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        float deltaPhi = 0f;
        float deltaTheta = 0f;
        float inputAvance = 0f;
        float inputLateral = 0f;

        // Flechas para rotar (solo lo usa la cámara Orbital)
        if (Input.GetKey(KeyCode.RightArrow)) deltaPhi += 0.02f;
        if (Input.GetKey(KeyCode.LeftArrow))  deltaPhi -= 0.02f;
        if (Input.GetKey(KeyCode.UpArrow))    deltaTheta -= 0.02f; 
        if (Input.GetKey(KeyCode.DownArrow))  deltaTheta += 0.02f;

        // WASD para moverse (solo lo usa la cámara Primera Persona)
        if (Input.GetKey(KeyCode.W)) inputAvance += 0.02f;
        if (Input.GetKey(KeyCode.S)) inputAvance -= 0.02f;
        if (Input.GetKey(KeyCode.D)) inputLateral += 0.02f;
        if (Input.GetKey(KeyCode.A)) inputLateral -= 0.02f;

        // Mouse para mirar (solo lo usa la cámara Primera Persona)
        float sensibilidadMouse = 0.02f;
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        Matrix4x4 nuevaMatrizVista = Matrix4x4.identity;
        bool huboMovimiento = false;

        if (camaraActiva == modoCamara.orbital){
            // A la orbital le pasamos las flechas del teclado
            if (deltaPhi != 0 || deltaTheta != 0){
                nuevaMatrizVista = camaraOrbital.CalcularMatrizVista(deltaPhi, deltaTheta);
                huboMovimiento = true;
            }
        }
        else if (camaraActiva == modoCamara.primeraPersona){
            // A la primera persona le pasamos el Mouse (rotación) y WASD (movimiento)
            if (mouseX != 0 || mouseY != 0 || inputAvance != 0 || inputLateral != 0){
                nuevaMatrizVista = camaraPrimeraPersona.CalcularMatrizVista(mouseX, mouseY, inputAvance, inputLateral);
                huboMovimiento = true;
            }
        }

        if (huboMovimiento){
            float aspect = (float)Screen.width / (float)Screen.height;
            Matrix4x4 nuevaMatrizProyeccion = MVP.CreateProjectionMatrix(60f, aspect, 0.1f, 1000f);
            ActualizarMatrices(nuevaMatrizVista, nuevaMatrizProyeccion);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            paredesVisibles = !paredesVisibles;
            foreach (GameObject pared in paredes)
            {
                pared.SetActive(paredesVisibles);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            techoVisible = !techoVisible;
            if (techo != null)
            {
                techo.SetActive(techoVisible);
            }
        }
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

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "techo",
            posicion = new Vector3(1.52f, 3f, 4.88f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(4.26f, 0.1f, 50f),
            colorPrincipal = Color.white
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "littleOne",
            nombreGameObject = "muebletv",
            posicion = new Vector3(3.45f, 0.19f, 6.2f), // Apoya en el suelo (Y = 0.3)
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.55f, 0.6f, 2.8f), 
            colorPrincipal = new Color(101f / 255f, 67f / 255f, 33f / 255f),
            paletaMateriales = new Dictionary<string, Color>() {
                {"littleOneTexture", new Color(80f / 255f, 50f / 255f, 20f / 255f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "tv",
            posicion = new Vector3(3.5f, 1f, 6.2f),
            rotacion = new Vector3(0, 90f, 0),
            escala = new Vector3(1.5f, 0.85f, 0.05f),
            colorPrincipal = Color.black
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "inodoro",
            nombreGameObject = "inodoro",
    
            posicion = new Vector3(1.736f, 0.388f, 2.00f),
            rotacion = new Vector3(0, 90f, 0),
            escala = new Vector3(0.64f, 0.5f, 0.55f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"toilet", Color.white},
                {"toilet2handle", new Color(140f/255f, 140f/255f, 140f/255f)}
            }
});

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "bidet",
            nombreGameObject = "Bidet_Realista",         
            posicion = new Vector3(2.5f, 0.2f, 2.1f),
            rotacion = new Vector3(0, 180, 0),
            escala = new Vector3(0.01f, 0.0075f, 0.01f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Rectangle02_1",new Color(1,1,1)},
                {"Cylinder02_2", new Color(140f/255f, 140f/255f, 140f/255f)}
                
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "shower1",
            nombreGameObject = "ducha",
            posicion = new Vector3(3.165f, 1.1f, 0.618f),
            rotacion = new Vector3(0, 180, 0),
            escala = new Vector3(1f, 1f, 1f),
            colorPrincipal = new Color(115f/255f, 115f/255f, 115f/255f),
            paletaMateriales = new Dictionary<string, Color>() {
                {"showerlighter", new Color(115f/255f, 115f/255f, 115f/255f)},
                {"showerdarker", new Color(0.8f, 0.8f, 0.8f)},
                {"showerBlack", Color.black},
                {"showerBlue", new Color(47f/255f, 171f/255f, 255f/255f)},
                {"showerdarker2", new Color(0.92f, 0.92f, 0.92f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "sink1",
            nombreGameObject = "lavamanos",
            posicion = new Vector3(1.77f, 0.557f, 0.434f),
            rotacion = new Vector3(0, -90, 0),
            escala = new Vector3(0.75f, 0.85f, 0.82f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                { "sinkdarker", new Color(0.8f, 0.8f, 0.8f) }, 
                { "sinklighter", Color.white },                
                { "sinkblack", Color.black }                   
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "mirror",
            nombreGameObject = "espejo",
            posicion = new Vector3(1.77f,1.686f, 0.117f),
            rotacion = new Vector3(0, -90, 0),
            escala = new Vector3(0.75f, 0.85f, 0.82f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                { "mirrorBlue", Color.blue},
                { "mirrorlighter", new Color(0.9f, 0.9f, 0.9f) }
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "mueble_baño",
            nombreGameObject = "mueblebanio",
            posicion = new Vector3(3.345f,0.861f, 2.143f),
            rotacion = new Vector3(0, -180, 0),
            escala = new Vector3(0.01125f, 0.01125f, 0.01125f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                { "Cylinder60_1", new Color(1,1,1) },
                { "Cylinder57_6", Color.black },
                { "Cylinder67_7", Color.yellow }
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "horno",
            nombreGameObject = "horno",
            posicion = new Vector3(1.57f, 0.378f, 2.92f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.01f, 0.01f, 0.012f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Line15_1", Color.black},
                {"Box11_3", new Color(225f/255f, 219f/255f, 219f/255f)},
                {"Line0911_5", new Color(191f/255f, 48f/255f, 48f/255f)},
                {"ChamferBox01_6", new Color(195f/255f, 188f/255f, 188f/255f)},
                {"Box13_8", Color.black},
                {"Box12_10", Color.black},
                {"ChamferCyl04_14", new Color(245f/255f, 170f/255f, 10f/255f)},
                {"Rectangle02_16", new Color(225f/255f, 219f/255f, 219f/255f)},
                {"Line22_20", Color.grey},
                {"Rectangle07_55", Color.gray}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "heladera",
            nombreGameObject = "heladera", 
            posicion = new Vector3(3.163f, 1f, 4.625f),
            rotacion = new Vector3(0, -90, 0),
            escala = new Vector3(0.3760522f, 0.2801863f, 0.3643649f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Material", new Color(1f,0f, 0f)},
                {"Aluminum", new Color(0f, 1f, 0f)}
            }
        });


        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "alacena",
            nombreGameObject = "alacena",
            posicion = new Vector3(3.21f, 2.027f, 2.781f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.01f, 0.01f, 0.011014f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Box17_1", new Color(1f, 0f, 0f)},
                {"Box172_2", new Color(0f, 1f, 0f)},
                {"Loft08_4", new Color(0f, 0f, 1f)},
                {"Box16_5", Color.yellow}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "alacena",
            nombreGameObject = "alacena",
            posicion = new Vector3(2.405f, 2.027f, 2.781f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.01f, 0.01f, 0.011014f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Box17_1", new Color(1f, 0f, 0f)},
                {"Box172_2", new Color(0f, 1f, 0f)},
                {"Loft08_4", new Color(0f, 0f, 1f)},
                {"Box16_5", Color.yellow}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "table",
            nombreGameObject = "mesa",
            posicion = new Vector3(1.1f, 0.297f, 6.15f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.6f, 0.6f, 0.6f),
            colorPrincipal = new Color(48/255f, 48/255f, 48/255f)
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "sink",
            nombreGameObject = "mesada_bacha",
            posicion = new Vector3(2.37f, 0.447f, 2.94f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.01f, 0.01f, 0.011f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Box27_3", new Color(225f/255f, 219f/255f, 219f/255f)}, 
                {"Line50_9", new Color(95f/255f, 93f/255f, 93f/255f)},//gris ocsuro 
                {"Rectangle10_12", new Color(199f/255f, 193f/255f, 193f/255f)},
                {"Box272_4", new Color(191f/255f, 48f/255f, 48f/255f)}, //rojo
                {"Loft10_1", Color.white}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "alacena_baja",
            nombreGameObject = "alacena_baja",
            posicion = new Vector3(3.2f, 0.38f, 2.94f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.01f, 0.01f, 0.011f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Loft16_1", new Color(225f/255f, 219f/255f, 219f/255f)}, 
                {"Box52_3", new Color(191f/255f, 48f/255f, 48f/255f)},
                {"Box522_4", new Color(199f/255f, 193f/255f, 193f/255f)},
                {"Line67_10", new Color(95f/255f, 93f/255f, 93f/255f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "cama",
            nombreGameObject = "cama",
            posicion = new Vector3(2.675f, 0.365f, 8.619f),
            rotacion = new Vector3(0, -90, 0),
            escala = new Vector3(0.0075605f, 0.012252f, 0.008f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"ChamferBox01_1", new Color(1f, 0f, 0f)},
                {"Box35_2", new Color(0f, 1f, 0f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "japanese_style_screen",
            nombreGameObject = "paredponja",
            posicion = new Vector3(2.522f, 0.9f, 7.519f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.01222f, 0.01f, 0.01f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"paper", new Color(1f, 0f, 0f)},
                {"wood", new Color(0f, 1f, 0f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "chair",
            nombreGameObject = "silla1",
            posicion = new Vector3(0.461f, 0.455f, 6.15f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(1f, 1f, 1f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"fusta_taula", new Color(1f, 0f, 0f)},
                {"coixi_cadira", new Color(0f, 1f, 0f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "chair",
            nombreGameObject = "silla2",
            posicion = new Vector3(1.693f, 0.455f, 6.15f),
            rotacion = new Vector3(0, 180, 0),
            escala = new Vector3(1f, 1f, 1f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"fusta_taula", new Color(1f, 0f, 0f)},
                {"coixi_cadira", new Color(0f, 1f, 0f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "chair",
            nombreGameObject = "silla3",
            posicion = new Vector3(1.1f, 0.455f, 6.706f),
            rotacion = new Vector3(0, 90, 0),
            escala = new Vector3(1f, 1f, 1f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"fusta_taula", new Color(1f, 0f, 0f)},
                {"coixi_cadira", new Color(0f, 1f, 0f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "chair",
            nombreGameObject = "silla4",
            posicion = new Vector3(1.1f, 0.455f, 5.564f),
            rotacion = new Vector3(0, -90, 0),
            escala = new Vector3(1f, 1f, 1f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"fusta_taula", new Color(1f, 0f, 0f)},
                {"coixi_cadira", new Color(0f, 1f, 0f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "armario",
            nombreGameObject = "armario",
            posicion = new Vector3(-0.106f, 0.977f, 8.757f),
            rotacion = new Vector3(0, 90, 0),
            escala = new Vector3(0.01547887f, 0.01f, 0.01f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"white", new Color(1f, 0f, 0f)},
                {"tan", new Color(0f, 1f, 0f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_Paredes",
            posicion = new Vector3(1.563f, 0.254f, 9.787f),
            rotacion = new Vector3(0, 0f, 0),
            escala = new Vector3(3.703922f, 0.5742043f, 1f),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_ventana",
            posicion = new Vector3(0.178f, 1.24f, 9.787f),
            rotacion = new Vector3(0, 0f, 0),
            escala = new Vector3(0.78751f, 1.504118f, 1f),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "pared_lisa",
            nombreGameObject = "Pared_ventana2",
            posicion = new Vector3(3.018f, 1.24f, 9.787f),
            rotacion = new Vector3(0, 0f, 0),
            escala = new Vector3(0.78751f, 1.504118f, 1f),
            colorPrincipal = Color.grey
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "ventana",
            nombreGameObject = "ventana",
            posicion = new Vector3(1.599f, 1.273f, 10.095f),
            rotacion = new Vector3(0, 0, 0),
            escala = new Vector3(0.08271708f, 0.097143f, 0.07326705f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"white", new Color(1f, 0f, 0f)},
                {"Glass", new Color(0f, 1f, 0f)},
                {"cold_steel", new Color(0f, 0f, 1f)}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "puertabanio",
            nombreGameObject = "puertabanio",
            posicion = new Vector3(1.19f, 0.972f, 0.693f),
            rotacion = new Vector3(0, -90, 0),
            escala = new Vector3(0.008265345f, 0.009452443f, 0.01134f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Door", new Color(1f, 0f, 0f)},
                {"Frame_A", new Color(0f, 1f, 0f)},
                {"Threshold", new Color(0f, 0f, 1f)},
                {"Handle", Color.yellow},
                {"Frame_B", Color.magenta}
            }
        });

        objetosACargar.Add(new DatosObjeto
        {
            nombreArchivo = "frontDoor2",
            nombreGameObject = "puerta",
            posicion = new Vector3(0.194f, 0.98f, 0f),
            rotacion = new Vector3(0, 180, 0),
            escala = new Vector3(0.01f, 0.009357493f, 0.01f),
            colorPrincipal = Color.white,
            paletaMateriales = new Dictionary<string, Color>() {
                {"Leaf_Side_B", new Color(1f, 0f, 0f)},
                {"Leaf_Side_A", new Color(0f, 1f, 0f)},
                {"Leaf_Frame", new Color(0f, 0f, 1f)},
                {"Door_Frame", Color.yellow},
                {"Pole_B", Color.magenta},
                {"Pole_A", Color.cyan},
                {"Hinge", Color.white},
                {"Handles",new  Color (1f, 1f, 0f)},
                {"Door_Frame_Head",new  Color(1f, 0f, 0.3f)}
            }
        });

        GenerarPisoFlotante();
    }

    private void CreateLienzo(){
        lienzo = new GameObject("Lienzo");
        lienzo.AddComponent<Camera>();
        lienzo.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        lienzo.GetComponent<Camera>().backgroundColor = Color.black;
    }

    private void InstanciarYConfigurarObjeto(DatosObjeto datos){
        CargadorObjetos parser = new CargadorObjetos();
        parser.ProcesarArchivo(datos.nombreArchivo, datos.paletaMateriales, datos.colorPrincipal);
        
        GameObject nuevoObjeto = new GameObject(datos.nombreGameObject);
        nuevoObjeto.AddComponent<MeshFilter>().mesh = new Mesh();
        nuevoObjeto.AddComponent<MeshRenderer>();

        nuevoObjeto.GetComponent<MeshFilter>().mesh.vertices = parser.vertices;
        nuevoObjeto.GetComponent<MeshFilter>().mesh.triangles = parser.triangles;
        nuevoObjeto.GetComponent<MeshFilter>().mesh.colors = parser.colors;
        nuevoObjeto.GetComponent<MeshFilter>().mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        Material materialUnico = new Material(Shader.Find("MyShader"));
        Matrix4x4 matrizModelado = MVP.CreateModelMatrix(datos.posicion, datos.rotacion * Mathf.Deg2Rad, datos.escala);
        materialUnico.SetMatrix("_ModelMatrix", matrizModelado);
        nuevoObjeto.GetComponent<MeshRenderer>().material = materialUnico;

        objetosInstanciados.Add(nuevoObjeto);

        if (datos.nombreGameObject.StartsWith("Pared_"))
        {
            paredes.Add(nuevoObjeto);
        }
        else if (datos.nombreGameObject == "techo")
        {
            techo = nuevoObjeto;
        }
    }

    private void ActualizarMatrices(Matrix4x4 matrizVista, Matrix4x4 matrizProyeccion){
        foreach(GameObject o in objetosInstanciados){
            o.GetComponent<MeshRenderer>().material.SetMatrix("_ViewMatrix", matrizVista);
            o.GetComponent<MeshRenderer>().material.SetMatrix("_ProjectionMatrix", matrizProyeccion);
        }
    }

    private void GenerarPisoFlotante(){
        Color maderaClara = new Color(193f / 255f, 154f / 255f, 107f / 255f);
        Color maderaOscura = new Color(140f / 255f, 100f / 255f, 60f / 255f);

        float anchoTablon = 0.2f;
        float largoTablon = 1.2f;
        float grosorTablon = 0.05f;

        // Límites del monoambiente
        float limiteMinX = -0.4f; // Pared izquierda
        float limiteMaxX = 3.8f;  // Pared derecha
        float limiteMinZ = 0.0f;  // Pared de la puerta (inicio)
        float limiteMaxZ = 9.8f;  // Pared de la ventana (fondo)

        float actualX = limiteMinX;
        int fila = 0;

        while (actualX < limiteMaxX){
            float actualZ = limiteMinZ;

            bool esFilaImpar = (fila % 2 != 0);
            float largoPrimerTablon = esFilaImpar ? (largoTablon / 2f) : largoTablon;

            int columna = 0;

            while (actualZ < limiteMaxZ){
                float largoTablonActual = (columna == 0) ? largoPrimerTablon : largoTablon;

                if (actualZ + largoTablonActual > limiteMaxZ){
                    largoTablonActual = limiteMaxZ - actualZ;
                }

                float centroZ = actualZ + (largoTablonActual / 2f);

                float variacion = UnityEngine.Random.Range(0f, 1f);
                Color colorUnico = Color.Lerp(maderaClara, maderaOscura, variacion);

                objetosACargar.Add(new DatosObjeto
                {
                    nombreArchivo = "cubo", 
                    nombreGameObject = $"Tablon_{fila}_{columna}",
                    posicion = new Vector3(actualX, -0.05f, centroZ),
                    rotacion = Vector3.zero,
                    escala = new Vector3(anchoTablon, grosorTablon, largoTablonActual),
                    colorPrincipal = colorUnico
                });

                actualZ += largoTablonActual;
                columna++;
            }

            actualX += anchoTablon;
            fila++;
        }
    }
}