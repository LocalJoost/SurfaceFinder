using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace LocalJoost.Examples
{
    public class ObjectPlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject objectToPlace;

        public void PlaceObject(MixedRealityPose pose)
        {
            var obj = Instantiate(objectToPlace, gameObject.transform);
            obj.transform.position = pose.Position;
            obj.transform.rotation *= pose.Rotation;
        }
    }
}