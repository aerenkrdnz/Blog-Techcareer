using Blog.Business.Dtos.CommentDtos;
using Blog.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Services
{
    public interface ICommentService
    {
        ServiceMessage AddComment(AddCommentDto addCommentDto);
        List<CommentInfoDto> GetCommentsByArticleId(int articleId);
        ServiceMessage DeleteComment(int id);
        CommentInfoDto GetCommentById(int id);
    }
}
