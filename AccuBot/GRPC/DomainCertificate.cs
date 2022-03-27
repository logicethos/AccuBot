using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using AccuBotCommon.Proto;
using Google.Protobuf.WellKnownTypes;
using System.IO;
using Google.Protobuf;

namespace AccuBot.GRPC;

public partial class ApiService
{
    public override Task<DomainCertificateList> DomainCertificateListGet(Empty request, ServerCallContext context)
    {
        DomainCertificateList domainCertificateList;
        if (File.Exists(Path.Combine(path,"domainCertificateList")))
        {  //Read from file
            domainCertificateList = DomainCertificateList.Parser.ParseFrom(File.ReadAllBytes(Path.Combine(path, "domainCertificateList")));
        }
        else
        {
            domainCertificateList = new DomainCertificateList();
            domainCertificateList.DomainCertificate.Add(new DomainCertificate
            {
                DomainCertificateID = 1,
                Domain = "mydomain.com",
                Monitor = false
            });
        }
        return Task.FromResult(domainCertificateList);
    }

    public override Task<MsgReply> DomainCertificateSet(DomainCertificate domainCertificate, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var exitingDomainCertificate = DomainCertificateListGet(null, null).Result;  //get existing nodes.

        if (domainCertificate.DomainCertificateID == 0) //id not set, so new cert
        {
            domainCertificate.DomainCertificateID = exitingDomainCertificate.DomainCertificate.Max(x => x.DomainCertificateID) + 1; //get new id
            exitingDomainCertificate.DomainCertificate.Add(domainCertificate);
        }
        else
        {
            var existingDomainCertificate = exitingDomainCertificate.DomainCertificate.FirstOrDefault(x => x.DomainCertificateID == domainCertificate.DomainCertificateID);
            if (existingDomainCertificate == null)  //Incorrect ID sent!
            {
                msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "DomainCertificate not found"};
                return Task.FromResult(msgReply);
            }
            else
            {
                existingDomainCertificate.Domain = domainCertificate.Domain;
                existingDomainCertificate.Monitor = domainCertificate.Monitor;
                existingDomainCertificate.DomainCertificateID = domainCertificate.DomainCertificateID;
            }
        }
        msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok, NewID32 = domainCertificate.DomainCertificateID};
        File.WriteAllBytes(Path.Combine(path, "domainCertificateList"),exitingDomainCertificate.ToByteArray());
        return Task.FromResult(msgReply);
    }

    public override Task<MsgReply> DomainCertificateDelete(ID32 domainCertificateID, ServerCallContext context)
    {
        MsgReply msgReply = null;
        var existingDomainCertificate = DomainCertificateListGet(null, null).Result;  //get existing nodes.

        var domainCertificate = existingDomainCertificate.DomainCertificate.FirstOrDefault(x => x.DomainCertificateID == domainCertificateID.ID);
        if (domainCertificate==null)
        {
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Fail, Message = "DomainCertificate not found"};
        }
        else
        {
            existingDomainCertificate.DomainCertificate.Remove(domainCertificate);
            File.WriteAllBytes(Path.Combine(path, "domainCertificateList"),existingDomainCertificate.ToByteArray());
            msgReply = new MsgReply() { Status = MsgReply.Types.Status.Ok};
        }
        return Task.FromResult(msgReply);
    }

}