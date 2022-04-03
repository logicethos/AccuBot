﻿using System;
namespace AccuBot
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyGitCommit : Attribute
    {
        
    public string Hash;
    public AssemblyGitCommit() : this(string.Empty) {}
    public AssemblyGitCommit(string hash) { Hash = hash; }
        
    }
}
