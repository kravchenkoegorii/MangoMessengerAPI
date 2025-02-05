﻿using System.Threading;
using System.Threading.Tasks;
using MangoAPI.BusinessLogic.ApiCommands.Documents;
using MangoAPI.IntegrationTests.Helpers;
using Xunit;

namespace MangoAPI.IntegrationTests.ApiCommandsTests.UploadDocumentCommandHandlerTests;

public class UploadDocumentSuccess : IntegrationTestBase
{
    private readonly Assert<UploadDocumentResponse> assert = new();

    [Fact]
    public async Task UploadDocumentSuccessAsync()
    {
        var file = MangoFilesHelper.GetTestImage();
        var user = await MangoModule.RequestAsync(
            request: CommandHelper.RegisterPetroCommand(),
            cancellationToken: CancellationToken.None);
        var command =
            new UploadDocumentCommand(FormFile: file, UserId: user.Response.Tokens.UserId, ContentType: "image/jpeg");

        var result = await MangoModule.RequestAsync(command, CancellationToken.None);

        assert.Pass(result);
        await BlobService.DeleteBlobAsync(result.Response.FileName);
    }
}
