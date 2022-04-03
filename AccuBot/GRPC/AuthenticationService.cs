using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Proto.Authentication;
using Google.Protobuf;


namespace AccuBot.GRPC;

public class AuthenticationService : Proto.Authentication.AccuBotAuthentication.AccuBotAuthenticationBase
{
    public override async Task<AuthenticationReply> Authenticate(AuthenticationRequest request, ServerCallContext context)
    {
        var authenticationReply = JwtAuthenticationManager.Authenticate(request);
        if (authenticationReply == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid user Credentials"));

        return authenticationReply;
    }
}