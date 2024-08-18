using UnityEngine;

public class HideAndLockCursor : MonoBehaviour
{

    public bool hideCursor = true;

    // Start is called before the first frame update
    void Start()
    {
        if(hideCursor)  
            Cursor.lockState = CursorLockMode.Locked;
    }

}
