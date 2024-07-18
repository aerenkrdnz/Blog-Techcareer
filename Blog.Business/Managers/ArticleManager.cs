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

        public ArticleManager(IRepository<Article> articleRepository, IRepository<User> userRepository)
        {
            _articleRepository = articleRepository;
            _userRepository = userRepository;
        }

        public ServiceMessage AddArticle(AddArticleDto addArticleDto)
        {
            var user = _userRepository.Get(x => x.Id == addArticleDto.UserId);
            if (user == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Kullanıcı Bulunamadı." };
            }

            var entity = new Article()
            {
                Title = addArticleDto.Title,
                Content = addArticleDto.Content,
                ImageUrl = addArticleDto.ImageUrl,
                UserId = addArticleDto.UserId
            };

            _articleRepository.Add(entity);

            return new ServiceMessage() { IsSucceed = true };
        }

        public List<ArticleInfoDto> GetAllArticles()
        {
            var articles = _articleRepository.GetAll().Select(article => new ArticleInfoDto
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                ImageUrl = article.ImageUrl,
                UserName = article.User.FirstName + " " + article.User.LastName,
                UserId = article.UserId 
            }).ToList();

            return articles;
        }

        public ArticleInfoDto GetArticleById(int id)
        {
            var article = _articleRepository.GetAll()
                .Include(a => a.User)
                .FirstOrDefault(x => x.Id == id);

            if (article == null)
            {
                return null;
            }

            return new ArticleInfoDto()
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                ImageUrl = article.ImageUrl,
                UserName = article.User.FirstName + " " + article.User.LastName,
                UserId = article.UserId 
            };
        }

        public ServiceMessage UpdateArticle(UpdateArticleDto updateArticleDto, int id)
        {
            var article = _articleRepository.Get(x => x.Id == id);
            if (article == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Blog Bulunamadı." };
            }

            article.Title = updateArticleDto.Title;
            article.Content = updateArticleDto.Content;
            article.ImageUrl = updateArticleDto.ImageUrl;
            article.ModifiedDate = DateTime.Now;

            _articleRepository.Update(article);

            return new ServiceMessage() { IsSucceed = true };
        }

        public ServiceMessage DeleteArticle(int id)
        {
            var article = _articleRepository.Get(x => x.Id == id);
            if (article == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Blog Bulunamadı." };
            }

            article.IsDeleted = true;
            _articleRepository.Update(article);

            return new ServiceMessage() { IsSucceed = true };
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
                UserId = article.UserId
            }).ToList();

            return articles;
        }


    }
}
