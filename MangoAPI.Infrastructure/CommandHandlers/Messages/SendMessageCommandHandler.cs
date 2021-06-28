﻿using System.Threading;
using System.Threading.Tasks;
using MangoAPI.DTO.Commands.Messages;
using MangoAPI.DTO.Responses.Messages;
using MediatR;

namespace MangoAPI.Infrastructure.CommandHandlers.Messages
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, SendMessageResponse>
    {
        public async Task<SendMessageResponse> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}