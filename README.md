# Debugging-The-History
 Videojuego Trivial/RPG para Android

Realizado por Iván juárez Rincón como proyecto de fin de ciclo para el Grado Superior de Desarrollo de Aplicaciones Multiplataforma.

Este repositorio contiene los archivos esenciales para la importación del proyecto a un equipo que tenga instalado el editor Unity 3d. Debido a que este programa crea una gran cantidad de archivos de forma automatica que no son necesarios a la hora de subir el proyecto, aqui solo están los imprescindibles.

Ademas del contenido de este repositorio, sera necesario añadir a la carpeta Assets una subcarpeta con los audios utilizados en el juego, los cuales exceden el limite de 100 MB que tiene Git Hub. Estos audios se pueden descargar desde el siguiente enlace: https://drive.google.com/drive/folders/186t6ksUPWa72g1ZevZnLsA9TGKrDYmKf?usp=sharing

Para poder ejecutar el proyecto en un entorno Android, primero se debera generar un APK desde el editor. La forma de realizarlo es la siguiente:

1 - Se debe crear un proyecto nuevo en Unity. 
2 - Una vez creado, en la carpeta del proyecto se copiaran todos los archivos de este repositorio más la carpeta de audios.
3 - Se selecciona File --> Build Settings --> y en la ventana que aparecera, se añadiran todas las escenas del juego, abriendo cada una de ellas de forma individual y      seleccionando Add Open Scenes en la ventana.
4 - Cuando esten todas las escenas añadidas, seleccionamos la opción Build, la cual generará el APK del proyecto.
