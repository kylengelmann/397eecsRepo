using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyorBelt : MonoBehaviour {

    [Tooltip("The velocity the belt moves at.")]
    public float velocity;

    [Tooltip("The velocity scale of the belt texture.")]
    public float beltVelocityScale;

    [Tooltip("The material of the belt. Should have a field named Offset for moving the belt texture.")]
    public Renderer beltRenderer;
    //Material beltMaterial;

    [Tooltip("The maximum distance and object can be off the belt to still be moved by it.")]
    public float maxMoveHeight;

    // The offset of the belt texture.
    Vector2 beltOffset = Vector4.zero;

    // The belt's collider
    BoxCollider box;

    // For storing the colliders on the belt;
    Collider[] onBelt;

    void Start() {
        // Get the belt's collider
        box = gameObject.GetComponent<BoxCollider>();

        // Initialize the array so OverlaoBox can put stuff in it
        onBelt = new Collider[16];

    }

	
	void Update () {
        // Move the belt texture to give the appearence of a moving belt
        beltOffset.x += velocity*beltVelocityScale*Time.deltaTime;
		//beltMaterial.SetFloat("Offset", beltOffset);

        beltRenderer.material.SetTextureOffset("_MainTex", beltOffset);
	}


    void FixedUpdate()
    {
        // Get all of the objects on the belt with an OverlapBox

        // Set the overlap box's center
        Vector3 center = box.center + Vector3.up*(box.size.z/2f + maxMoveHeight/2f);

        // For some reason you can't just multiply Vectors?
        center.x *= transform.lossyScale.x;
        center.y *= transform.lossyScale.y;
        center.z *= transform.lossyScale.z;

        center += transform.position;

        // Set the overlap box's half extents
        Vector3 halfExtents = box.size/2f;
        halfExtents.y = maxMoveHeight/2f;

        halfExtents.x *= transform.lossyScale.x;
        halfExtents.z *= transform.lossyScale.z;


        // Overlap box to get objects on the belt
        int numOnBelt = Physics.OverlapBoxNonAlloc(center, halfExtents, onBelt, transform.rotation);

        // Find the ones that can be moved by the belt and move them
        for(int i = 0; i < numOnBelt; i++) {
            velocityTaker velTake;
            if((velTake = onBelt[i].gameObject.GetComponent<velocityTaker>()) != null) {
                velTake.velocity = transform.forward*velocity;
                velTake.isOnGround = true;
            }
        }
    }
}
