using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlogApp.Models;

namespace BlogApp.Services
{
    public class BlogPostService : IBlogPostService
    {

        private readonly HttpClient httpClient;

        public BlogPostService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<bool> DoesFileExistAsync(string filePath)
        {
            HttpResponseMessage message = await httpClient.GetAsync(filePath);
            if (message.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            return true;
        }

        public async Task<BlogPost> GetPostAsync(string trimmedTitle)
        {
            var result = await GetPostsAsync();
            foreach (BlogPost bp in result)
            {
                if (bp.TrimmedTitle == trimmedTitle)
                {
                    return bp;
                }
            }
            return null;
        }

        public async Task<List<BlogPost>> GetPostsAsync()
        {
            try
            {
                List<BlogPost> blogPosts = new List<BlogPost>();
                String jsonFiles = await httpClient.GetStringAsync("posts/post.manifest");
                String[] jsonFileNames = jsonFiles.Split("\r\n", StringSplitOptions.None);
                foreach (string jsonFileName in jsonFileNames)
                {
                    String postFile = Path.GetFileNameWithoutExtension(jsonFileName);
                    if (!String.IsNullOrEmpty(jsonFileName))
                    {
                        await GetPosts(blogPosts, jsonFileName, postFile);
                    }
                }
                return blogPosts;
            }
            catch (Exception ex)
            {
                String test = ex.Message;
            }
            var emptyList = new List<BlogPost>();
            return emptyList;
        }

        private async Task GetPosts(List<BlogPost> blogPosts, string jsonFileName, string postFile)
        {
            String blogFileLocation = "posts/" + jsonFileName;
            if (await DoesFileExistAsync(blogFileLocation))
            {
                BlogPost[] currentPosts = await httpClient.GetFromJsonAsync<BlogPost[]>(blogFileLocation);
                String markdownFileLocation = "posts/" + postFile + ".md";
                await GetMarkdown(currentPosts, markdownFileLocation);
                blogPosts.AddRange(currentPosts);
            }
        }

        private async Task GetMarkdown(BlogPost[] currentPosts, string markdownFileLocation)
        {
            foreach (BlogPost bp in currentPosts)
            {
                if (await DoesFileExistAsync(markdownFileLocation))
                {
                    String markdown = await httpClient.GetStringAsync(markdownFileLocation);
                    if (!String.IsNullOrEmpty(markdown))
                    {
                        var html = Markdig.Markdown.ToHtml(markdown ?? "");
                        var convertedMarkdown = new MarkupString(html);
                        bp.MarkupString = convertedMarkdown;
                    }
                }
            }
        }
    }
    
}
