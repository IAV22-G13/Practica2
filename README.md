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

function pathfindAStar(graph: Graph, start: Node, end: Node, heuristic: Heuristic) -> Connection();
    # This structure is used to keep track of the
    # information we need for each node. 
    class NodeRecord:
        node: Node
        connection: Connection 
        costSoFar: float
        estimatedlotalCost: float

    # Initialize the record for the start node.
    startRecord = new NodeRecord()
    startRecord.node = start
    startRecord.connection = null
    startRecord.costSoFar =
    startRecord.estimatedTotalCost = heuristic.estimate(start)

    # Initialize the open and closed lists. 
    open = new PathfindingList()
    open += startRecord
    closed = new PathfindingList()

    # Iterate through processing each node. 
    while length(open) >
        # Find the smallest element in the open list (using the
        # estimatedTotalCost).
        current = open.smallestElement()

        # If it is the goal node, then terminate.
        if current.node == goal:
            break

        # Otherwise get its outgoing connections.
        connections = graph. getConnections( current)

        # Loop through each connection in turn.
        for connection in connections:
            # Get the cost estimate for the end node. 
            endNode = connection.getToNode()
            endNodeCost = current.costSoFar + connection.getCost()

            # If the node is closed we may have to skip, or remove it
            # from the closed list.
            if closed.contains(endNode):
                # Here we find the record in the closed list
                # corresponding to the endNode.
                endNodeRecord = closed.find(endNode)

                # If we didn't find a shorter route, skip. 
                if endNodeRecord.costSoFar <= endNodeCost: 
                    continue

                # Otherwise remove it from the closed list. 
                closed -= endNodeRecord

                # We can use the node's old cost values to calculate
                # its heuristic without calling the possibly expensive
                # heuristic function.
                endNodeHeuristic = endNodeRecord.estimatedTotalCost - 
                            endNodeRecord.costSoFar

            # Skip if the node is open and we've not found a better
            # route.
            else if open.contains(endNode):
                # Here we find the record in the open list
                # corresponding to the endNode.
                endNodeRecord = open.find(endNode)

                # If our route is no better, then skip.
                if endNodeRecord.costSoFar <= endNodeCost: 
                    continue

                # Again, we can calculate its heuristic. 
                endNodeHeuristic = endNodeRecord.cost -
                                    endNodeRecord.costSoFar

            # Otherwise we know we've got an unvisited node, so make a 
            # record for it.
            else:
                endNodeRecord = new NodeRecord()
                endNodeRecord.node = endNode

                # Well need to calculate the heuristic value using
                # the function, since we don't have an existing record 
                # to use.
                endNodeHeuristic = heuristic.estimate(endNode)

            # Were here if we need to update the node. Update the 
            # cost, estimate and connection.
            endNodeRecord.cost = endNodeCost
            endNodeRecord. connection = connection 
            endNodeRecord.estimatedTotalCost = endNodeCost + 
                                        endNodeHeuristic

            # And add it to the open list. 
            if not open.contains(endNode): 
                open += endNodeRecord

        # We've finished looking at the connections for the current
        # node, so add it to the closed list and remove it from the
        # open list.
        open -= current
        closed += current

    # Were here if we've either found the goal, or if we've no more 
    # nodes to search, find which.
    if current.node != goal:
        # We've run out of nodes without finding the goal, so there's 
        # no solution.
        return null

    else:
        # Compile the list of connections in the path.
        path = []

        # Work back along the path, accumulating connections. 
        while current.node != start:
            path += current.connection
            current = current.connection.getFromNode()

        # Reverse the path, and return it. 
        return reverse(path)

-Suavizado de caminos = Path Smoothing

function smoothPath(inputPath: Vector[]) -> Vector[]:
    # If the path is only two nodes long, then we can't smooth it, so
    # return.
    if len(inputPath) = 2:
       return inputPath

    # Compile an output path.
    outputPath = [inputPath[0]]

    # Keep track of where we are in the input path. We start at 2,
    # because we assume two adjacent nodes will pass the ray cast. 
    inputIndex: int = 2

    # Loop until we find the last item in the input.
    while inputIndex < len(inputPath) - 1:
        # Do the ray cast.
        fromPt = outputPath[len(outputPath) - 1]
        toPt = inputPath[inputIndex]
        if not rayClear(fromPt, toPt):
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

