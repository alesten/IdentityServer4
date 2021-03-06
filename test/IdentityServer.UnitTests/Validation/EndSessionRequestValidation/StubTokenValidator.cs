﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer4.UnitTests.Validation.EndSessionRequestValidation
{
    public class StubTokenValidator : ITokenValidator
    {
        public TokenValidationResult AccessTokenValidationResult { get; set; } = new TokenValidationResult();
        public TokenValidationResult IdentityTokenValidationResult { get; set; } = new TokenValidationResult();

        public Task<TokenValidationResult> ValidateAccessTokenAsync(string token, string expectedScope = null)
        {
            return Task.FromResult(AccessTokenValidationResult);
        }

        public Task<TokenValidationResult> ValidateIdentityTokenAsync(string token, string clientId = null, bool validateLifetime = true)
        {
            return Task.FromResult(IdentityTokenValidationResult);
        }
    }
}
