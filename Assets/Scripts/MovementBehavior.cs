using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehavior : MonoBehaviour {
    public float MovementSpeed;
    public float RotationSpeed;

	void Update () {
        float MoveHorizontal = Input.GetAxis("Horizontal");
        float MoveVertical = Input.GetAxis("Vertical");

        Vector3 Movement = new Vector3(MoveHorizontal, 0.0f, MoveVertical);

        transform.Translate(Movement * MovementSpeed * Time.deltaTime);
            
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0.0f, -RotationSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0.0f, RotationSpeed * Time.deltaTime, 0.0f);
        }
        // Sphere Only
        //GetComponent<Rigidbody>().AddForce(Movement * MovementSpeed * Time.deltaTime);
    }
}
