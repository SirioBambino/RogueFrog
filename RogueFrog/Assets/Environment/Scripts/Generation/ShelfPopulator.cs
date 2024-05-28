using UnityEngine;

// Class that adds random objects to shelves across the level
namespace RogueFrog.Environment.Scripts.Generation
{
    public class ShelfPopulator : MonoBehaviour
    {
        public static void PopulateShelves(GameObject roomObjects, GameObject[] shelfPropsPrefabs, LevelParametersSO levelParameters)
        {
            foreach (Transform child in roomObjects.transform)
            {
                if (child.name == "ShelfTall(Clone)")
                {
                    // Iterate through the shelves
                    for (int i = 0; i < 5; i++)
                    {
                        // Spawn a big prop
                        if (Random.Range(0.0f, 1.0f) > 0.8f)
                        {
                            if (Random.Range(0.0f, 1.0f) > 0.1f)
                                InstantiateShelfProp(shelfPropsPrefabs[Random.Range(shelfPropsPrefabs.Length - 3, shelfPropsPrefabs.Length)],
                                    child, new Vector3(Random.Range(-0.1f, -0.2f), (0.65f + i) / levelParameters.levelScale, 0),
                                    new Vector3(90, Random.Range(-30, 30), 0));
                        }
                        // Spawn a small prop
                        else
                        {
                            if (Random.Range(0.0f, 1.0f) > 0.1f)
                                InstantiateShelfProp(shelfPropsPrefabs[Random.Range(0, shelfPropsPrefabs.Length - 3)],
                                    child, new Vector3(Random.Range(-0.1f, -0.25f), (0.65f + i) / levelParameters.levelScale, 0),
                                    new Vector3(0, Random.Range(0, 360), 0));
                            if (Random.Range(0.0f, 1.0f) > 0.1f)
                                InstantiateShelfProp(shelfPropsPrefabs[Random.Range(0, shelfPropsPrefabs.Length - 3)],
                                    child, new Vector3(Random.Range(0.1f, 0.25f), (0.65f + i) / levelParameters.levelScale, 0),
                                    new Vector3(0, Random.Range(0, 360), 0));
                        }
                    }
                }
                else if (child.name == "ShelfSmall(Clone)")
                {
                    // Iterate through the shelves
                    for (int i = 0; i < 4; i++)
                    {
                        // Spawn a big prop
                        if (Random.Range(0.0f, 1.0f) > 0.8f)
                        {
                            if (Random.Range(0.0f, 1.0f) > 0.1f)
                                InstantiateShelfProp(shelfPropsPrefabs[Random.Range(shelfPropsPrefabs.Length - 3, shelfPropsPrefabs.Length)],
                                    child, new Vector3(Random.Range(-0.1f, -0.2f), (0.65f + i) / levelParameters.levelScale, 0),
                                    new Vector3(90, Random.Range(-30, 30), 0));
                        }
                        // Spawn a small prop
                        else
                        {
                            if (Random.Range(0.0f, 1.0f) > 0.1f)
                                InstantiateShelfProp(shelfPropsPrefabs[Random.Range(0, shelfPropsPrefabs.Length - 3)],
                                    child, new Vector3(Random.Range(-0.1f, -0.4f), (0.65f + i) / levelParameters.levelScale, 0),
                                    new Vector3(0, Random.Range(0, 360), 0));
                            if (Random.Range(0.0f, 1.0f) > 0.1f)
                                InstantiateShelfProp(shelfPropsPrefabs[Random.Range(0, shelfPropsPrefabs.Length - 3)],
                                    child, new Vector3(Random.Range(0.1f, 0.4f), (0.65f + i) / levelParameters.levelScale, 0),
                                    new Vector3(0, Random.Range(0, 360), 0));
                        }
                    }
                }
            }
        }

        private static void InstantiateShelfProp(GameObject prop, Transform child, Vector3 positionOffset, Vector3 rotationOffset)
        {
            Vector3 pOffset = positionOffset;
            Quaternion rOffset = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);
            GameObject shelfProp = Instantiate(prop, child.transform.position, child.transform.rotation * rOffset, child.transform);
            shelfProp.transform.localPosition = pOffset;
        }
    }
}
