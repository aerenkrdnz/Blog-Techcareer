using Blog.Business.Dtos.TagDtos;
using Blog.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Services
{
    public interface ITagService
    {
        ServiceMessage AddTag(AddTagDto addTagDto);
        List<TagInfoDto> GetAllTags();
        TagInfoDto GetTagById(int id);
    }
}
