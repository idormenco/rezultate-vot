﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using CSharpFunctionalExtensions;
using ElectionResults.Core.Models;
using ElectionResults.Core.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ElectionResults.Core.Infrastructure
{
    public class ElectionConfigurationSource : IElectionConfigurationSource
    {
        private readonly AppConfig _config;
        private readonly AmazonSimpleSystemsManagementClient _amazonSettingsClient;

        public ElectionConfigurationSource(IOptions<AppConfig> config)
        {
            _config = config.Value;
            _amazonSettingsClient = new AmazonSimpleSystemsManagementClient();
        }

        public async Task<Result> UpdateInterval(int seconds)
        {
            var putParameterRequest = new PutParameterRequest
            {
                Name = $"/{Consts.PARAMETER_STORE_NAME}/settings/intervalInSeconds",
                Value = seconds.ToString(),
                Type = ParameterType.String,
                Overwrite = true
            };
            var response = await _amazonSettingsClient.PutParameterAsync(putParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok();
            return Result.Failure("Couldn't update the job timer");
        }

        public async Task<Result<int>> GetInterval()
        {
            var getParameterRequest = new GetParameterRequest
            {
                Name = $"/{Consts.PARAMETER_STORE_NAME}/settings/intervalInSeconds",
            };
            var response = await _amazonSettingsClient.GetParameterAsync(getParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok(int.Parse(response.Parameter.Value));
            return Result.Failure<int>("Couldn't retrieve the job timer");
        }

        public async Task<Result> UpdateElectionConfig(ElectionsConfig config)
        {
            var putParameterRequest = new PutParameterRequest
            {
                Name = $"/{Consts.PARAMETER_STORE_NAME}/settings/electionsConfig",
                Value = JsonConvert.SerializeObject(config),
                Type = ParameterType.String,
                Overwrite = true
            };
            var response = await _amazonSettingsClient.PutParameterAsync(putParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok();
            return Result.Failure("Couldn't update the job timer");
        }

        public async Task<Result<string>> GetConfigAsync()
        {
            var getParameterRequest = new GetParameterRequest
            {
                Name = $"/{Consts.PARAMETER_STORE_NAME}/settings/electionsConfig",
            };
            var response = await _amazonSettingsClient.GetParameterAsync(getParameterRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
                return Result.Ok(response.Parameter.Value);
            return Result.Failure<string>("Couldn't update the job timer");
        }

        public List<ElectionResultsFile> GetListOfFilesWithElectionResults()
        {
            var electionsConfig = JsonConvert.DeserializeObject<ElectionsConfig>(_config.ElectionsConfig);
            return electionsConfig.Files;
        }
    }
}