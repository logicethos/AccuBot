using System.Net.Http.Json;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace AccuBot;


public class DockerManager : IDisposable
{
    const string cgroup = "/proc/self/cgroup";
    
    ImagesCreateParameters watchtowerImage = new ImagesCreateParameters
    {
        FromImage = "containrrr/watchtower",
        Tag = "latest",
    };
    
    public String DockerID { get; private set; }
    public String DockerName { get; private set; } = "AccuBot";
    DockerClient client;

    public DockerManager(string dockerSocket="unix:///var/run/docker.sock")
    {
        client = new DockerClientConfiguration(new Uri(dockerSocket)).CreateClient();
        DockerID = GetID();
    }

    /// <summary>
    /// Lists Containers Locally.
    /// </summary>
    public async Task ListContainers()
    {
        IList<ContainerListResponse> containers =
            await client.Containers.ListContainersAsync(new ContainersListParameters() { Limit = 10, });
        
        foreach (var container in containers)
        {
            Console.WriteLine($"Image: {container.Image}, Names: {string.Join(", ", container.Names)}  State:{container.State}");
        }
    }

    public async Task ListTags()
    {
        var image = await client.Images.GetImageHistoryAsync("containrrr/watchtower");
        foreach (var reply in image)
        {
            Console.WriteLine($"ID: {reply.ID}  Comment: {reply.Comment}  Created: {reply.CreatedBy}");
        }
    }

    
    
    
    /// <summary>
    /// Get Local Container ID
    /// </summary>
    /// <returns>ID or Null</returns>
    private string GetID()
    {
        if (File.Exists(cgroup))
        {
            var lines = File.ReadLines(cgroup);
            foreach (var line in lines)
            {
                var pt1 = line.LastIndexOf("/");
                var len = line.Length - pt1;
                if (pt1 == 64) return line.Substring(pt1, 64);
            }
        }

        return null;
    }

    
    public async Task UpgradeContainer()
    {
        try
        {
            
            
            //Pull Image
            await client.Images.CreateImageAsync(watchtowerImage, new AuthConfig(), new Progress());
            
            //Set Watchtower container Run Parameters
            var runParams = new CreateContainerParameters
            {
                Name = null,
                Env = null,
                Cmd = new string[] { "--run-once", DockerID ?? DockerName },
                ArgsEscaped = false,
                Image = watchtowerImage.FromImage,
                Entrypoint = null,
                Labels = null,
                HostConfig = new HostConfig()
                {
                    Binds = new List<string>()
                    {
                        "/var/run/docker.sock:/var/run/docker.sock",
                    },
                    AutoRemove = true
                }
            };
            
            //Create Watchtower container
            var result = await client.Containers.CreateContainerAsync(runParams);

            Console.WriteLine($"CreateContainerAsync: {result}");
            foreach (var warn in result.Warnings)
            {
                Console.WriteLine($"Warning: {warn}");
            }
            //Run Watchtower
            var result2 = await client.Containers.StartContainerAsync(result.ID, new ContainerStartParameters());
            Console.WriteLine($"StartContainerAsync: {result2}");
            
            
        }catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Dispose()
    {
        client.Dispose();
    }
}

public class Progress : IProgress<JSONMessage>
{
    public void Report(JSONMessage value)
    {
        Console.WriteLine(value.Status);
    }
}
