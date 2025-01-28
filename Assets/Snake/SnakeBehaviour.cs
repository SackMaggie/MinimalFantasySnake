using UnityEngine;

namespace Snake
{
    /// <summary>
    /// My prefered way of handle Unity functions
    /// When child class inherit parent class that contain unity function
    /// Only the child class get invoked and the parent will not get invoked
    /// Unless accessing via base.XXXX() which will not consistant across the project since someone will declare private void Start() anyway
    /// </summary>
    public class SnakeBehaviour : MonoBehaviour
    {
        protected virtual void Start()
        {

        }

        protected virtual void Awake()
        {

        }
    }
}