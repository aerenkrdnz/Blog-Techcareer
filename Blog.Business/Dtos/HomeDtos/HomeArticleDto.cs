using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Dtos.HomeDtos
{
    public class HomeArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public string UserName { get; set; }
        public List<string> TagNames { get; set; }
        public List<int> TagIds { get; set; }
    }
}
