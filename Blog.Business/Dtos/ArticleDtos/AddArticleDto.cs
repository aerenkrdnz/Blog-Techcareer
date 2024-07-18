using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Dtos.ArticleDtos
{
    public class AddArticleDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public int UserId { get; set; }
        public List<int> TagIds { get; set; }
    }
}
