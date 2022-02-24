*RESUMEN ENUNCIADO*

En esta práctica deberemos explorar los diferentes algoritmos de navegación de
entornos, y para ello contaremos con diferentes personajes con comportamientos
diferentes y un entorno dividido en casillas por el cual se moverán.

-Teseo, el jugador, se mueve en la dirección en la que le indique el jugador
pinchando con el ratón en el mapa, además, en el caso de que el jugador pulse 
la tecla espacio este pasará a moverse de forma automática por el camino más corto
hasta la baldosa de salida. Este cámino, que representa el hilo de Ariadna, se
mostrará en pantanlla con una linea blanca, y las baldosas por las que pasa
brillarán con circulos blancos mientras el jugador siga manteniendo el espacio.
-Minotauro, un enemigo que merodea por el escenario, y en caso de que Teseo
entre en su línea de visión le perseguirá.

Contenidos requeridos para la práctica:

-Comportamiento de merodeo por casillas para el Minotauro.
-Algoritmo de recorrido mínimo para el jugador, tanto para moverse hacia la baldosa
de salida, como para moverse hacie la casilla pulsada por el jugador.
-Algoritmo de suavizado de caminos, que se activa o desactiva.
-Representación del hilo de Ariadna y brillo de la casilla por la que pasa.

Extras:

-Generar el laberinto de forma procedural.
-Hacer un movimiento en patrulla del minotauro y permitir un número variable de
monstruos.
-Añadir baldosas que cambien la velocidad de movimiento de los personajes.
-Cambiar la heurística utilzada en A*
-Añadir varias salidas al laberinto y que Teseo escoja la más cercana a la que ir
con el movimiento automático.