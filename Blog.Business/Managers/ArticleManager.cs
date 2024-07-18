using Blog.Business.Dtos.ArticleDtos;
using Blog.Business.Services;
using Blog.Business.Types;
using Blog.Data.Entities;
using Blog.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Business.Managers
{
    public class ArticleManager : IArticleService
    {
        private readonly IRepository<Article> _articleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ArticleTag> _articleTagRepository;

        public ArticleManager(IRepository<Article> articleRepository, IRepository<User> userRepository, IRepository<ArticleTag> articleTagRepository)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
            _articleTagRepository = articleTagRepository;
        }

        public ServiceMessage AddArticle(AddArticleDto addArticleDto)
        {
            var user = _userRepository.Get(x => x.Id == addArticleDto.UserId);
            if (user == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Kullanıcı Bulunamadı." };
            }

            var entity = new Article
            {
                Title = addArticleDto.Title,
                Content = addArticleDto.Content,
                ImageUrl = addArticleDto.ImageUrl,
                UserId = addArticleDto.UserId
            };

            _articleRepository.Add(entity);

            foreach (var tagId in addArticleDto.TagIds)
            {
                _articleTagRepository.Add(new ArticleTag
                {
                    ArticleId = entity.Id,
                    TagId = tagId
                });
            }

            return new ServiceMessage() { IsSucceed = true };
        }

        public List<ArticleInfoDto> GetAllArticles()
        {
            var articles = _articleRepository.GetAll().Include(a => a.User).Include(a => a.ArticleTags).ThenInclude(at => at.Tag).Select(article => new ArticleInfoDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                ImageUrl = article.ImageUrl,
                UserName = article.User.FirstName + " " + article.User.LastName,
                TagNames = article.ArticleTags.Select(at => at.Tag.Name).ToList()
            }).ToList();

            return articles;
        }

        public List<ArticleInfoDto> GetArticlesByUserId(int userId)
        {
            var articles = _articleRepository.GetAll(a => a.UserId == userId).Select(article => new ArticleInfoDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                ImageUrl = article.ImageUrl,
                UserName = article.User.FirstName + " " + article.User.LastName,
                TagNames = article.ArticleTags.Select(at => at.Tag.Name).ToList(),
                TagIds = article.ArticleTags.Select(at => at.TagId).ToList(),
                UserId = article.UserId
            }).ToList();

            return articles;
        }


        public List<ArticleInfoDto> SearchArticles(string searchTerm)
        {
            var articles = _articleRepository.GetAll(a => a.Title.Contains(searchTerm) || a.Content.Contains(searchTerm))
                .Include(a => a.User)
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Select(article => new ArticleInfoDto
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    ImageUrl = article.ImageUrl,
                    UserName = article.User.FirstName + " " + article.User.LastName,
                    TagNames = article.ArticleTags.Select(at => at.Tag.Name).ToList()
                }).ToList();

            return articles;
        }

        public List<ArticleInfoDto> FilterArticlesByTag(int tagId)
        {
            var articles = _articleRepository.GetAll()
                .Include(a => a.User)
                .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
                .Where(a => a.ArticleTags.Any(at => at.TagId == tagId))
                .Select(article => new ArticleInfoDto
                {
                    Id = article.Id,
                    Title = article.Title,
                    Content = article.Content,
                    ImageUrl = article.ImageUrl,
                    UserName = article.User.FirstName + " " + article.User.LastName,
                    TagNames = article.ArticleTags.Select(at => at.Tag.Name).ToList()
                }).ToList();

            return articles;
        }


        public ArticleInfoDto GetArticleById(int id)
        {
            var article = _articleRepository.GetAll().Include(a => a.User).Include(a => a.ArticleTags).ThenInclude(at => at.Tag).FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return null;
            }

            return new ArticleInfoDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                ImageUrl = article.ImageUrl,
                UserName = article.User.FirstName + " " + article.User.LastName,
                TagNames = article.ArticleTags.Select(at => at.Tag.Name).ToList()
            };
        }

        public ServiceMessage UpdateArticle(UpdateArticleDto updateArticleDto, int id)
        {
            var article = _articleRepository.Get(x => x.Id == id);
            if (article == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Makale Bulunamadı." };
            }

            article.Title = updateArticleDto.Title;
            article.Content = updateArticleDto.Content;
            article.ImageUrl = updateArticleDto.ImageUrl;
            article.ModifiedDate = DateTime.Now;

            _articleRepository.Update(article);

            var existingTags = _articleTagRepository.GetAll(at => at.ArticleId == id).ToList();
            foreach (var existingTag in existingTags)
            {
                _articleTagRepository.Delete(existingTag);
            }

            foreach (var tagId in updateArticleDto.TagIds)
            {
                _articleTagRepository.Add(new ArticleTag
                {
                    ArticleId = article.Id,
                    TagId = tagId
                });
            }

            return new ServiceMessage() { IsSucceed = true };
        }

        public ServiceMessage DeleteArticle(int id)
        {
            var article = _articleRepository.Get(x => x.Id == id);
            if (article == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Makale Bulunamadı." };
            }

            article.IsDeleted = true;
            _articleRepository.Update(article);

            return new ServiceMessage() { IsSucceed = true };
        }
    }
}
