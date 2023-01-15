using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(NetworkObject))]
public class PingScript : NetworkBehaviour
{
    // For a value like RTT an exponential moving average is a better indication of the current rtt and fluctuates less.
    struct ExponentialMovingAverageCalculator
    {
        readonly float m_Alpha;
        float m_Average;

        public float Average => m_Average;

        public ExponentialMovingAverageCalculator(float average)
        {
            m_Alpha = 2f / (k_MaxWindowSize + 1);
            m_Average = average;
        }

        public float NextValue(float value) => m_Average = (value - m_Average) * m_Alpha + m_Average;
    }

    // RTT
    // Client sends a ping RPC to the server and starts it's timer.
    // The server receives the ping and sends a pong response to the client.
    // The client receives that pong response and stops its time.
    // The RPC value is using a moving average, so we don't have a value that moves too much, but is still reactive to RTT changes.

    const int k_MaxWindowSizeSeconds = 3; // it should take x seconds for the value to react to change
    const float k_PingIntervalSeconds = 0.1f;
    const float k_MaxWindowSize = k_MaxWindowSizeSeconds / k_PingIntervalSeconds;

    // Some games are less sensitive to latency than others. For fast-paced games, latency above 100ms becomes a challenge for players while for others 500ms is fine. It's up to you to establish those thresholds.
    const float k_StrugglingNetworkConditionsRTTThreshold = 120;
    const float k_BadNetworkConditionsRTTThreshold = 200;

    ExponentialMovingAverageCalculator m_RTT = new ExponentialMovingAverageCalculator(0);
    ExponentialMovingAverageCalculator m_UtpRTT = new ExponentialMovingAverageCalculator(0);

    float m_LastPingTime;
    [SerializeField] TextMeshProUGUI m_TextStat;

    // When receiving pong client RPCs, we need to know when the initiating ping sent it so we can calculate its individual RTT
    int m_CurrentRTTPingId;

    Dictionary<int, float> m_PingHistoryStartTimes = new Dictionary<int, float>();

    ClientRpcParams m_PongClientParams;

    [SerializeField] bool m_IsServer;

    string m_TextToDisplay;

    public override void OnNetworkSpawn()
    {
        m_IsServer = IsServer;
        bool isClientOnly = IsClient && !IsServer;
        if (!IsOwner && isClientOnly) // we don't want to track player ghost stats, only our own
        {
            enabled = false;
            return;
        }
        if (IsOwner)
        {
            m_TextStat = GameObject.FindGameObjectWithTag("UI PLAYER").GetComponent<TextMeshProUGUI>();
        }

        m_PongClientParams = new ClientRpcParams() { Send = new ClientRpcSendParams() { TargetClientIds = new[] { OwnerClientId } } };
    }

    void FixedUpdate()
    {
        if (!m_IsServer)
        {
            if (Time.realtimeSinceStartup - m_LastPingTime > k_PingIntervalSeconds)
            {
                // We could have had a ping/pong where the ping sends the pong and the pong sends the ping. Issue with this is the higher the latency, the lower the sampling would be. We need pings to be sent at a regular interval
                PingServerRPC(m_CurrentRTTPingId);
                m_PingHistoryStartTimes[m_CurrentRTTPingId] = Time.realtimeSinceStartup;
                m_CurrentRTTPingId++;
                m_LastPingTime = Time.realtimeSinceStartup;

                m_UtpRTT.NextValue(NetworkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.ServerClientId));
            }

            if (m_TextStat != null)
            {
                //m_TextToDisplay = $"RTT: {(m_RTT.Average * 1000).ToString("0")} ms;\nUTP RTT {m_UtpRTT.Average.ToString("0")} ms";
                m_TextToDisplay = $"Ping: {((m_RTT.Average * 1000) / 2).ToString("0")} ms";
                if (m_UtpRTT.Average > k_BadNetworkConditionsRTTThreshold)
                {
                    m_TextStat.color = Color.red;
                }
                else if (m_UtpRTT.Average > k_StrugglingNetworkConditionsRTTThreshold)
                {
                    m_TextStat.color = Color.yellow;
                }
                else
                {
                    m_TextStat.color = Color.white;
                }
            }
        }
        else
        {
            m_TextToDisplay = $"Ping: 0 ms";
        }
        if (m_TextStat)
        {
            m_TextStat.text = m_TextToDisplay;
        }
    }

    [ServerRpc]
    void PingServerRPC(int pingId, ServerRpcParams serverParams = default)
    {
        PongClientRPC(pingId, m_PongClientParams);
    }

    [ClientRpc]
    void PongClientRPC(int pingId, ClientRpcParams clientParams = default)
    {
        var startTime = m_PingHistoryStartTimes[pingId];
        m_PingHistoryStartTimes.Remove(pingId);
        m_RTT.NextValue(Time.realtimeSinceStartup - startTime);
    }
}