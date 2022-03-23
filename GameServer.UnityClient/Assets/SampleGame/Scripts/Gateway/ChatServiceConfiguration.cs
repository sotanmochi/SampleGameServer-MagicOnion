using UnityEngine;
using SampleGame.Configuration;

namespace SampleGame.Gateway
{
    [CreateAssetMenu(
        fileName = "ChatServiceConfiguration", 
        menuName = "SampleGame/Configuration/ChatServiceConfiguration")]
    public class ChatServiceConfiguration : ChatServiceConfigurationBase
    {
        public string Address = "http://localhost:5000";
    }
}