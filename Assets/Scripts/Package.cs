using UnityEngine;
public enum PackageType
{
    Regular,
    Heavy,
    Fragile
}
public class Package : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public PackageType packageType;
    public int weight;
    public int value;

    public bool CanBeThrown()
    {
        return packageType == PackageType.Regular || packageType == PackageType.Fragile;
    }


}
