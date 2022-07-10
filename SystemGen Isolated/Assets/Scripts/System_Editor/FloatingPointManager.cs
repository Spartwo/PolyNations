using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;



public class FloatingPointManager : MonoBehaviour
{
    public float Threshold = 500.0f; //0.5AU
    void LateUpdate()
    {
        Vector3 CameraPosition = this.transform.position;

        //triggres if any postition of the camera in x,y,z extends beyond the threshold
        if (CameraPosition.magnitude > Threshold)
        {

            for (int z = 0; z < SceneManager.sceneCount; z++)
            {
                //find every gameobject with no parent
                foreach (GameObject g in SceneManager.GetSceneAt(z).GetRootGameObjects())
                {
                    //move n accordance with the camera position
                    g.transform.position -= CameraPosition;
                }
            }
            Debug.Log("recentering origin" );
        }

    }
}