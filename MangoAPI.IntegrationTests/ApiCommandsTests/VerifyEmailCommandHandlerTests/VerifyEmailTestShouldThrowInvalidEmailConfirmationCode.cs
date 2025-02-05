﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MangoAPI.BusinessLogic.Responses;
using MangoAPI.Domain.Constants;
using MangoAPI.IntegrationTests.Helpers;
using Xunit;

namespace MangoAPI.IntegrationTests.ApiCommandsTests.VerifyEmailCommandHandlerTests;

public class VerifyEmailTestShouldThrowInvalidEmailConfirmationCode : IntegrationTestBase
{
    private readonly Assert<ResponseBase> assert = new();

    [Fact]
    public async Task VerifyEmailTestShouldThrowInvalidEmailConfirmationCodeAsync()
    {
        const string expectedMessage = ResponseMessageCodes.InvalidEmailConfirmationCode;
        var expectedDetails = ResponseMessageCodes.ErrorDictionary[expectedMessage];
        await MangoModule.RequestAsync(
            request: CommandHelper.RegisterPetroCommand(),
            cancellationToken: CancellationToken.None);

        var result = await MangoModule.RequestAsync(
            request: CommandHelper.CreateVerifyEmailCommand("kolosovp95@gmail.com", Guid.NewGuid()),
            cancellationToken: CancellationToken.None);

        assert.Fail(result, expectedMessage, expectedDetails);
    }
}
