using Blog.Business.Dtos.ArticleDtos;
using Blog.Business.Types;
using System.Collections.Generic;

namespace Blog.Business.Services
{
    public interface IArticleService
    {
        ServiceMessage AddArticle(AddArticleDto addArticleDto);
        List<ArticleInfoDto> GetAllArticles();
        List<ArticleInfoDto> GetArticlesByUserId(int userId);
        List<ArticleInfoDto> SearchArticles(string searchTerm);
        List<ArticleInfoDto> FilterArticlesByTag(int tagId);
        ArticleInfoDto GetArticleById(int id);
        ServiceMessage UpdateArticle(UpdateArticleDto updateArticleDto, int id);
        ServiceMessage DeleteArticle(int id);

    }
}
