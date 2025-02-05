﻿using System;

namespace MangoAPI.Domain.Entities;

public class UserContactEntity
{
    public Guid Id { get; set; }

    public Guid ContactId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserEntity User { get; set; }
}