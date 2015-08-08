using UnityEngine;

public abstract class Screen : MonoBehaviour {
    /**
 * @class   GameController
 *
 * @brief   This abstract class is the base for all screens that we use in the game.
 *          
 * @details Each derivitive should probably only exist once (exceptions include common dialog screens)
 *
 * @author  Peter
 * @date    07/08/2015
 */

    public bool orthographic;
    public Transform StartingTransform;
    public Vector3 startingRotation;

    private Camera camera;
    private Vector3 screenTopRight;
    private Vector3 screenBottomLeft;

    public virtual void Start() {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        camera.transform.position = StartingTransform.position;
        camera.transform.rotation = orthographic ?  Quaternion.Euler(0,0,0) : Quaternion.Euler(startingRotation);
    }
    public abstract void Update();
}
