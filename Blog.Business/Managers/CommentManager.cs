using Blog.Business.Dtos.CommentDtos;
using Blog.Business.Services;
using Blog.Business.Types;
using Blog.Data.Entities;
using Blog.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Business.Managers
{
    public class CommentManager : ICommentService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<User> _userRepository;

        public CommentManager(IRepository<Comment> commentRepository, IRepository<User> userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        public ServiceMessage AddComment(AddCommentDto addCommentDto)
        {
            var user = _userRepository.Get(x => x.Id == addCommentDto.UserId);
            if (user == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Kullanıcı Bulunamadı." };
            }

            var entity = new Comment()
            {
                Content = addCommentDto.Content,
                ArticleId = addCommentDto.ArticleId,
                UserId = addCommentDto.UserId
            };

            _commentRepository.Add(entity);

            return new ServiceMessage() { IsSucceed = true };
        }

        public List<CommentInfoDto> GetCommentsByArticleId(int articleId)
        {
            var comments = _commentRepository.GetAll(c => c.ArticleId == articleId).Include(c => c.User).Select(comment => new CommentInfoDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserName = comment.User.FirstName + " " + comment.User.LastName,
                ArticleId = comment.ArticleId,
                UserId = comment.UserId,
                ProfileImageUrl = comment.User.ProfileImageUrl 
            }).ToList();

            return comments;
        }

        public CommentInfoDto GetCommentById(int id)
        {
            var comment = _commentRepository.GetAll()
                                            .Include(c => c.User)
                                            .FirstOrDefault(x => x.Id == id);
            if (comment == null)
            {
                return null;
            }

            return new CommentInfoDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserName = comment.User.FirstName + " " + comment.User.LastName,
                ArticleId = comment.ArticleId,
                UserId = comment.UserId,
                ProfileImageUrl = comment.User.ProfileImageUrl 
            };
        }

        public ServiceMessage DeleteComment(int id)
        {
            var comment = _commentRepository.Get(x => x.Id == id);
            if (comment == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Yorum Bulunamadı." };
            }

            comment.IsDeleted = true;
            _commentRepository.Update(comment);

            return new ServiceMessage() { IsSucceed = true };
        }
    }
}
