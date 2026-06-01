# Kipu Videogame - Hueso de Vaca

## 📋 Prerrequisitos del Sistema

Antes de clonar el proyecto, asegúrate de tener instalado lo siguiente:
* **Unity Hub** y el **Unity Editor** (versión recomendada: la utilizada para crear el proyecto, con soporte para módulos 2D).
* **Git** (si vas a clonar desde el repositorio).
* Una cuenta de **Itch.io** (para la publicación final).

### Dependencias y Paquetes de Unity
El proyecto hace uso de las siguientes librerías que deben estar presentes en el Package Manager:
* **Universal Render Pipeline (URP):** Configurado para 2D.
* **Post-Processing:** Esencial para los efectos de la maldición y viñetas.
* **Cinemachine:** Para el seguimiento físico de la cámara.
* **UI Toolkit:** Para la interfaz de diálogos interactivos y alertas.
* **Steamworks.NET / Steam Input API:** Requerido para la lectura del giroscopio del mando.

## 🚀 Guía de Instalación y Configuración

Sigue estos pasos para levantar el entorno de desarrollo localmente:

### Paso 1: Obtener el Código Fuente
**Opción A (Vía Git):**
Abre tu terminal y ejecuta:
`git clone https://github.com/SaraChiriboga/kipu-videogame.git`

**Opción B (Vía Drive/.ZIP):**
Descarga el archivo comprimido, extráelo en tu disco duro (asegúrate de que la ruta sea corta para evitar errores de Unity en Windows).

### Paso 2: Abrir en Unity
1. Abre **Unity Hub**.
2. Haz clic en **"Open"** (o "Add project from disk").
3. Navega hasta la carpeta raíz `kipu-videogame` y selecciónala.
4. Unity comenzará a importar los `Assets` y a resolver las dependencias. Esto puede tomar unos minutos la primera vez.

### Paso 3: Carga de la Escena Principal
Una vez que el editor esté abierto, el proyecto no cargará la escena automáticamente.
1. En la ventana **Project**, navega a la ruta: `Assets/Scenes/`.
2. Haz doble clic en el archivo **`HuesoDeVaca.unity`** para abrir el nivel principal.

### Paso 4: Verificación de Errores Comunes
* **Pantalla rosa o sin efectos:** Ve a `Edit > Project Settings > Graphics` y asegúrate de que el archivo del *Scriptable Render Pipeline* (URP 2D) esté asignado correctamente.
* **Faltan librerías:** Ve a `Window > Package Manager` y verifica que *Post Processing* y *Cinemachine* estén instalados desde el *Unity Registry*.
* **Errores de UI:** Verifica que el UI Document en el objeto `UIDocument` de la escena tenga asignado el archivo UXML correcto.

## 📦 Estructura Clave del Proyecto
* `/Assets/Scripts/`: Contiene la lógica principal, incluyendo `IntroCinematic.cs`y `PlayerMovement.cs`.
* `/Assets/`: Recursos, prefabs y scripts para la interfaz gráfica.
* `/Assets/Animations/`: Controladores de animación (ej. `AmadaAnimator.controller`, `DulceMariaAnimator.controller`).
* `/Assets/Scenes/`: Escenas del juego.

## 🔗 Publicación en Itch.io
https://sarilola.itch.io/kipu-hueso-de-vaca
