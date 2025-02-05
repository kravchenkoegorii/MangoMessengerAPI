﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangoAPI.BusinessLogic.ApiCommands.Communities;
using MangoAPI.Domain.Constants;
using MangoAPI.IntegrationTests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace MangoAPI.IntegrationTests.ApiCommandsTests.UpdateChannelPictureCommandHandlerTests;

public class UpdateChannelPictureShouldThrowLimitExceed10 : IntegrationTestBase
{
    private readonly ITestOutputHelper testOutputHelper;
    private readonly Assert<UpdateChannelPictureResponse> assert = new();

    public UpdateChannelPictureShouldThrowLimitExceed10(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task UpdateChannelPictureShouldThrowLimitReached10Async()
    {
        const string expectedMessage = ResponseMessageCodes.UploadedDocumentsLimitReached10;
        var expectedDetails = ResponseMessageCodes.ErrorDictionary[expectedMessage];
        var sender = await MangoModule.RequestAsync(
            CommandHelper.RegisterPetroCommand(), CancellationToken.None);
        var userId = sender.Response.Tokens.UserId;
        var createChannelCommand = CommandHelper.CreateExtremeCodeMainChatCommand(userId);
        var chat = await MangoModule.RequestAsync(createChannelCommand, CancellationToken.None);
        var chatId = chat.Response.ChatId;
        var file = MangoFilesHelper.GetTestImage();
        var command = new UpdateChanelPictureCommand(
            ChatId: chatId,
            UserId: userId,
            NewGroupPicture: file,
            ContentType: "image/jpeg");

        var imageNamesList = new List<string>();
        for (var i = 0; i < 10; i++)
        {
            var res = await MangoModule.RequestAsync(command, CancellationToken.None);
            testOutputHelper.WriteLine(
                $"UpdateChannelPicture_ShouldThrow_LimitReached10 File upload: {res.Response.Message}");
            imageNamesList.Add(res.Response.FileName);
        }

        var result = await MangoModule.RequestAsync(command, CancellationToken.None);

        assert.Fail(result, expectedMessage, expectedDetails);
        foreach (var fileName in imageNamesList)
        {
            await BlobService.DeleteBlobAsync(fileName);
        }
    }
}
