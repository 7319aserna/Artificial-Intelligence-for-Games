using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    // FleeBehavior (FB)
    FB,
    // SeekBehavior (SB)
    SB,
    // WanderingBehavior(WB)
    WB,
    // Fleeing and Seeking Behavior (FSB)
    FSB,
    // Fleeing and Wandering Behavior (FWB)
    FWB,
    // Seeking and Wandering Behavior (SWB)
    SWB,
    // Fleeing, Seeking, and Wandering Behavior (FSWB)
    FSWB
}

public class SteeringBehaviors : MonoBehaviour
{
    private bool IsCurrentlyWandering;
    private bool IsCurrentlyFleeing;
    private bool IsCurrentlySeeking;

    private float WanderingBehaviorTimer = 0.0f;

    private GameObject WanderingPositionGameObject;
    // WBPosition: Creates a target that A.I.(Wander) will follow
    private GameObject WBPosition;

    private new Rigidbody rigidbody;

    private Vector3 CurrentVelocity;
    private Vector3 Force;
    private Vector3 Velocity;
    private Vector3 WanderingPosition;

    [HideInInspector]
    public bool IsSeekBehaviorEnabled;
    [HideInInspector]
    public bool IsFleeBehaviorEnabled;
    [HideInInspector]
    public bool IsWanderBehaviorEnabled;

    public float MaximumVelocity;

    [HideInInspector]
    public GameObject AreaBoundaries;
    [HideInInspector]
    public GameObject FleeTarget;
    [HideInInspector]
    public GameObject SeekTarget;

    [HideInInspector]
    public State CurrentState;

    // DELETE ALL BELOW THIS LATER
    bool m_Started;
    // DELETE ALL ABOVE THIS LATER

    void Start()
    {
        IsCurrentlyWandering = false;
        IsCurrentlyFleeing = false;
        IsCurrentlySeeking = false;
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (IsWanderBehaviorEnabled == true)
        {
            WBPosition = new GameObject("WB");
        }
        // DELETE ALL BELOW THIS LATER
        // Use this to ensure that the Gizmos are being drawn when in Play Mode.
        m_Started = true;
        // DELETE ALL ABOVE THIS LATER
    }

    void Update()
    {
        CurrentVelocity = Velocity;

        switch (CurrentState)
        {
            case State.FB:
                FleeBehavior(FleeTarget);
                break;
            case State.SB:
                SeekBehavior(SeekTarget);
                break;
            case State.WB:
                WanderBehavior(AreaBoundaries);
                break;
            case State.FSB:
                FleeBehavior(FleeTarget);
                SeekBehavior(SeekTarget);
                break;
            case State.FWB:
                FleeBehavior(FleeTarget);
                WanderBehavior(AreaBoundaries);
                break;
            case State.SWB:
                SeekBehavior(SeekTarget);
                WanderBehavior(AreaBoundaries);
                break;
            case State.FSWB:
                FleeBehavior(FleeTarget);
                SeekBehavior(SeekTarget);
                WanderBehavior(AreaBoundaries);
                break;
            default:
                Debug.LogError("Invalid state!");
                break;
        }

        // If the Target comes into contact with the A.I. Agent's (Flee) Range
        if (IsFleeBehaviorEnabled == true)
        {
            CurrentState = State.FB;
        }

        // If the Target comes into contact with the A.I. Agent's (Seek) Range
        if (IsSeekBehaviorEnabled == true)
        {
            CurrentState = State.SB;
        }

        // If the A.I. Agent is currently wandering
        if (IsWanderBehaviorEnabled == true)
        {
            CurrentState = State.WB;
        }

        // If the Target comes into contact with the A.I. Agent's (Flee) Range and/or comes into contact with the A.I. Agent's (Seek) Range
        if (IsFleeBehaviorEnabled == true & IsSeekBehaviorEnabled == true)
        {
            CurrentState = State.FSB;
        }

        // If the Target comes into contact with the A.I. Agent's (Flee) Range and/or the A.I. Agent is currently wandering
        if (IsFleeBehaviorEnabled == true & IsWanderBehaviorEnabled == true)
        {
            CurrentState = State.FWB;
        }

        // If the Target comes into contact with the A.I. Agent's (Seek) Range and/or the A.I. Agent is currently wandering
        if (IsSeekBehaviorEnabled == true & IsWanderBehaviorEnabled == true)
        {
            CurrentState = State.SWB;
        }

        // If the Target comes into contact with the A.I. Agent's (Flee) Range and/or comes into contact with the A.I. Agent's (Seek) Range and/or the A.I. Agent is currently wandering
        if (IsFleeBehaviorEnabled == true & IsSeekBehaviorEnabled & IsWanderBehaviorEnabled == true)
        {
            CurrentState = State.FSWB;
        }
    }

    // DELETE ALL BELOW THIS LATER
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (m_Started)
        {
            Gizmos.DrawWireCube(transform.position, transform.localScale * 10);
        }
    }
    // DELETE ALL ABOVE THIS LATER

    private void FleeBehavior(GameObject Target)
    {
        Collider[] FleeHitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 10);
        int i = 0;
        foreach (Collider Hit in FleeHitColliders)
        {
            if (Hit.gameObject == Target)
            {
                IsCurrentlyFleeing = true;
                // Calculate a vector from the agent to it's target
                Vector3 Vector3Target = Target.GetComponent<Transform>().position;
                Vector3 Vector3Agent = this.gameObject.GetComponent<Transform>().position;
                // Calculate vector away from target
                Vector3 V3AgentSubtractTarget = Vector3Agent - Vector3Target;
                Vector3 Normalise = NormalizeVector3(V3AgentSubtractTarget);
                // Scale the vector by our maximum velocity (scalar)
                Vector3 CalculatedVector3 = Scalar(Normalise, MaximumVelocity);
                // Subtract agent's current velocity (vector) from vector to obtain force required to change agent's direction towards it's target
                Force = CalculatedVector3 - CurrentVelocity;
                // Apply force to agent's velocity
                Velocity += Force * Time.fixedDeltaTime;
                // Update agent's position
                rigidbody.position += Velocity * Time.fixedDeltaTime;
            }
            else
            {
                IsCurrentlyFleeing = false;
            }
            i++;
        }
    }

    private void SeekBehavior(GameObject Target)
    {
        Collider[] SeekHitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * 10);
        int i = 0;
        foreach (Collider Hit in SeekHitColliders)
        {
            if (Hit.gameObject == Target)
            {
                Debug.Log(Hit.gameObject.name);
                IsCurrentlySeeking = true;
                // Calculate a vector from the agent to it's target
                Vector3 Vector3Target = Target.GetComponent<Transform>().position;
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
                rigidbody.position += Velocity * Time.fixedDeltaTime;
            }
            else
            {
                IsCurrentlySeeking = false;
            }
            i++;
        }
    }

    private void WanderBehavior(GameObject WanderingBoundaries)
    {
        if (IsCurrentlyFleeing == false & IsCurrentlySeeking == false)
        {
            if (IsCurrentlyWandering == false)
            {
                Renderer wRenderer = WanderingBoundaries.GetComponent<Renderer>();
                float xRange = wRenderer.bounds.size.x / 2;
                float zRange = wRenderer.bounds.size.z / 2;
                WBPosition.transform.position = new Vector3(Random.Range(-xRange, xRange), 1.0f, Random.Range(-zRange, zRange));
                IsCurrentlyWandering = true;
            }
            else
            {
                WanderingBehaviorTimer += Time.deltaTime;
                // Calculate a vector from the agent to it's target
                Vector3 Vector3Target = WBPosition.transform.position;
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
                rigidbody.position += Velocity * Time.fixedDeltaTime;
                if (WanderingBehaviorTimer >= 10.0f)
                {
                    WanderingBehaviorTimer = 0.0f;
                    IsCurrentlyWandering = false;
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
[CustomEditor(typeof(SteeringBehaviors))]
public class SteeringBehaviorsCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SteeringBehaviors SB = (SteeringBehaviors)target;

        // Draw checkbox for the FleeBehavior bool
        SB.IsFleeBehaviorEnabled = EditorGUILayout.Toggle("IsFleeBehaviorEnabled", SB.IsFleeBehaviorEnabled);
        if (SB.IsFleeBehaviorEnabled)
        {
            SB.FleeTarget = EditorGUILayout.ObjectField("Target", SB.FleeTarget, typeof(GameObject), true) as GameObject;
        }

        // Draw checkbox for the SeekBehavior bool
        SB.IsSeekBehaviorEnabled = EditorGUILayout.Toggle("IsSeekBehaviorEnabled", SB.IsSeekBehaviorEnabled);
        if (SB.IsSeekBehaviorEnabled)
        {
            SB.SeekTarget = EditorGUILayout.ObjectField("Target", SB.SeekTarget, typeof(GameObject), true) as GameObject;
        }

        // Draw checkbox for the WanderBehavior bool
        SB.IsWanderBehaviorEnabled = EditorGUILayout.Toggle("IsWanderBehaviorEnabled", SB.IsWanderBehaviorEnabled);
        if (SB.IsWanderBehaviorEnabled)
        {
            SB.AreaBoundaries = EditorGUILayout.ObjectField("WanderingBoundaries", SB.AreaBoundaries, typeof(GameObject), true) as GameObject;
        }
    }
}
#endif