﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Stores.InMemory;
using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer4.UnitTests.Validation.TokenRequest
{
    public class TokenRequestValidation_General_Invalid
    {
        private const string Category = "TokenRequest Validation - General - Invalid";

        IClientStore _clients = new InMemoryClientStore(TestClients.Get());
        ClaimsPrincipal _subject = IdentityServerPrincipal.Create("bob", "Bob Loblaw");

        [Fact]
        [Trait("Category", Category)]
        public void Parameters_Null()
        {
            var validator = Factory.CreateTokenRequestValidator();

            Func<Task> act = () => validator.ValidateRequestAsync(null, null);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", Category)]
        public void Client_Null()
        {
            var validator = Factory.CreateTokenRequestValidator();

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.AuthorizationCode);
            parameters.Add(OidcConstants.TokenRequest.Code, "valid");
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            Func<Task> act = () => validator.ValidateRequestAsync(parameters, null);

            act.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Unknown_Grant_Type()
        {
            var client = await _clients.FindEnabledClientByIdAsync("codeclient");
            var store = Factory.CreateAuthorizationCodeStore();

            var code = new AuthorizationCode
            {
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                IsOpenId = true,
                RedirectUri = "https://server/cb",
                Subject = _subject
            };

            await store.StoreAuthorizationCodeAsync("valid", code);

            var validator = Factory.CreateTokenRequestValidator(
                authorizationCodeStore: store);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "unknown");
            parameters.Add(OidcConstants.TokenRequest.Code, "valid");
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.UnsupportedGrantType);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Invalid_Protocol_Type()
        {
            var client = await _clients.FindEnabledClientByIdAsync("client.cred.wsfed");
            var codeStore = Factory.CreateAuthorizationCodeStore();

            var validator = Factory.CreateTokenRequestValidator(
                authorizationCodeStore:codeStore);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.GrantType, "client_credentials");

            var result = await validator.ValidateRequestAsync(parameters, client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.InvalidClient);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Missing_Grant_Type()
        {
            var client = await _clients.FindEnabledClientByIdAsync("codeclient");
            var store = Factory.CreateAuthorizationCodeStore();

            var code = new AuthorizationCode
            {
                ClientId = client.ClientId,
                Lifetime = client.AuthorizationCodeLifetime,
                IsOpenId = true,
                RedirectUri = "https://server/cb",
                Subject = _subject
            };

            await store.StoreAuthorizationCodeAsync("valid", code);

            var validator = Factory.CreateTokenRequestValidator(
                authorizationCodeStore: store);

            var parameters = new NameValueCollection();
            parameters.Add(OidcConstants.TokenRequest.Code, "valid");
            parameters.Add(OidcConstants.TokenRequest.RedirectUri, "https://server/cb");

            var result = await validator.ValidateRequestAsync(parameters, client);

            result.IsError.Should().BeTrue();
            result.Error.Should().Be(OidcConstants.TokenErrors.UnsupportedGrantType);
        }
    }
}