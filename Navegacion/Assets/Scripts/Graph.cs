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
namespace UCM.IAV.Navegacion
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract class for graphs
    /// </summary>

    struct NodeRecord
    {
        public Vertex vertex;
        public Vertex connection;
        public float costSoFar;
        public float estimatedTotalCost;

        public NodeRecord(Vertex vertex, Vertex conec, float costSoFar, float estimatedTotalCost)
        {
            this.vertex = vertex;
            this.connection = conec;
            this.costSoFar = costSoFar;
            this.estimatedTotalCost = estimatedTotalCost;
        }

        public static bool operator ==(NodeRecord a, NodeRecord b)
        {
            if (a.vertex == b.vertex) return true;
            else return false;
        }
        public static bool operator !=(NodeRecord a, NodeRecord b) => !(a == b);
    }

    public abstract class Graph : MonoBehaviour
    {
        public float raySize;
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbors;
        protected List<List<float>> costs;
        //protected Dictionary<int, int> instIdToId;

        //// this is for informed search like A*
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;
        //public bool isFinished;

        public virtual void Start()
        {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }


        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
                return new Vertex[0];
            if (v.id < 0 || v.id >= neighbors.Count)
                return new Vertex[0];
            return neighbors[v.id].ToArray();
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex[] neighbours;
            Queue<Vertex> q = new Queue<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id; // El vértice que tenga de previo a sí mismo, es el vértice origen
            q.Enqueue(src);
            while (q.Count != 0)
            {
                v = q.Dequeue();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id; // El vecino n tiene de 'padre' a v
                    q.Enqueue(n);
                }
            }
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex[] neighbours;
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id;
            Stack<Vertex> s = new Stack<Vertex>();
            s.Push(src);
            while (s.Count != 0)
            {
                v = s.Pop();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id;
                    s.Push(n);
                }
            }
            return new List<Vertex>();
        }

        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null)
        {
            Vertex srcOV = srcO.GetComponent<Vertex>();
            Vertex dstOV = dstO.GetComponent<Vertex>();
            // AQUÍ HAY QUE PONER LA IMPLEMENTACIÓN DEL ALGORITMO A*
            NodeRecord startRecord = new NodeRecord();
            startRecord.vertex = srcOV;
            //startRecord.costSoFar = startRecord.estimatedTotalCost = h.estimated(srcOV);

            List<NodeRecord> open = new List<NodeRecord>();     //Lista ordenada por estimatedfTotalCost
            open.Add(startRecord);
            List<NodeRecord> closed = new List<NodeRecord>();

            NodeRecord current = open[0];
            while (open.Count > current.estimatedTotalCost)
            {
                if (current.vertex == dstO) break;

                Vertex[] connections = GetNeighbours(current.vertex);
                for (int i = 0; i < connections.Length; i++)
                {
                    NodeRecord endNode = new NodeRecord();
                    endNode.vertex = connections[i];
                    float endNodeCost = current.costSoFar + costs[current.vertex.id][i];
                    float endNodeHeuristic = 0;
                    NodeRecord endNodeRecord;
                    if (closed.Contains(endNode))
                    {
                        endNodeRecord = closed[closed.IndexOf(endNode)];

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        closed.Remove(endNodeRecord);

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    else if (open.Contains(endNode))
                    {
                        endNodeRecord = open[open.IndexOf(endNode)];

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    else
                    {
                        endNodeRecord = new NodeRecord();
                        endNodeRecord.vertex = endNode.vertex;

                        //endNodeHeuristic = h.edstimated(endNode.vertex);
                    }

                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.connection = connections[i];
                    endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

                    if (!open.Contains(endNodeRecord))
                        open.Add(endNodeRecord);
                }

                open.Remove(current);
                closed.Add(current);
            }
            if (current.vertex != dstOV)
                return new List<Vertex>();
            else
            {
                List<Vertex> sol = new List<Vertex>();
                while (current.vertex != srcOV)
                {
                    sol.Insert(0, current.vertex);
                    //current = closed[closed.IndexOf(current.connection)];
                }

                return sol;
            }
        }

        public List<Vertex> Smooth(List<Vertex> path)
        {
            // AQUÍ HAY QUE PONER LA IMPLEMENTACIÓN DEL ALGORITMO DE SUAVIZADO
            if (path.Count <= 2)
                return path;

            List<Vertex> outputpath = new List<Vertex>();
            outputpath.Add(path[0]);

            int index = 2;

            while (index < path.Count - 1)
            {
                Vector3 fromPt = outputpath[outputpath.Count - 1].transform.position;
                Vector3 toPt = path[index].transform.position;
                fromPt.y = 0.5f;
                toPt.y = 0.5f;               

                Vector3 fromPt1 = new Vector3(fromPt.x - raySize, fromPt.y, fromPt.z - raySize);
                Vector3 fromPt2 = new Vector3(fromPt.x + raySize, fromPt.y, fromPt.z - raySize);
                Vector3 fromPt3 = new Vector3(fromPt.x - raySize, fromPt.y, fromPt.z + raySize);
                Vector3 fromPt4 = new Vector3(fromPt.x + raySize, fromPt.y, fromPt.z + raySize);

                Vector3 toPt1 = new Vector3(toPt.x - raySize, toPt.y, toPt.z - raySize);
                Vector3 toPt2 = new Vector3(toPt.x + raySize, toPt.y, toPt.z - raySize);
                Vector3 toPt3 = new Vector3(toPt.x - raySize, toPt.y, toPt.z + raySize);
                Vector3 toPt4 = new Vector3(toPt.x + raySize, toPt.y, toPt.z + raySize);

                if (Physics.Raycast(fromPt1, toPt1 - fromPt1, out RaycastHit hit, (toPt1 - fromPt1).magnitude) ||
                    Physics.Raycast(fromPt2, toPt2 - fromPt2, out RaycastHit hit1, (toPt2 - fromPt2).magnitude) ||
                    Physics.Raycast(fromPt3, toPt3 - fromPt3, out RaycastHit hit2, (toPt3 - fromPt3).magnitude) ||
                    Physics.Raycast(fromPt4, toPt4 - fromPt4, out RaycastHit hit3, (toPt4 - fromPt4).magnitude))
                {
                    outputpath.Add(path[index - 1]);
                }
                index++;
            }

            outputpath.Add(path[path.Count - 1]);
            return outputpath; //newPath
        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();
            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

        // Sí me parece razonable que la heurística trabaje con la escena de Unity
        // Heurística de distancia euclídea
        public float EuclidDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Vector3.Distance(posA, posB);
        }

        // Heurística de distancia Manhattan
        public float ManhattanDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }

        public GameObject randCass()
        {
            int cass = Random.Range(0, vertices.Count-1);
            return vertices[cass].gameObject;
        }
    }
}