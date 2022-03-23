using UnityEngine;
using SampleGame.Configuration;

namespace SampleGame.Gateway
{
    [CreateAssetMenu(
        fileName = "MultiplayerServiceConfiguration", 
        menuName = "SampleGame/Configuration/MultiplayerServiceConfiguration")]
    public class MultiplayerServiceConfiguration : MultiplayerServiceConfigurationBase
    {
        public string Address = "http://localhost:5000";
    }
}