using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class DijkstrasAlgorithm : MonoBehaviour
{
    private bool IsGoingForwards = true;

    // Current Position in the List = CPITL
    private int CPITL = 0;
    private int NodeListSize;
    private int NodeValue;

    private Vector3 CurrentVelocity;
    private Vector3 Force;
    private Vector3 Velocity;

    [HideInInspector]
    public bool HasShortestPathBeenFound = false;
    [HideInInspector]
    public bool IsEndNodeLocated = false;
    [HideInInspector]
    public bool IsEndingPointSet;
    [HideInInspector]
    public bool IsStartingPointSet;
    [HideInInspector]
    public bool IsThereAValidPath = false;

    public float MaximumVelocity;

    [HideInInspector]
    public GameObject EndingNode;
    [HideInInspector]
    public GameObject Node;

    [HideInInspector]
    public List<GameObject> NodeList = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> ShortestPath = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> TotalNodesCreated = new List<GameObject>();

    [HideInInspector]
    public Vector3 EndingPointCoordinates;
    [HideInInspector]
    public Vector3 StartingPointCoordinates;


    private void Start()
    {
        IsEndingPointSet = false;
        StartingPointCoordinates = this.transform.position;
        // When we create the first Node (Starting Node), we would give it a value of zero...
        // so when we create the Node List, we would store all the Nodes that have a value of zero.
        // Then, we would increase the Node Value by one and all the previous nodes with the previous value (0)...
        // would create new nodes but will have the new current value (1).
        // This would rinse and repeat until we would reach the End Node.
        // Afterwards, we would set the value of the End Node to be what the Current Node is now but plus one.
        // Then we would start from the End Node and go back finding whichever node value is one below our current value.
        if (IsStartingPointSet == true)
        {
            // The Starting Node's and future Node names would be equal to the Node Value.
            // When going from the End Node back to the Staring Node, it would check for the Node's value that is less (Node Value - 1) and if there is more than one...
            // it would grab the one that was found first.
            NodeValue = 0;

            Node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Node.name = NodeValue.ToString();
            Node.tag = "Starting Node";
            Node.transform.position = StartingPointCoordinates;
            Node.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            Node.GetComponent<SphereCollider>().isTrigger = true;
            Node.GetComponent<Renderer>().material.color = Color.green;
            NodeList.Add(Node);
            TotalNodesCreated.Add(Node);
        }
    }

    private void Update()
    {
        CurrentVelocity = Velocity;
    }

    // Dijkstra's Algorithm Functions
    public void DownLeftRightUp(float DBN, List<GameObject> NL)
    {
        if (IsEndNodeLocated != true)
        {
            // A fail-safe system, if there are too many nodes being made on the screen, the user can kill it...
            // by pressing the B and M key together
            if (Input.GetKeyDown(KeyCode.B) & Input.GetKeyDown(KeyCode.M))
            {
                NodeListSize = TotalNodesCreated.Count;
                for (int Stefanie = 1; Stefanie < NodeListSize; Stefanie++)
                {
                    Destroy(TotalNodesCreated[Stefanie]);
                }
                Destroy(EndingNode);
                IsEndingPointSet = false;
                NodeList.Clear();
                NodeValue = 0;
                TotalNodesCreated.Clear();
                NodeList.Add(Node);
                TotalNodesCreated.Add(Node);
                return;
            }

            if (IsStartingPointSet == true)
            {
                EndingNode = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                EndingNode.name = "Ending Node";
                EndingNode.tag = "End Node";
                EndingNode.transform.position = EndingPointCoordinates;
                EndingNode.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                EndingNode.GetComponent<Renderer>().material.color = Color.red;
                IsStartingPointSet = false;
            }

            // Create a new list and move all the new created nodes to the new list, and once you have all the nodes, you remove the previous...
            // node from the list and move all the nodes from the new created list onto the NL List and rinse/repeat.
            // Before we start building any nodes, we would want to make sure that if there are any null Nodes, we would remove those Nodes.
            NodeListSize = TotalNodesCreated.Count;
            for (int I = 0; I < NodeListSize; I++)
            {
                if (TotalNodesCreated[I] == null)
                {
                    TotalNodesCreated.Remove(TotalNodesCreated[I]);
                    NodeListSize = TotalNodesCreated.Count;
                    I = 0;
                }
            }

            int NLSize = NL.Count;
            List<GameObject> CreatedNodes = new List<GameObject>();
            for (int S = 0; S < NLSize; S++)
            {
                if (int.Parse(NL[S].name) == NodeValue)
                {
                    NodeValue++;

                    if (IsEndNodeLocated != true & IsThereAValidPath != true)
                    {
                        // Down
                        GameObject Down = Instantiate(NL[S]);
                        Down.name = NodeValue.ToString();
                        Down.tag = "Node";
                        Vector3 D = new Vector3(0.0f, 0.0f, -DBN);
                        Down.transform.position += D;
                        CreatedNodes.Add(Down);
                        // Create a list up above (TotalNodesCreated) and check current node to Current Node list
                        NodeListSize = TotalNodesCreated.Count;
                        // Destroys Current Node If There Are Exsisting Nodes There
                        for (int T = 0; T < NodeListSize; T++)
                        {
                            if (Down.transform.position == TotalNodesCreated[T].transform.position)
                            {
                                CreatedNodes.Remove(Down);
                                Destroy(Down);
                            }
                        }
                        // Destroys Current Node If In Contact Of End Node, Or Wall
                        Collider[] HitCollidersDown = Physics.OverlapSphere(Down.transform.position, DBN / 2);
                        foreach (Collider Hit in HitCollidersDown)
                        {
                            if (Hit.tag == "End Node")
                            {
                                IsEndNodeLocated = true;
                                IsThereAValidPath = true;
                                break;
                            }
                            if (Hit.tag == "Wall")
                            {
                                CreatedNodes.Remove(Down);
                                Destroy(Down);
                            }
                        }
                        if (Down != null)
                        {
                            TotalNodesCreated.Add(Down);
                        }
                    }

                    if (IsEndNodeLocated != true & IsThereAValidPath != true)
                    {
                        // Left
                        GameObject Left = Instantiate(NL[S]);
                        Left.name = NodeValue.ToString();
                        Left.tag = "Node";
                        Vector3 L = new Vector3(-DBN, 0.0f, 0.0f);
                        Left.transform.position += L;
                        CreatedNodes.Add(Left);
                        // Create a list up above (TotalNodesCreated) and check current node to Current Node list
                        NodeListSize = TotalNodesCreated.Count;
                        // Destroys Current Node If There Are Exsisting Nodes There
                        for (int E = 0; E < NodeListSize; E++)
                        {
                            if (Left.transform.position == TotalNodesCreated[E].transform.position)
                            {
                                CreatedNodes.Remove(Left);
                                Destroy(Left);
                            }
                        }
                        // Destroys Current Node If In Contact Of End Node, Or Wall
                        Collider[] HitCollidersLeft = Physics.OverlapSphere(Left.transform.position, DBN / 2);
                        foreach (Collider Hit in HitCollidersLeft)
                        {
                            if (Hit.tag == "End Node")
                            {
                                IsEndNodeLocated = true;
                                IsThereAValidPath = true;
                                break;
                            }
                            if (Hit.tag == "Wall")
                            {
                                CreatedNodes.Remove(Left);
                                Destroy(Left);
                            }
                        }
                        if (Left != null)
                        {
                            TotalNodesCreated.Add(Left);
                        }
                    }

                    if (IsEndNodeLocated != true & IsThereAValidPath != true)
                    {
                        // Right
                        GameObject Right = Instantiate(NL[S]);
                        Right.name = NodeValue.ToString();
                        Right.tag = "Node";
                        Vector3 R = new Vector3(DBN, 0.0f, 0.0f);
                        Right.transform.position += R;
                        CreatedNodes.Add(Right);
                        // Create a list up above (TotalNodesCreated) and check current node to Current Node list
                        NodeListSize = TotalNodesCreated.Count;
                        // Destroys Current Node If There Are Exsisting Nodes There
                        for (int F = 0; F < NodeListSize; F++)
                        {
                            if (Right.transform.position == TotalNodesCreated[F].transform.position)
                            {
                                CreatedNodes.Remove(Right);
                                Destroy(Right);
                            }
                        }
                        // Destroys Current Node If In Contact Of End Node, Or Wall
                        Collider[] HitCollidersRight = Physics.OverlapSphere(Right.transform.position, DBN / 2);
                        foreach (Collider Hit in HitCollidersRight)
                        {
                            if (Hit.tag == "End Node")
                            {
                                IsEndNodeLocated = true;
                                IsThereAValidPath = true;
                                break;
                            }
                            if (Hit.tag == "Wall")
                            {
                                CreatedNodes.Remove(Right);
                                Destroy(Right);
                            }
                        }
                        if (Right != null)
                        {
                            TotalNodesCreated.Add(Right);
                        }
                    }

                    if (IsEndNodeLocated != true & IsThereAValidPath != true)
                    {
                        // Up
                        GameObject Up = Instantiate(NL[S]);
                        Up.name = NodeValue.ToString();
                        Up.tag = "Node";
                        Vector3 U = new Vector3(0.0f, 0.0f, DBN);
                        Up.transform.position += U;
                        CreatedNodes.Add(Up);
                        // Create a list up above (TotalNodesCreated) and check current node to Current Node list
                        NodeListSize = TotalNodesCreated.Count;
                        // Destroys Current Node If There Are Exsisting Nodes There
                        for (int A = 0; A < NodeListSize; A++)
                        {
                            if (Up.transform.position == TotalNodesCreated[A].transform.position)
                            {
                                CreatedNodes.Remove(Up);
                                Destroy(Up);
                            }
                        }
                        // Destroys Current Node If In Contact Of End Node, Or Wall
                        Collider[] HitCollidersUp = Physics.OverlapSphere(Up.transform.position, DBN / 2);
                        foreach (Collider Hit in HitCollidersUp)
                        {
                            if (Hit.tag == "End Node")
                            {
                                IsEndNodeLocated = true;
                                IsThereAValidPath = true;
                                break;
                            }
                            if (Hit.tag == "Wall")
                            {
                                CreatedNodes.Remove(Up);
                                Destroy(Up);
                            }
                        }
                        if (Up != null)
                        {
                            TotalNodesCreated.Add(Up);
                        }
                        NodeValue--;
                    }
                }
            }
            NLSize = NL.Count;
            NodeListSize = CreatedNodes.Count;
            if (NodeListSize == 0)
            {
                IsThereAValidPath = false;
                NodeListSize = TotalNodesCreated.Count;
                for (int Stefanie = 1; Stefanie < NodeListSize; Stefanie++)
                {
                    Destroy(TotalNodesCreated[Stefanie]);
                }
                Destroy(EndingNode);
                IsEndingPointSet = false;
                NodeList.Clear();
                NodeValue = 0;
                TotalNodesCreated.Clear();
                NodeList.Add(Node);
                TotalNodesCreated.Add(Node);
                return;
            }

            // In Node List we would remove all the numbers in the list and now, all of the gameobjects in the CreatedNodes list will now populate the NodesList instead.
            NL.Clear();

            for (int I = 0; I < NodeListSize; I++)
            {
                NL.Add(CreatedNodes[I]);
            }
            NodeValue++;
        }
    }

    public GameObject NodesInCurrentArea(GameObject CurrentNodeSelected, float DBN)
    {
        // If all else fails, change from while, to if.
        while (HasShortestPathBeenFound == false)
        {
            // Has Current Node Selected Been Changed = HCNSBC
            bool HCNSBC = false;
            // Positions in the Current Area = PITCA
            int PITCA = 0;
            // Start off with the Ending Node
            CurrentNodeSelected.name = NodeValue.ToString();
            List<GameObject> PositionsInTheCurrentArea = new List<GameObject>();

            Collider[] NodesInTheCurrentArea = Physics.OverlapSphere(CurrentNodeSelected.transform.position, DBN);
            foreach (Collider Hit in NodesInTheCurrentArea)
            {
                // If the Current Node detects a Node(with a tag(Node)) that is within it's radius, and if it's value is one less than Current Node's value, that Node...
                // would get added onto the NodesInTheCurrentArea list.
                if (Hit.tag == "Starting Node")
                {
                    HasShortestPathBeenFound = true;
                    StartingPointCoordinates = Hit.gameObject.transform.position;
                }
                else if (Hit.tag != "Node")
                {
                    continue;
                }
                else if (Hit.tag == "Node" & int.Parse(Hit.name) == NodeValue - 1)
                {
                    PositionsInTheCurrentArea.Add(Hit.gameObject);
                }
            }

            PITCA = PositionsInTheCurrentArea.Count;
            // In NodePositions Compare the current node vs the next node
            for (int S = 0; S < PITCA; S++)
            {
                float CurrentNodePosition = Vector3.Distance(CurrentNodeSelected.transform.position, StartingPointCoordinates);
                float PositionOfNode = Vector3.Distance(PositionsInTheCurrentArea[S].transform.position, StartingPointCoordinates);
                if (CurrentNodePosition < PositionOfNode)
                {
                    continue;
                }
                else
                {
                    CurrentNodeSelected = PositionsInTheCurrentArea[S];
                    HCNSBC = true;
                }
            }

            // If Current Node Selected has not been changed, then set HCNSBC to false and if HCNSBC is false, the first Node in the PITCA list would be set as the Current Node.
            if (HCNSBC == false)
            {
                CurrentNodeSelected = PositionsInTheCurrentArea[0];
            }
            PositionsInTheCurrentArea.Clear();
            ShortestPath.Add(CurrentNodeSelected);
            ShortestPath.Add(CurrentNodeSelected);
            ShortestPath.Reverse();
            TotalNodesCreated.Remove(CurrentNodeSelected);
            NodeValue--;

            CurrentNodeSelected.GetComponent<Renderer>().material.color = Color.blue;
        }
        return CurrentNodeSelected;
    }

    public void SeekBehavior(List<GameObject> Targets, float DBN)
    {
        NodeListSize = ShortestPath.Count - 1;

        if (Targets[CPITL] == null)
        {
            return;
        }

        if (CPITL == NodeListSize)
        {
            IsGoingForwards = false;
        }
        else if (CPITL <= 0)
        {
            IsGoingForwards = true;
        }

        // Calculate a vector from the agent to it's target
        Vector3 Vector3Target = Targets[CPITL].transform.position;
        Vector3 Vector3Agent = this.gameObject.GetComponent<Transform>().position;
        Vector3 V3TargetSubtractAgent = Vector3Target - Vector3Agent;
        Vector3 Normalise = NormalizeVector3(V3TargetSubtractAgent);
        // Scale the vector by our maximum velocity (scalar)
        Vector3 CalculatedVector3 = Scalar(Normalise, MaximumVelocity);
        // Subtract agent's current velocity (vector) from vector to obtain force required to change agent's direction towards it's target
        Force = CalculatedVector3 - CurrentVelocity;
        // Apply force to agent's velocity
        Velocity += Force * Time.fixedDeltaTime;
        // Update agent's position
        this.gameObject.transform.position += Velocity * Time.fixedDeltaTime;
        //this.gameObject.GetComponent<Rigidbody>().position += Velocity * Time.fixedDeltaTime;

        // Hit Colliders = HC
        Collider[] HC = Physics.OverlapSphere(transform.position, DBN / 2);
        foreach (Collider Hit in HC)
        {
            if (Hit.tag == "End Node")
            {
                return;
            }
            else if (Hit.tag != "Node" & Hit.tag != "Starting Node")
            {
                continue;
            }
            else if (int.Parse(Hit.name) == int.Parse(Targets[CPITL].name))
            {
                if (IsGoingForwards == true)
                {
                    CPITL++;
                }
                else
                {
                    CPITL--;
                }
            }
        }
    }

    private float Vector3Magnitude(Vector3 V3)
    {
        float Vector3X = V3.x * V3.x;
        float Vector3Y = V3.y * V3.y;
        float Vector3Z = V3.z * V3.z;
        float Vector3XYZ = Vector3X + Vector3Y + Vector3Z;
        float Magnitude = Mathf.Sqrt(Vector3XYZ);
        return Magnitude;
    }

    private Vector3 NormalizeVector3(Vector3 V3)
    {
        float Mag = Vector3Magnitude(V3);
        V3.x /= Mag;
        V3.y /= Mag;
        V3.z /= Mag;
        return V3;
    }

    private Vector3 Scalar(Vector3 LHS, float RHS)
    {
        LHS = new Vector3(LHS.x * RHS, LHS.y * RHS, LHS.z * RHS);
        return LHS;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DijkstrasAlgorithm))]
public class DijkstrasAlgorithmCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DijkstrasAlgorithm DS = (DijkstrasAlgorithm)target;

        // Once set, the user will check the box (IsStartingPointSet). If true, then draw a sphere at that position
        DS.IsStartingPointSet = EditorGUILayout.Toggle("IsStartingPointSet", DS.IsStartingPointSet);
    }
}
#endif