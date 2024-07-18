using Blog.Business.Dtos.HomeDtos;
using Blog.Business.Services;
using Blog.Data.Entities;
using Blog.Data.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Business.Managers
{
    public class HomeManager : IHomeService
    {
        private readonly IRepository<Article> _articleRepository;

        public HomeManager(IRepository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public List<HomeArticleDto> GetAllArticles()
        {
            return _articleRepository.GetAll().Select(a => new HomeArticleDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                ImageUrl = a.ImageUrl,
                TagIds = a.ArticleTags.Select(at => at.TagId).ToList()
            }).ToList();
        }

        public List<HomeArticleDto> SearchArticles(string searchTerm)
        {
            return _articleRepository.GetAll()
                .Where(a => a.Title.Contains(searchTerm) || a.Content.Contains(searchTerm))
                .Select(a => new HomeArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    ImageUrl = a.ImageUrl,
                    TagIds = a.ArticleTags.Select(at => at.TagId).ToList()
                }).ToList();
        }

        public List<HomeArticleDto> FilterArticlesByTags(List<int> tagIds)
        {
            return _articleRepository.GetAll()
                .Where(a => a.ArticleTags.Any(at => tagIds.Contains(at.TagId)))
                .Select(a => new HomeArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    ImageUrl = a.ImageUrl,
                    TagIds = a.ArticleTags.Select(at => at.TagId).ToList()
                }).ToList();
        }
    }
}
