﻿using System.Threading;
using System.Threading.Tasks;
using MangoAPI.BusinessLogic.ApiCommands.Contacts;
using MangoAPI.BusinessLogic.Responses;
using MangoAPI.Domain.Constants;
using MangoAPI.IntegrationTests.Helpers;
using Xunit;

namespace MangoAPI.IntegrationTests.ApiCommandsTests.AddContactCommandHandlerTests;

public class AddContactShouldThrowContactExists : IntegrationTestBase
{
    private readonly Assert<ResponseBase> assert = new();

    [Fact]
    public async Task AddContactCommandHandlerTestShouldThrowContactExistsAsync()
    {
        const string expectedMessage = ResponseMessageCodes.ContactAlreadyExist;
        var expectedDetails = ResponseMessageCodes.ErrorDictionary[expectedMessage];
        var sender = await MangoModule.RequestAsync(CommandHelper.RegisterKhachaturCommand(), CancellationToken.None);
        var receiver = await MangoModule.RequestAsync(CommandHelper.RegisterPetroCommand(), CancellationToken.None);
        var command = new AddContactCommand(
            UserId: sender.Response.Tokens.UserId,
            ContactId: receiver.Response.Tokens.UserId);
        await MangoModule.RequestAsync(command, CancellationToken.None);

        var result = await MangoModule.RequestAsync(command, CancellationToken.None);

        assert.Fail(result, expectedMessage, expectedDetails);
    }
}
