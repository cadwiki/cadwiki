

using Microsoft.AspNetCore.Components;

namespace BlogApp.Models
{
    public class BlogPost
    {
        public string DateCreated { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public MarkupString MarkupString { get; set; }

        public string TrimmedTitle
        {
            get
            {
                return Title.Replace(" ", "-");
            }
            set
            {
                TrimmedTitle = value;
            }
        }

    }
}
