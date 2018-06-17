using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FileManager : MonoBehaviour
{
    public int n;
    public GameObject filesListPan, filesContent, filesPrefab, Camera, Drivers , objMarkerless, test;
    public RawImage fileImg;
    public GameObject IdDisplayModel = null;
    public GameObject[] sideUI;
    public GameObject tracker;
    private GameObject[] instancedObjs, objectArray;
    public static FileManager instance;
    public static GameObject model;

    private void Awake()
    {
        instance = this;
    }

	void Start ()
    {
	}

    public void LoadFilesList()
    {        
        StartCoroutine("InstantiateObject");
    }

    IEnumerator InstantiateObject()
    {
        string uri = "file:///mnt/sdcard/DLC/kitchen";
        UnityWebRequest request = UnityWebRequest.GetAssetBundle(uri, 0);
        yield return request.SendWebRequest();
        try
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
            objectArray = bundle.LoadAllAssets<GameObject>() as GameObject[];
        }
        catch
        {
        }

        foreach (GameObject obj in sideUI)
            obj.SetActive(false);

        filesListPan.SetActive(true);
        instancedObjs = new GameObject[objectArray.Length];
        for (int i = 0; i < objectArray.Length; i++)
        {
            FileScript file = Instantiate(filesPrefab, filesContent.transform).GetComponent<FileScript>();
            file.fileNameText.text = objectArray[i].name;
            file.index = i;
            instancedObjs[i] = file.gameObject;
        }
    }

    public void SelectFile(int index)
    {
        objectArray[index].transform.localScale = new Vector3(10, 10, 10);
        if (IdDisplayModel)
            Destroy(IdDisplayModel);
        IdDisplayModel =Instantiate(objectArray[index], objMarkerless.transform);
        filesListPan.SetActive(false);
        foreach (GameObject obj in sideUI)
            obj.SetActive(true);
        foreach (GameObject obj in instancedObjs)
            Destroy(obj);  
    }
}
