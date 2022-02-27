using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;


namespace AccuTest.GRPC;

public partial class ApiService : AccuBotAPI.AccuBotAPIBase
{
    public static ApiService instance;
    string path = "data";

}