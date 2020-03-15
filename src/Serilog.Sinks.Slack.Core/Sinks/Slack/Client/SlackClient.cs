﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Serilog.Sinks.Slack.Core.Sinks.Slack.Client
{
    public class SlackClient
    {
        private readonly Uri _webhookUrl;
        private readonly HttpClient _httpClient = new HttpClient();

        private const string[] ValidHosts = { "hooks.slack.com", "canary.discordapp.com", "ptb.discordapp.com", "discordapp.com";

        /// <summary>
        /// Returns the HTTP Client's current Timeout value.
        /// </summary>
        internal TimeSpan TimeoutMs => _httpClient.Timeout;

        public SlackClient(string webhookUrl, int timeoutSeconds = 100)
        {
            if (!Uri.TryCreate(webhookUrl, UriKind.Absolute, out _webhookUrl))
                throw new ArgumentException("Please enter a valid Slack webhook url");

            if (!ValidHosts.Contains(_webhookUrl.Host))
                throw new ArgumentException("Please enter a valid Slack webhook url");

            _httpClient.Timeout = new TimeSpan(0, 0, 0, timeoutSeconds);
        }
        
        public async Task PostAsync(SlackMessage message, IEnumerable<string> channels)
        {
            foreach (var channel in channels)
                await PostAsync(message, channel);
        }

        public async Task<HttpResponseMessage> PostAsync(
            SlackMessage message, 
            string channel = null)
        {
            var payload = new
            {
                text = message.Text,
                channel,
                username = message.Username,
            };

            var serializedPayload = JsonConvert.SerializeObject(payload);
            var response = await _httpClient.PostAsync(_webhookUrl,
                new StringContent(serializedPayload, Encoding.UTF8, "application/json"));

            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(
            string message,
            string channel = null)
        {
            var response = await _httpClient.PostAsync(_webhookUrl,
                new StringContent(message, Encoding.UTF8, "application/json"));

            return response;
        }
    }
}
