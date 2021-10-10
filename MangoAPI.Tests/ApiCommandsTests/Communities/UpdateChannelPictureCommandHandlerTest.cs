﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MangoAPI.BusinessLogic.ApiCommands.Communities;
using MangoAPI.BusinessLogic.BusinessExceptions;
using MangoAPI.Domain.Constants;
using NUnit.Framework;

namespace MangoAPI.Tests.ApiCommandsTests.Communities
{
    [TestFixture]
    public class UpdateChannelPictureCommandHandlerTest
    {
        [Test]
        public async Task UpdateChannelPictureCommandHandlerTest_ShouldThrowChatNotFound()
        {
            using var dbContextFixture = new DbContextFixture();
            var handler = new UpdateChannelPictureCommandHandler(dbContextFixture.PostgresDbContext);
            var command = new UpdateChanelPictureCommand
            {
                UserId = SeedDataConstants.RazumovskyId,
                ChatId = Guid.NewGuid(),
                Image = "../../../MangoAPI.Presentation/wwwroot/extremecode_dotnet.png"
            };

            Func<Task> result = async () => await handler.Handle(command, CancellationToken.None);

            await result.Should().ThrowAsync<BusinessException>()
                .WithMessage(ResponseMessageCodes.ChatNotFound);
        }
    }
}