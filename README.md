*RESUMEN ENUNCIADO*

En esta práctica deberemos explorar los diferentes algoritmos de navegación de
entornos, y para ello contaremos con diferentes personajes con comportamientos
diferentes y un entorno dividido en casillas por el cual se moverán.

-Teseo, el jugador, se mueve en la dirección en la que le indique el jugador
pinchando con el ratón en el mapa, además, en el caso de que el jugador pulse 
la tecla espacio este pasará a moverse de forma automática por el camino más corto
hasta la baldosa de salida. Este camino, que representa el hilo de Ariadna, se
mostrará en pantanlla con una linea blanca, y las baldosas por las que pasa
brillarán con circulos blancos mientras el jugador siga manteniendo el espacio.
-Minotauro, un enemigo que merodea por el escenario, y en caso de que Teseo
entre en su línea de visión le perseguirá.

Contenidos requeridos para la práctica:

-Comportamiento de merodeo por casillas para el Minotauro.
-Algoritmo de recorrido mínimo para el jugador, tanto para moverse hacia la baldosa
de salida, como para moverse hacia la casilla pulsada por el jugador.
-Algoritmo de suavizado de caminos, que se activa o desactiva a preferencia del
usuario.
-Representación del hilo de Ariadna y brillo de la casilla por la que pasa.

Restricciones

-No utilizar plugins de terceros.
-Documentar los algoritmos y heurísticas.
-Diseñar y programar de forma limpia.

Extras:

-Generar el laberinto de forma procedural.
-Hacer un movimiento en patrulla del minotauro y permitir un número variable de
monstruos.
-Añadir baldosas que cambien la velocidad de movimiento de los personajes.
-Cambiar la heurística utilzada en A*
-Añadir varias salidas al laberinto y que Teseo escoja la más cercana a la que ir
con el movimiento automático.

**PUNTO PARTIDA**

En el proyecto de Unity contamos con una escena en la que hay dos elementos para mostar el comportamiento proporcionado por el profesor.
En primer lugar, un GraphGrid, que a partir de un archivo .txt crea un tablero, generando dos tipos de prefabs distintos según si es una 
pared o una casilla válida. Este comportamiento configura también la información de cada casilla, poniéndoles el componente Vertex, encargado
de almacenar la posición de la casilla, así como las aristas a sus vecinos(pudiendo ser solo las 4 adyacentes en horizontal y vertical, o las 8, contando 
diagonales) y el vertex anterior. Por último, tiene un método para averiguar cual de sus vecinos está más cerca.
Además hay un TesterGraph, encargado de dibujar en la pestaña de escena el punto que selecciones de inicio, el final, y el camino entre ellos,
pudiendo diferenciar entre dfs, bfs o astar. Además tiene la opción de suavizar el camino, aunque el algoritmo aún está por programar.
Por último, se ha proporcionado un scripts extra que podrá ser útil para completar los objetivos: BinaryHeap, para implementar colas
de prioridad.
Los controles del punto de partida son, apuntando con el ratón, click para marcar el punto inicial, apuntar para indicar el final, y espacio para
calcular el camino que une esos dos puntos.