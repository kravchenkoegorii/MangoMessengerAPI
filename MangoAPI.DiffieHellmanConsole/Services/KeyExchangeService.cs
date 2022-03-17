﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MangoAPI.BusinessLogic.ApiCommands.KeyExchange;
using MangoAPI.BusinessLogic.ApiQueries.KeyExchange;
using MangoAPI.DiffieHellmanConsole.Consts;
using MangoAPI.Domain.Constants;
using Newtonsoft.Json;

namespace MangoAPI.DiffieHellmanConsole.Services;

public class KeyExchangeService
{
    private const string Route = "key-exchange";
    private readonly HttpClient _httpClient;

    public KeyExchangeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        var tokensResponse = new TokensService().GetTokensAsync().GetAwaiter().GetResult();

        if (tokensResponse == null)
        {
            const string error = ResponseMessageCodes.TokensNotFound;
            var details = ResponseMessageCodes.ErrorDictionary[error];

            throw new InvalidOperationException($"{error}. {details}, {nameof(tokensResponse)}");
        }

        var accessToken = tokensResponse.Tokens.AccessToken;

        _httpClient.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<GetKeyExchangeResponse> GetKeyExchangesAsync()
    {
        const string route = Urls.ApiUrl + Route;
        var result = await HttpRequest.GetAsync(_httpClient, route);
        var response = JsonConvert.DeserializeObject<GetKeyExchangeResponse>(result);
        return response;
    }

    public async Task<CreateKeyExchangeResponse> CreateKeyExchangeRequestAsync(Guid requestUserId,
        string publicKey)
    {
        var command = new CreateKeyExchangeRequest
        {
            PublicKey = publicKey,
            RequestedUserId = requestUserId
        };

        const string route = Urls.ApiUrl + Route;
        var result = await HttpRequest.PostWithBodyAsync(_httpClient, route, command);
        var response = JsonConvert.DeserializeObject<CreateKeyExchangeResponse>(result);
        return response;
    }

    public async Task ConfirmOrDeclineKeyExchange(Guid requestId, string publicKeyBase64)
    {
        var request = new ConfirmOrDeclineKeyExchangeRequest
        {
            Confirmed = true,
            PublicKey = publicKeyBase64,
            RequestId = requestId
        };

        const string route = Urls.ApiUrl + Route;
        await HttpRequest.DeleteWithBodyAsync(_httpClient, route, request);
    }
}