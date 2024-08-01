using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using UnityEngine;

public class AWSAuth : MonoBehaviour
{
    private string userPoolId;
    private string clientId;
    private RegionEndpoint region;

    private AmazonCognitoIdentityProviderClient provider;
    private CognitoUserPool userPool;
    private CognitoUser user;
    private string idToken;

    void Start()
    {
        LoadConfig();
        provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), region);
        userPool = new CognitoUserPool(userPoolId, clientId, provider);
    }

    private void LoadConfig()
    {
        TextAsset configText = Resources.Load<TextAsset>("config");
        Config config = JsonUtility.FromJson<Config>(configText.text);

        userPoolId = config.UserPoolId;
        clientId = config.ClientId;
        region = RegionEndpoint.GetBySystemName(config.Region);
    }

    public void SignIn(string username, string password)
    {
        user = new CognitoUser(username, clientId, userPool, provider);
        InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
        {
            Password = password
        };

        user.StartWithSrpAuthAsync(authRequest).ContinueWith(authResult =>
        {
            if (authResult.Exception == null)
            {
                Debug.Log("User authenticated successfully");
                idToken = user.SessionTokens.IdToken;
                // Now you can use the authenticated user
            }
            else
            {
                Debug.LogError("Authentication failed: " + authResult.Exception.Message);
            }
        });
    }

    public string GetIdToken()
    {
        return idToken;
    }
}
