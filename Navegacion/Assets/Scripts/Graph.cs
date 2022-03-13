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
    using System;

    /// <summary>
    /// Abstract class for graphs
    /// </summary>

    public class NodeRecord : IComparable<NodeRecord>
    {
        public Vertex vertex;
        public Vertex connection;
        public float costSoFar;
        public float estimatedTotalCost;

        public NodeRecord()
        {
            this.vertex = null;
            this.connection = null;
            this.costSoFar = 0;
            this.estimatedTotalCost = 0;
        }

        public int CompareTo(NodeRecord b)
        {
            if (this.estimatedTotalCost < b.estimatedTotalCost) return -1;
            else if (this.estimatedTotalCost > b.estimatedTotalCost) return 1;
            return 0;
        }

        public bool Equals(NodeRecord b)
        {
            return (this.vertex.id == b.vertex.id);
        }
        public override bool Equals(object b)
        {
            NodeRecord bN = (NodeRecord)b;
            if (ReferenceEquals(b, null)) return false;
            return (this.vertex.id == bN.vertex.id);
        }
        public static bool operator <(NodeRecord a, NodeRecord b)
        {
            return a.estimatedTotalCost < b.estimatedTotalCost;
        }

        public static bool operator >(NodeRecord a, NodeRecord b)
        {
            return a.estimatedTotalCost > b.estimatedTotalCost;
        }
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

        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO/*, Heuristic h = null*/)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex srcOV = srcO.GetComponent<Vertex>();
            Vertex dstOV = dstO.GetComponent<Vertex>();
            // AQUÍ HAY QUE PONER LA IMPLEMENTACIÓN DEL ALGORITMO A*
            NodeRecord startRecord = new NodeRecord();
            startRecord.vertex = srcOV;
            startRecord.costSoFar = startRecord.estimatedTotalCost = EuclidDist(srcOV, dstOV); //h.estimated(srcOV);

            BinaryHeap<NodeRecord> open = new BinaryHeap<NodeRecord>();     //Lista ordenada por estimatedfTotalCost
            List<NodeRecord> openList = new List<NodeRecord>();
            open.Add(startRecord);
            openList.Add(startRecord);
            List<NodeRecord> closed = new List<NodeRecord>();

            NodeRecord current = open.Top;
            while (open.Count != 0)
            {
                if (current.vertex.id == dstOV.id) 
                    break;

                open.Remove();
                openList.Remove(current);
                //Recorre los vecinos de current
                Vertex[] connections = GetNeighbours(current.vertex);
                for (int i = 0; i < connections.Length; i++)
                {
                    NodeRecord endNode = new NodeRecord();
                    endNode.vertex = connections[i];
                    float endNodeCost = current.costSoFar + costs[current.vertex.id][i];    //Coste de llegada al vecino
                    float endNodeHeuristic;
                    NodeRecord endNodeRecord;
                    //Si está en lista de procesados
                    if (closed.Contains(endNode))
                    {
                        endNodeRecord = closed[closed.IndexOf(endNode)];

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        closed.Remove(endNodeRecord);

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                    }
                    //Si está para procesar
                    else if (openList.Contains(endNode))
                    {
                        endNodeRecord = openList[openList.IndexOf(endNode)];

                        if (endNodeRecord.costSoFar <= endNodeCost)
                            continue;

                        endNodeHeuristic = endNodeRecord.estimatedTotalCost - endNodeRecord.costSoFar;
                        open.Remove(endNode);
                        openList.Remove(endNode);
                    }
                    //Primera vez llega
                    else
                    {
                        endNodeRecord = new NodeRecord();
                        endNodeRecord.vertex = endNode.vertex;

                        endNodeHeuristic = EuclidDist(current.vertex, endNodeRecord.vertex);
                        open.Add(endNodeRecord);
                        openList.Add(endNodeRecord);
                    }

                    endNodeRecord.costSoFar = endNodeCost;
                    endNodeRecord.connection = current.vertex; // connections[i];
                    endNodeRecord.estimatedTotalCost = endNodeCost + endNodeHeuristic;

                    if (!open.Contains(endNodeRecord))
                    {
                        open.Add(endNodeRecord);
                        openList.Add(endNodeRecord);
                    }
                }
                //Actualizamos actual, quitamos de open y ponemos en closed
                closed.Add(current);
                if (open.Count > 0) 
                    current = open.Top;
            }
            if (current.vertex != dstOV)
            {
                NodeRecord des = new NodeRecord();
                des.vertex = dstOV;
                if (closed.Contains(des)) 
                    Debug.Log("si estoy");
                return new List<Vertex>();
            }
            else
            {
                List<Vertex> sol = new List<Vertex>();
                while (current.vertex != srcOV)
                {
                    sol.Insert(0, current.vertex);
                    NodeRecord a = new NodeRecord();
                    a.vertex = current.connection;
                    current = closed[closed.IndexOf(a)];
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
            int cass = UnityEngine.Random.Range(0, vertices.Count - 1);
            return vertices[cass].gameObject;
        }
    }

    //public virtual Edge[] GetEdges(Vertex v)
    //{
    //    //Si no tiene vecinos
    //    if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
    //        return new Edge[0];
    //    if (v.id < 0 || v.id >= neighbors.Count)
    //        return new Edge[0];
    //    //Numero de conexiones que hay
    //    int numConexiones = neighbors[v.id].Count;
    //    Edge[] conexiones = new Edge[numConexiones];
    //    List<Vertex> vertexList = neighbors[v.id];
    //    List<float> costList = costs[v.id];
    //    //Por cada una de las conexiones cogemos y nos guardamos la conexion en si
    //    for (int i = 0; i < numConexiones; i++)
    //    {
    //        conexiones[i] = new Edge();
    //        conexiones[i].cost = costList[i];
    //        conexiones[i].vertex = vertexList[i];
    //    }
    //    return conexiones;
    //}

    //public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null)
    //{
    //    Vertex src = GetNearestVertex(new Vector3(srcO.transform.position.x, srcO.transform.position.y, srcO.transform.position.z));
    //    Vertex dst = GetNearestVertex(new Vector3(dstO.transform.position.x, dstO.transform.position.y, dstO.transform.position.z));

    //    //Frontera
    //    BinaryHeap<Edge> frontera = new BinaryHeap<Edge>();
    //    Edge[] conexiones;
    //    Edge nodoActual, child;


    //    int size = vertices.Count;
    //    float[] distValue = new float[size];
    //    int[] previous = new int[size];


    //    nodoActual = new Edge(src, 0);
    //    frontera.Add(nodoActual);
    //    distValue[src.id] = 0;
    //    previous[src.id] = src.id;

    //    for (int i = 0; i < size; i++)
    //    {
    //        if (i == src.id)
    //            continue;
    //        distValue[i] = Mathf.Infinity;
    //        previous[i] = -1;
    //    }

    //    //Mientras haya algun sitio a donde ir...
    //    while (frontera.Count != 0)
    //    {
    //        nodoActual = frontera.Remove();
    //        int nodeId = nodoActual.vertex.id;
    //        if (ReferenceEquals(nodoActual.vertex, dst))
    //        {
    //            return BuildPath(src.id, nodoActual.vertex.id, ref previous);
    //        }

    //        conexiones = GetEdges(nodoActual.vertex);

    //        foreach (Edge e in conexiones)
    //        {
    //            int eId = e.vertex.id;
    //            if (previous[eId] != -1)
    //                continue;
    //            float cost = distValue[nodeId] + e.cost;

    //            cost += h.estimate(nodoActual.vertex, e.vertex);


    //            if (cost < distValue[e.vertex.id])
    //            {
    //                distValue[eId] = cost;
    //                previous[eId] = nodeId;
    //                frontera.Remove(e);
    //                child = new Edge(e.vertex, cost);
    //                frontera.Add(child);
    //            }
    //        }
    //    }
    //    return new List<Vertex>();

    //}
}