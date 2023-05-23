using UnityEngine;

public static class Utility
{
    public static float Remap(float v, float iMin, float iMax, float oMin, float oMax)
    {
        return Mathf.Lerp(oMin, oMax, Mathf.InverseLerp(iMin, iMax, v));
    }

    public static void IgnoreCollision(GameObject a, GameObject b, bool ignore)
    {
        var acl = a.GetComponentsInChildren<Collider>(true);
        var bcl = b.GetComponentsInChildren<Collider>(true);

        foreach (var ac in acl)
        foreach (var bc in bcl)
        {
            Physics.IgnoreCollision(ac, bc, ignore);
        }
    }
}