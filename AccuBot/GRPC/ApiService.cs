using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Proto.API;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;


namespace AccuBot.GRPC;

[Authorize]
public partial class ApiService : AccuBotAPI.AccuBotAPIBase
{
    public static ApiService instance;
    string path = "data";

}