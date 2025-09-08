Procedural Dungeon & Terrain Generator
🎮 Descripción del Proyecto

Este proyecto implementa Generación Procedural de Contenido (PCG) en Unity, combinando tres técnicas vistas en clases:

Algoritmo Constructivo → generación de salas y pasillos para formar mazmorras.

Algoritmo Fractal (Diamond-Square) → creación de terrenos montañosos naturales.

Algoritmo de Gramática (L-System) → generación de árboles con estructuras ramificadas.

El resultado es un mundo procedural que puede regenerarse dinámicamente y adaptarse a diferentes contextos.

🛠️ Técnicas implementadas

Diamond-Square (Fractal/Ruido)

Genera el heightmap del terreno.

En nuestra versión se limitan los valores negativos para evitar depresiones extremas y se controla el rango de ruido para obtener terrenos más jugables.

Algoritmo Constructivo

Se crean salas circulares en el mapa.

Se conectan entre sí con pasillos rectos, facilitando la navegación.

L-System (Gramática)

Se parte de un axioma y una regla de reescritura.

Tras varias generaciones se obtiene una estructura ramificada que se interpreta como un árbol.

🎮 Controles

W / A / S / D → mover la cámara en el plano.

Q / E → mover la cámara hacia abajo / arriba.

T → regenerar el terreno, las salas, pasillos y árboles.

💻 Instrucciones de ejecución

Clonar este repositorio:

git clone <URL_DEL_REPOSITORIO>


Abrir la carpeta del proyecto en Unity (2021.3 o superior recomendado).

Ejecutar la escena principal (MainScene.unity).

Explorar el terreno generado y regenerar el mundo con la tecla T.

🌍 Contextos de uso

El sistema puede adaptarse a diferentes proyectos:

Mazmorras RPG → con salas y pasillos jugables en un terreno fractal.

Paisajes naturales → eliminando las salas, queda un generador de montañas.

📂 Repositorio e itch.io

Repositorio: [GitHub](https://github.com/FrancoToro/GP_Proyecto_1)

Versión jugable: [itch.io](https://rigxorzero.itch.io/procedural-dungeon)

👥 Autores

Integrante 1 – [Franco Toro](https://github.com/FrancoToro)

Integrante 2 – [Matias Oyarzún](https://github.com/gekicchi)

Integrante 3 – [Héctor Villalobos](https://github.com/RigxorZero)
