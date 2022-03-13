**RESUMEN ENUNCIADO**

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
    -Comportamiendo de persecución del minotaruro a jugador en caso de que lo
        detecte en su campo de visión
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

**COMPORTAMIENTOS A AÑADIR**

-Algoritmo generación caminos eficiente = A*

    function reconstruct_path(cameFrom, current)
        total_path := {current}
        while current in cameFrom.Keys:
            current := cameFrom[current]
            total_path.prepend(current)
        return total_path

    // A* finds a path from start to goal.
    // h is the heuristic function. h(n) estimates the cost to reach goal from node n.
    function A_Star(start, goal, h)
        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        // This is usually implemented as a min-heap or priority queue rather than a hash-set.
        openSet := {start}

    // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
    // to n currently known.
    cameFrom := an empty map

    // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
    gScore := map with default value of Infinity
    gScore[start] := 0

    // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
    // how short a path from start to finish can be if it goes through n.
    fScore := map with default value of Infinity
    fScore[start] := h(start)

    while openSet is not empty
        // This operation can occur in O(Log(N)) time if openSet is a min-heap or a priority queue
        current := the node in openSet having the lowest fScore[] value
        if current = goal
            return reconstruct_path(cameFrom, current)

        openSet.Remove(current)
        for each neighbor of current
            // d(current,neighbor) is the weight of the edge from current to neighbor
            // tentative_gScore is the distance from start to the neighbor through current
            tentative_gScore := gScore[current] + d(current, neighbor)
            if tentative_gScore < gScore[neighbor]
                // This path to neighbor is better than any previous one. Record it!
                cameFrom[neighbor] := current
                gScore[neighbor] := tentative_gScore
                fScore[neighbor] := tentative_gScore + h(neighbor)
                if neighbor not in openSet
                    openSet.add(neighbor)

    // Open set is empty but goal was never reached
    return failure

-Suavizado de caminos = Path Smoothing

    function smoothPath(inputPath: Vector[]) -> Vector[]:

    # Compile an output path.
    outputPath = [inputPath[0]]

    # Keep track of where we are in the input path. We start at 2,
    # because we assume two adjacent nodes will pass the ray cast. 
    inputIndex: int = 2

    # Loop until we find the last item in the input.
    while inputIndex < len(inputPath) - 1:
        # Do 4 ray cast that represent de corners of the collider, so the character doesn hit the walls.
        fromPt = outputPath[len(outputPath) - 1]
        toPt = inputPath[inputIndex]
        fromPt1 = fromPt + offsets
        toPt1 = toPt + offsets
        fromPt2 = fromPt + offsets
        toPt2 = toPt + offsets
        fromPt3 = fromPt + offsets
        toPt3 = toPt + offsets
        fromPt4 = fromPt + offsets
        toPt4 = toPt + offsets
        if not rayClear(fromPt, toPt) and not rayClear(fromPt1, toPt1) and ... rayClear(fromPt4, toPt4):
            # The ray cast failed, add the last node that passed to
            # the output list.
            outputPath += inputPath[inputIndex - 1]

        # Consider the next node.
        inputIndex ++

    # We've reached the end of the input path, add the end node to the
    # output and return it.
    outputPath += inputPath[len(inputPath) - 1] 

    return outputPath

-Movimiento de merodeo del minotauro = Selección de un objetivo aleatorio y 
generación del camino mediante A*

**IMPLEMENTACION FINAL**

-Movimiento del jugador: 
 
Para el movimiento del jugador hemos utilizado el control jugador de la primera práctica modificando algunos
detalles, y para el movimiento automático hasta la salida, hemos utilizado AStar, la heuristica utilizada es la Distancia Euclidea o Manhattan,
se puede cambiar con la tecla H. Hasta que el jugador no llega a hasta una pequeña distancia del centro de la casilla
no empieza a dirigirse a la siguiente, asi evitamos que choque en algunas esquinas.

El hilo de Ariadna se crea desde el componente GoToCass, en el método DrawPath, con ayuda del componente de Unity LineRenderer.
Se introducen los puntos con LineRenderer.setPosition, y este los une con una linea que representa el hilo.
Las casillas se colorean de morado (en blanco a penas se veían) en el mismo DrawPath cambiando el material del MeshRenderer.

-Movimiento del Minotauro:

El movimiento aleatorio del minotauro es através del mismo GoToCass que utiliza el jugador, solo que en vez de ir a la salida 
escoge una casilla aleatoria y se mueve utilizando AStar hasta esa casilla, una vez ha llegado escoge una nueva. Si en el 
proceso ve al jugador, su movimiento cambia a perseguirle. Este comportamiento se define en el componente FieldOfView, que
crea una circunferencia, dectecta si el player esta dentro de esta circunferencia y comprueba si está en un angulo de visión 
respecto la direccíon en la que esta mirando, y si tambien se cumple esta condición lanza 4 raycast desde las esquinas del colider 
a las esquinas del colider del jugador, y si estos rayos no chocan con ningún obstaculo entonces se mueve hacia él. Si alguna de estas
condiciones no se cumple, se sigue mviendo en la ultima dirección en la que estaba el player un breve momento, y si aun no lo 
encuentra entoces vuelve a su movimiento aleatorio.

Cuando se activa o desactiva el suavizado, se realiza tanto para el minotauro como para el player.

El minotauro además hace que su casilla se comporte como un obstaculo para el player, y las cuatro casillas colindantes al 
minotauro cuentan como atravesar 8 casillas normales, por lo que el player evitará ese aumento de coste.

-Laberinto:

Existe la posibilidad de crear un mapa aleatorio con pasillos, que se genera de forma procedural dependiendo de 
la un Random cuya semilla es la hora actual del dia, por lo que siempre genera un mapa diferente. Para ello hemos 
utilizado el pseudocódigo del libro:
    
    #Clase que lleva la casilla a la que refiere
    class Location:
     x: int
     y: int

     function makeConnection(location: Location) -> Location:
     # Consider neighbors in a random order.
     neighbors = shuffle(NEIGHBORS)

     x = location.x
     y = location.y

     for (dx, dy, dirn) in neighbors:
         # Check if that location is valid.
         nx = x + dx
         ny = y + dy
         fromDirn = 3 - dirn

         if nx >= 0 && nx < numRows && ny >= 0 && ny < numCols && !yaMarcada:   
             # Perform the connection.
             cells[x][y].directions[dirn] = true
             cells[nx][ny].inMaze = true
             cells[nx][ny].directions[fromDirn] = true
             return Location(nx, ny)

     # null of the neighbors were valid.
     return null

     function maze(level: Level, start: Location):
         # A stack of locations we can branch from.
         locations = [start]
         playerIniPos = start;

         while locations:
         current = locations.top()

         # Try to connect to a neighboring location.
         next = level.makeConnection(current)
         if next:
         # If successful, it will be our next iteration.
         locations.push(next)
         else:
         locations.pop()

Si no para hacer prubas con un mapa con salas puedes utilizar el mapa por defecto.

Para cambiar entre mapa aleatorio y el predefinido utilizaremos la tecla m.

En el caso de generar un mapa aleatorio, el player siempre se genera en la entrada, y el minotauro en una casilla aleatoria que no sea obstáculo. 

Hemos hecho ajustes en la mayoria de los métodos de la plantilla a lo largo del desarrollo, para adaptarlos a lo que nos fuese necesario
para la práctica. también hemos añadido otros métodos en estas plantillas para mejorar su funcionalidad, como el randCass, que devuelve una casilla
aleatoria del graphgrid, entre otros.
