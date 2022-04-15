using System.Net.Http.Json;
using System.Linq;

namespace AccuBot;

public class GitLab
{
    private UInt32 ProjectID { get; set; }
    private UInt32 RepoID { get; set; }
    
    public GitLab(UInt32 projectID)
    {
        ProjectID = projectID;
    }
    
    public GitLab(string projectPath)
    {
        var registry = GetDockerRegistory(projectPath).Result;
        ProjectID = registry.id;
    }
    
    public async Task<GitLabRegistry> GetDockerRegistory(string ProjectPath)
    {
        using (var httpClient = new HttpClient())
        {
            var repositories = await httpClient.GetFromJsonAsync<List<GitLabRegistry>>(
                $"https://gitlab.com/api/v4/projects/{ProjectID}/registry/repositories");

            var repository = repositories.FirstOrDefault(x => x.path == ProjectPath);
            return repository;
        }
    }

    public async Task<List<GitLabImages>> GetDockerImages(UInt32 dockerRegistoryID)
    {
        using (var httpClient = new HttpClient())
        {
            var images = await httpClient.GetFromJsonAsync<List<GitLabImages>>(
                $"https://gitlab.com/api/v4/projects/{ProjectID}/registry/repositories/{dockerRegistoryID}/tags");
            
            return images;
        }
    }
    
    public async Task<Commits> GetCommits(string tag)
    {
        using (var httpClient = new HttpClient())
        {
            var images = await httpClient.GetFromJsonAsync<Commits>(
                $"https://gitlab.com/api/v4/projects/{ProjectID}/repository/tags/{tag}");
            
            return images;
        }
    }

    public async Task<List<GitLabImages>> ListTags2()
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var repositories = await httpClient.GetFromJsonAsync<List<GitLabRegistry>>(
                    $"https://gitlab.com/api/v4/projects/{ProjectID}/registry/repositories");

                var images = await httpClient.GetFromJsonAsync<List<GitLabImages>>(
                    $"https://gitlab.com/api/v4/projects/{ProjectID}/registry/repositories/{RepoID}/tags");

                return images;
            }
        }
        catch (HttpRequestException) // Non success
        {
            Console.WriteLine("An error occurred.");
        }
        catch (NotSupportedException) // When content type is not valid
        {
            Console.WriteLine("The content type is not supported.");
        }

        return null;
    }
}


//https://gitlab.com/api/v4/projects/{RepoID}/registry/repositories/accumulate/tags
public class GitLabImages
{
    public string name { get; set; }
    public string path { get; set; }
    public string location { get; set; }
}

//"https://gitlab.com/api/v4/projects/{ProJectID}/registry/repositories"
public class GitLabRegistry
{
    public UInt32 id { get; set; }
    public string name { get; set; }
    public string path { get; set; }
    public int project_id { get; set; }
    public string location { get; set; }
    public DateTime created_at { get; set; }
    public DateTime cleanup_policy_started_at { get; set; }
}


public class Commit
{
    public string id { get; set; }
    public string short_id { get; set; }
    public DateTime created_at { get; set; }
    public List<string> parent_ids { get; set; }
    public string title { get; set; }
    public string message { get; set; }
    public string author_name { get; set; }
    public string author_email { get; set; }
    public DateTime authored_date { get; set; }
    public string committer_name { get; set; }
    public string committer_email { get; set; }
    public DateTime committed_date { get; set; }
  //  public Trailers trailers { get; set; }
    public string web_url { get; set; }
}

//https://gitlab.com/api/v4/projects/{ProjectID}/repository/tags/{tag}
public class Commits
{
    public string name { get; set; }
    public string message { get; set; }
    public string target { get; set; }
    public Commit commit { get; set; }
    public object release { get; set; }
    public bool @protected { get; set; }
}