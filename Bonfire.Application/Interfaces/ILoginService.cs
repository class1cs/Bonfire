﻿using Bonfire.Core.Dtos.Requests;
using Bonfire.Core.Entities;

namespace Bonfire.Application.Interfaces;

public interface ILoginService
{
    public Task<string> Login(LoginRequest loginRequest);

    public Task<User?> VerifyLoginCredentials(string nickName, string password);
}