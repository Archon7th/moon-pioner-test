using UnityEngine;

namespace Extensions
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// Flattens the vector onto the XZ plane by setting the Y component to zero.
        /// </summary>
        public static Vector3 Flat(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }
    }
}