using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AppSyncClient : MonoBehaviour
{
    private string apiUrl;
    private AWSAuth awsAuth;

    void Start()
    {
        LoadConfig();
        awsAuth = GetComponent<AWSAuth>();
    }

    private void LoadConfig()
    {
        TextAsset configText = Resources.Load<TextAsset>("config");
        Config config = JsonUtility.FromJson<Config>(configText.text);

        apiUrl = config.AppSyncUrl;
    }

    public void CreateContract(string tenantId, string contractId, string contractDataJson)
    {
        string mutation = @"
        mutation CreateContract($input: EntityInput!) {
            createEntity(input: $input) {
                id
                data
            }
        }";

        var variables = new
        {
            input = new
            {
                tenantId = tenantId,
                entityName = "CONTRACT",
                id = contractId,
                data = contractDataJson
            }
        };

        StartCoroutine(PostRequest(apiUrl, mutation, variables));
    }

    public void GetContract(string tenantId, string contractId)
    {
        string query = @"
        query ListContract($tenantId: String!, $entityName: String!, $id: String!) {
            listEntity(tenantId: $tenantId, entityName: $entityName, id: $id) {
                id
                data
            }
        }";

        var variables = new
        {
            tenantId = tenantId,
            entityName = "CONTRACT",
            id = contractId
        };

        StartCoroutine(PostRequest(apiUrl, query, variables));
    }

    private IEnumerator PostRequest(string url, string query, object variables)
    {
        var request = new
        {
            query = query,
            variables = variables
        };

        string jsonRequest = JsonUtility.ToJson(request);
        UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", awsAuth.GetIdToken());

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + webRequest.error);
        }
        else
        {
            Debug.Log("Received: " + webRequest.downloadHandler.text);
        }
    }
}
