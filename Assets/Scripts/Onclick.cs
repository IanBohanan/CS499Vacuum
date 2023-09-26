using UnityEngine;

public class DeleteObjectOnClick : MonoBehaviour
{
    // Public variable to hold the reference to the GameObject you want to delete.
    public GameObject Vacuum;

    // Public method that will be called when the button is clicked to delete the object.
    public void DeleteObject()
    {
        // Check if the 'Vacuum' reference is not null (it exists).
        if (Vacuum != null)
        {
            // If the object exists, destroy it.
            Destroy(Vacuum);
        }
        else
        {
            // If 'Vacuum' is not assigned (null), log a warning message.
            Debug.LogWarning("Object to delete is not assigned!");
        }
    }
}
