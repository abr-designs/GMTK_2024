using Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World;

public class WorldReplaceManager : MonoBehaviour, ICreateWorldReplacers {

    [Header("References")]
    [SerializeField] private List<string> worldTagList = new List<string>();
    [SerializeField] private Dictionary<string, List<GameObject>> dictionaryTagObjects = new Dictionary<string, List<GameObject>>();

    [Header("DEBUG")]
    [SerializeField] private string testWorldTag = "Tree";
    [SerializeField] private float testScale = 5f;
    [SerializeField] private GameObject testWorldObjectPrefab;
    
    [SerializeField] 
    private LayerMask layerMask;


    private void Start() {
        EmptyChildObjects();
        IdentifyWorldTags();
        IdentifyWorldTagObjects();
    }


    [ContextMenu("Identify World Tags")]
    private void Inspector_IdentifyWorldTags() {
        IdentifyWorldTags();
        Debug.Log($"Identified {worldTagList.Count} worldTags");
    }
    // identify world objects
    private void IdentifyWorldTags() {

        worldTagList.Clear();

        WorldTagObject[] worldTagObjectArray = FindObjectsOfType<WorldTagObject>();

        foreach(WorldTagObject worldTagObject in worldTagObjectArray) {
            string currentTag = worldTagObject.worldTag;
            if (!worldTagList.Contains(currentTag)) {
                worldTagList.Add(currentTag);
            }
        }

        worldTagList.Sort();
    }


    [ContextMenu("Identify World Tag Objects")]
    private void Inspector_IdentifyWorldTagObjects() {
        IdentifyWorldTagObjects();
        int dicationaryValues = dictionaryTagObjects.Values.Sum(list => list.Count); ;
        Debug.Log($"Dictionary updated with {dictionaryTagObjects.Keys.Count} keys and {dicationaryValues} values");
    }
    // identify world objects
    private void IdentifyWorldTagObjects() {

        dictionaryTagObjects.Clear();

        foreach (string tagString in worldTagList) {
            // get list of all objects with this tag
            WorldTagObject[] worldTagObjectArray = FindObjectsOfType<WorldTagObject>();
            List<GameObject> worldGameObjectList = new List<GameObject>();

            foreach(WorldTagObject worldTagObject in worldTagObjectArray) {
                if(worldTagObject.worldTag == tagString) {
                    worldGameObjectList.Add(worldTagObject.gameObject);
                }
            }

            dictionaryTagObjects.Add(tagString, worldGameObjectList);
        }
    }


    [ContextMenu("Spawn World Objects")]
    private void Inspector_SpawnWorldObjects() {
        CreateWorldVersion(testWorldTag, testScale, testWorldObjectPrefab);
        Debug.Log($"Spawned {dictionaryTagObjects[testWorldTag].Count} {testWorldTag}");
    }
    // spawn objects based on prefabs
    public void CreateWorldVersion(string worldTag, float scale, GameObject container)
    {
        //Layer Restrict the new Objects
        //------------------------------------------------//
        var gameObjects = container.GetComponentsInChildren<Transform>();
        
        int LayerIgnoreRaycast = LayerMask.NameToLayer("WorldTagScene");

        foreach (var o in gameObjects)
        {
            o.gameObject.layer = LayerIgnoreRaycast;// layerMask.value;
        }
        
        //------------------------------------------------//
        
        List<GameObject> worldObjectReference = dictionaryTagObjects[worldTag];

        foreach (GameObject worldObject in worldObjectReference) {

            GameObject newGameObject = Instantiate(container, worldObject.transform.position, worldObject.transform.rotation, transform);
            newGameObject.transform.localScale = Vector3.one * scale;
            
            

        }
        
        container.SetActive(false);
    }


    [ContextMenu("Destroy Transform Children")]
    private void Inspector_DestroyChildren() {
        EmptyChildObjects();
    }
    // remove spawned prefab objects from transform
    private void EmptyChildObjects() {

        List<GameObject> children = new List<GameObject>();

        foreach(Transform child in transform) {
            children.Add(child.gameObject);
        }

        for (int i = children.Count  -1; i >= 0; i -= 1) {
            DestroyImmediate(children[i]);
        }
    }

    [ContextMenu("Run All of the Above")]
    private void Inspector_RunAllFunctions() {
        EmptyChildObjects();
        IdentifyWorldTags();
        IdentifyWorldTagObjects();
        CreateWorldVersion(testWorldTag, testScale, testWorldObjectPrefab);
    }

}