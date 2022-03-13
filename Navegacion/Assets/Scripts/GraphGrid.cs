/*    
    Obra original:
        Copyright (c) 2018 Packt
        Unity 2018 Artificial Intelligence Cookbook - Second Edition, by Jorge Palacios
        https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition
        MIT License

    Modificaciones:
        Copyright (C) 2020-2022 Federico Peinado
        http://www.federicopeinado.com

        Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
        Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
        Contacto: email@federicopeinado.com
*/

public class location
{
    public int x;
    public int y;
    public location(int x_, int y_) { x = x_; y = y_; }
}

namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Random = UnityEngine.Random;

    public class GraphGrid : Graph
    {
        public GameObject obstaclePrefab;
        public GameObject playerPrefab;
        public string mapsDir = "Maps"; // Directorio por defecto
        public string mapName = "arena.map"; // Fichero por defecto
        public bool get8Vicinity = false;
        public float cellSize = 1f;
        [Range(0, Mathf.Infinity)]
        public float defaultCost = 1f;
        [Range(0, Mathf.Infinity)]
        public float maximumCost = Mathf.Infinity;


        public location[] mazeNeigh = new location[] { new location(0, 1), new location(0, -1), new location(1, 0), new location(-1, 0) };

        [SerializeField]
        bool randomMap = false;
        [SerializeField]
        int sizeRandom;

        int numCols;
        int numRows;
        GameObject[] vertexObjs;
        bool[,] mapVertices;
        location playerIniPos;
        Quaternion playerIniRot;
        int endCass;

        private int GridToId(int x, int y)
        {
            return Math.Max(numRows, numCols) * y + x;
        }

        private Vector2 IdToGrid(int id)
        {
            Vector2 location = Vector2.zero;
            location.y = Mathf.Floor(id / numCols);
            location.x = Mathf.Floor(id % numCols);
            return location;
        }

        private void LoadRandMap()
        {
            int j = 0;
            int i = 0;
            int id = 0;

            Vector3 position = Vector3.zero;
            Vector3 scale = Vector3.zero;
            numRows = sizeRandom;
            numCols = sizeRandom;

            vertices = new List<Vertex>(numRows * numCols);
            neighbors = new List<List<Vertex>>(numRows * numCols);
            costs = new float[numRows * numCols];
            vertexObjs = new GameObject[numRows * numCols];
            mapVertices = new bool[numRows, numCols];

            for (i = 0; i < numRows; i++)
            {
                for (j = 0; j < numCols; j++)
                {
                    mapVertices[i, j] = false;
                }
            }

            Random.InitState(System.DateTime.Now.Second);

            //AQUI
            int p = Random.Range(0, 4);
            int o = Random.Range(1, numRows - 1);
            if (o % 2 == 0)
                o += 1;

            switch (p)
            {
                case 0:
                    mapVertices[o, 0] = true;
                    randMap(new location(o, 1));
                    playerIniRot = Quaternion.Euler(0, 90, 0);
                    break;
                case 1:
                    mapVertices[0, o] = true;
                    randMap(new location(1, o));
                    playerIniRot = Quaternion.Euler(0, 0, 0);
                    break;
                case 2:
                    mapVertices[o, sizeRandom - 1] = true;
                    randMap(new location(o, sizeRandom - 2));
                    playerIniRot = Quaternion.Euler(0, 270, 0);
                    break;
                case 3:
                    mapVertices[sizeRandom - 1, o] = true;
                    randMap(new location(sizeRandom - 2, o));
                    playerIniRot = Quaternion.Euler(0, 180, 0);
                    break;
            }

            o = Random.Range(1, numRows - 1);
            if (o % 2 == 0)
                o += 1;
            switch (p)
            {
                case 0:
                    mapVertices[o, sizeRandom - 1] = true;
                    endCass = GridToId(sizeRandom - 1, o);
                    break;
                case 1:
                    mapVertices[sizeRandom - 1, o] = true;
                    endCass = GridToId(o, sizeRandom - 1);
                    break;
                case 2:
                    mapVertices[o, 0] = true;
                    endCass = GridToId(0, o);
                    break;
                case 3:
                    mapVertices[0, o] = true;
                    endCass = GridToId(o, 0);
                    break;
            }

            for (i = 0; i < numRows; i++)
            {
                for (j = 0; j < numCols; j++)
                {
                    position.x = j * cellSize + 0.5f;
                    position.z = i * cellSize + 0.5f;
                    id = GridToId(j, i);
                    if (mapVertices[i, j])
                        vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity) as GameObject;
                    else
                        vertexObjs[id] = Instantiate(obstaclePrefab, position, Quaternion.identity) as GameObject;
                    vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                    Vertex v = vertexObjs[id].AddComponent<Vertex>();
                    v.id = id;
                    vertices.Add(v);
                    neighbors.Add(new List<Vertex>());
                    float y = vertexObjs[id].transform.localScale.y;
                    scale = new Vector3(cellSize, y, cellSize);
                    vertexObjs[id].transform.localScale = scale;
                    vertexObjs[id].transform.parent = gameObject.transform;
                }
            }

            // now onto the neighbours
            for (i = 0; i < numRows; i++)
            {
                for (j = 0; j < numCols; j++)
                {
                    SetNeighbours(j, i, get8Vicinity);
                }
            }

        }

        public GameObject getEndCass()
        {
            return vertices[endCass].gameObject;
        }

        void randMap(location ini)
        {
            playerIniPos = ini;
            mapVertices[ini.x, ini.y] = true;

            Stack<location> lAux = new Stack<location>();
            lAux.Push(ini);

            while (lAux.Count > 0)
            {
                location curr = lAux.Peek();

                location next = makeConnection(curr);
                if (next != null)
                {
                    lAux.Push(next);
                }
                else
                {
                    lAux.Pop();
                }
            }
        }

        location makeConnection(location l)
        {
            location[] neigh = shuffle();

            int x = l.x;
            int y = l.y;

            for (int i = 0; i < 4; i++)
            {
                int nx = x + (neigh[i].x * 2);
                int ny = y + (neigh[i].y * 2);
                if (nx >= 0 && nx < numRows && ny >= 0 && ny < numCols && !mapVertices[nx, ny])
                {
                    mapVertices[nx, ny] = true;
                    mapVertices[x + neigh[i].x, y + neigh[i].y] = true;
                    return (new location(nx, ny));
                }
            }


            return null;
        }

        location[] shuffle()
        {
            location[] aux = new location[4];
            int r = Random.Range(0, 4);
            for (int i = 0; i < 4; i++)
            {
                aux[i] = mazeNeigh[(r + i) % 4];
            }
            return aux;
        }

        private void LoadMap(string filename)
        {
            string path = Application.dataPath + "/" + mapsDir + "/" + filename;
            try
            {
                StreamReader strmRdr = new StreamReader(path);
                using (strmRdr)
                {
                    int j = 0;
                    int i = 0;
                    int id = 0;
                    string line;

                    Vector3 position = Vector3.zero;
                    Vector3 scale = Vector3.zero;
                    line = strmRdr.ReadLine();// non-important line
                    line = strmRdr.ReadLine();// height
                    numRows = int.Parse(line.Split(' ')[1]);
                    line = strmRdr.ReadLine();// width
                    numCols = int.Parse(line.Split(' ')[1]);
                    line = strmRdr.ReadLine();// "map" line in file

                    vertices = new List<Vertex>(numRows * numCols);
                    neighbors = new List<List<Vertex>>(numRows * numCols);
                    costs = new float[numRows * numCols];
                    vertexObjs = new GameObject[numRows * numCols];
                    mapVertices = new bool[numRows, numCols];

                    for (i = 0; i < numRows; i++)
                    {
                        line = strmRdr.ReadLine();
                        for (j = 0; j < numCols; j++)
                        {
                            bool isGround = true;
                            if (line[j] != '.')
                                isGround = false;
                            mapVertices[i, j] = isGround;
                            position.x = j * cellSize;
                            position.z = i * cellSize;
                            id = GridToId(j, i);
                            if (isGround)
                                vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity) as GameObject;
                            else
                                vertexObjs[id] = Instantiate(obstaclePrefab, position, Quaternion.identity) as GameObject;
                            vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                            Vertex v = vertexObjs[id].AddComponent<Vertex>();
                            v.id = id;
                            vertices.Add(v);
                            neighbors.Add(new List<Vertex>());
                            float y = vertexObjs[id].transform.localScale.y;
                            scale = new Vector3(cellSize, y, cellSize);
                            vertexObjs[id].transform.localScale = scale;
                            vertexObjs[id].transform.parent = gameObject.transform;
                        }
                    }

                    // now onto the neighbours
                    for (i = 0; i < numRows; i++)
                    {
                        for (j = 0; j < numCols; j++)
                        {
                            SetNeighbours(j, i, get8Vicinity);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Load()
        {
            playerIniPos = new location(0, 0);
            if (!randomMap)
                LoadMap(mapName);
            else
                LoadRandMap();
            Instantiate(playerPrefab, new Vector3(playerIniPos.y * cellSize /*+ (cellSize / 2)*/, 0, playerIniPos.x * cellSize /*+ (cellSize / 2)*/), playerIniRot);
        }

        protected void SetNeighbours(int x, int y, bool get8 = true)
        {
            int col = x;
            int row = y;
            int i, j;
            int vertexId = GridToId(x, y);
            neighbors[vertexId] = new List<Vertex>();   //??????????
            costs[vertexId] = new float();
            Vector2[] pos = new Vector2[0];             //??????????
            if (get8)
            {
                pos = new Vector2[8];
                int c = 0;
                for (i = row - 1; i <= row + 1; i++)
                {
                    for (j = col - 1; j <= col + 1; j++)    //Añadida 3ª columna
                    {
                        if (i == row && j == col)
                            continue;
                        pos[c] = new Vector2(j, i);
                        c++;
                    }
                }
            }
            else
            {
                pos = new Vector2[4];
                pos[0] = new Vector2(col, row - 1);
                pos[1] = new Vector2(col - 1, row);
                pos[2] = new Vector2(col + 1, row);
                pos[3] = new Vector2(col, row + 1);
            }
            foreach (Vector2 p in pos)
            {
                i = (int)p.y;
                j = (int)p.x;
                if (i < 0 || j < 0 || i >= numRows || j >= numCols || (i == row && j == col) || !mapVertices[i, j])
                    continue;
                int id = GridToId(j, i);
                neighbors[vertexId].Add(vertices[id]);
                costs[vertexId] = defaultCost;
            }
        }

        public override float[] GetNeighboursCosts(int vertId)
        {
            Vector2[] pos = new Vector2[4];
            Vector2 p = IdToGrid(vertId);
            pos[0] = new Vector2(p.x, p.y - 1);
            pos[1] = new Vector2(p.x - 1, p.y);
            pos[2] = new Vector2(p.x + 2, p.y);
            pos[3] = new Vector2(p.x, p.y + 1);

            int tam = neighbors[vertId].Count;
            float[] n = new float[tam];

            for (int i = 0; i < tam; i++)
            {
                int x = (int)pos[i].y;
                int y = (int)pos[i].x;
                if (x < 0 || y < 0 || x >= numRows || y >= numCols || !mapVertices[x, y])
                    continue;
                n[i] = costs[GridToId(x, y)];
            }

            return n;
        }

        public override Vertex GetNearestVertex(Vector3 position)
        {
            int col = (int)(position.x / cellSize);
            int row = (int)(position.z / cellSize);
            Vector2 p = new Vector2(col, row);
            List<Vector2> explored = new List<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            queue.Enqueue(p);
            do
            {
                p = queue.Dequeue();
                col = (int)p.x;
                row = (int)p.y;
                int id = GridToId(col, row);
                if (mapVertices[row, col])
                    return vertices[id];

                if (!explored.Contains(p))
                {
                    explored.Add(p);
                    int i, j;
                    for (i = row - 1; i <= row + 1; i++)
                    {
                        for (j = col - 1; j <= col + 1; j++)
                        {
                            if (i < 0 || j < 0)
                                continue;
                            if (j >= numCols || i >= numRows)
                                continue;
                            if (i == row && j == col)
                                continue;
                            queue.Enqueue(new Vector2(j, i));
                        }
                    }
                }
            } while (queue.Count != 0);
            return null;
        }

    }
}
