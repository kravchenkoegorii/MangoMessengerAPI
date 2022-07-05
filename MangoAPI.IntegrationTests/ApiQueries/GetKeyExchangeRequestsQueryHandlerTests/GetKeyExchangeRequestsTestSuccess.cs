﻿using System.Threading;
using System.Threading.Tasks;
using MangoAPI.BusinessLogic.ApiQueries.DiffieHellmanKeyExchanges;
using MangoAPI.IntegrationTests.Helpers;
using Xunit;

namespace MangoAPI.IntegrationTests.ApiQueries.GetKeyExchangeRequestsQueryHandlerTests;

public class GetKeyExchangeRequestsTestSuccess : IntegrationTestBase
{
    private readonly Assert<GetKeyExchangeRequestsResponse> assert = new();

    [Fact]
    public async Task GetKeyExchangeRequestsTest_Success()
    {
        var sender =
            await MangoModule.RequestAsync(CommandHelper.RegisterKhachaturCommand(), CancellationToken.None);
        var requestedUser =
            await MangoModule.RequestAsync(CommandHelper.RegisterPetroCommand(), CancellationToken.None);
        var publicKey = MangoFilesHelper.GetTestImage();
        await MangoModule.RequestAsync(
            request: CommandHelper.CreateOpenSslCreateKeyExchangeCommand(
                receiverId: sender.Response.UserId,
                senderId: requestedUser.Response.UserId,
                senderPublicKey: publicKey),
            cancellationToken: CancellationToken.None);
        var query = new GetKeyExchangeRequestsQuery(requestedUser.Response.UserId);

        var response = await MangoModule.RequestAsync(query, CancellationToken.None);

        assert.Pass(response);
    }
}
