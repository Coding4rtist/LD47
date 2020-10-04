using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {
	[System.Serializable]
	public class Pool {
		public string tag;
		public GameObject prefab;
		public int size;
	}

	public static PoolManager Instance;

	public List<Pool> pools;

	private Dictionary<string, Queue<ObjectInstance>> poolDictionary;

	private void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	private void Start() {
		poolDictionary = new Dictionary<string, Queue<ObjectInstance>>();
		foreach (Pool pool in pools) {
			AddPool(pool.tag, pool.prefab, pool.size);
		}
	}

	public void AddPool(string tag, GameObject prefab, int size) {
		if (poolDictionary.ContainsKey(tag)) {
			Debug.LogWarning("PoolManager already contains a pool with tag " + tag + ".");
			return;
		}

		Queue<ObjectInstance> objectPool = new Queue<ObjectInstance>();
		GameObject parent = new GameObject(tag + "_pool");
		parent.transform.SetParent(transform);

		for (int i = 0; i < size; i++) {
			GameObject obj = Instantiate(prefab);
			obj.SetActive(false);
			ObjectInstance newObject = new ObjectInstance(obj, parent);
			objectPool.Enqueue(newObject);
		}
		poolDictionary.Add(tag, objectPool);
	}

	public GameObject SpawnObject(string tag, Vector3 position, Quaternion rotation) {
		if(!poolDictionary.ContainsKey(tag)) {
			Debug.LogWarning("Pool with tag " + tag + " doens't exist.");
			return null;
		}

		ObjectInstance objectToSpawn = poolDictionary[tag].Dequeue();
		objectToSpawn.Reuse(position, rotation);
		poolDictionary[tag].Enqueue(objectToSpawn);

		return objectToSpawn.gameObject;
	}

	public class ObjectInstance {
		public GameObject gameObject;

		Transform transform;
		bool hasPoolObjectComponent;
		PoolObject poolObjectScript;

		public ObjectInstance(GameObject obj,GameObject parent){
			gameObject = obj;
			transform = obj.transform;
			transform.SetParent(parent.transform,false);
			gameObject.SetActive(false);

			poolObjectScript = gameObject.GetComponent<PoolObject>();
			hasPoolObjectComponent = poolObjectScript != null;
		}

		public void Reuse(Vector3 position, Quaternion rotation){
			if(hasPoolObjectComponent){
				poolObjectScript.OnObjectReuse();
			}
			
         transform.position = position;
         transform.rotation = rotation;
			//if (gameObject.activeSelf) gameObject.SetActive(false);
			gameObject.SetActive(true);
		}
	}
}
