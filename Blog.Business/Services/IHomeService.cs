using Blog.Business.Dtos.HomeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Services
{
    public interface IHomeService
    {
        List<HomeArticleDto> GetAllArticles();
        List<HomeArticleDto> SearchArticles(string searchTerm);
        List<HomeArticleDto> FilterArticlesByTags(List<int> tagIds);
    }
}
