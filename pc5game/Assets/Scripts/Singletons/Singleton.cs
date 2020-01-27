using UnityEngine;
/*
 * Inherit from this base class to create a singleton.
 * e.g. public class MyClassName : Singleton<MyClassName> {}
 * 
 * Does NOT implement DontDestroyOnLoad() by default, so add it to each 
 * singleton type if desired.
 */
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;

    protected virtual void Awake()
    {
        /* Check if there is an old instance (for example, carried over from
         * a previous scene due to DontDestroyOnLoad()). If so, destroy 
         * this new instance so we can continue using the old one. */
        if (m_Instance != null)
        {
            Destroy(gameObject);
        }

        /* Otherwise, set up this newly created instance. */
        else
        {
            m_Instance = gameObject.GetComponent<T>();

            ///* Make instance persistent. */
            //DontDestroyOnLoad(m_Instance.gameObject);
        }
    }
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                /* Search for existing instance. */
                m_Instance = (T)FindObjectOfType(typeof(T));

                /* Create new instance if one doesn't already exist. */
                if (m_Instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    m_Instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString() + " (Singleton)";
                }

                ///* Make instance persistent. */
                //DontDestroyOnLoad(m_Instance.gameObject);
            }

            return m_Instance;
        }
    }
}