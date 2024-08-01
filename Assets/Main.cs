using UnityEngine;

public class Main : MonoBehaviour
{
    private AWSAuth awsAuth;
    private AppSyncClient appSyncClient;

    void Start()
    {
        awsAuth = GetComponent<AWSAuth>();
        appSyncClient = GetComponent<AppSyncClient>();

        awsAuth.SignIn("your_username", "your_password");
    }

    public void CreateContractExample()
    {
        string tenantId = "test"; // from your configuration
        string contractId = "your_contract_id";
        string contractDataJson = "{\"key\":\"value\"}";

        appSyncClient.CreateContract(tenantId, contractId, contractDataJson);
    }

    public void GetContractExample()
    {
        string tenantId = "test"; // from your configuration
        string contractId = "your_contract_id";

        appSyncClient.GetContract(tenantId, contractId);
    }
}
