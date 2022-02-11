using System.Collections.Generic;
using System.Threading.Tasks;
using BlogApp.Models;

namespace BlogApp.Services
{
    public interface IBlogPostService
    {

        public interface IPostService
        {
            Task<IEnumerable<BlogPost>> GetPosts();

        }

    }
}