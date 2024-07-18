using Blog.Business.Dtos.TagDtos;
using Blog.Business.Services;
using Blog.Business.Types;
using Blog.Data.Entities;
using Blog.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Managers
{
    public class TagManager : ITagService
    {
        private readonly IRepository<Tag> _tagRepository;

        public TagManager(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public ServiceMessage AddTag(AddTagDto addTagDto)
        {
            var entity = new Tag
            {
                Name = addTagDto.Name
            };

            _tagRepository.Add(entity);

            return new ServiceMessage() { IsSucceed = true };
        }

        public List<TagInfoDto> GetAllTags()
        {
            var tags = _tagRepository.GetAll().Select(tag => new TagInfoDto
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToList();

            return tags;
        }

        public TagInfoDto GetTagById(int id)
        {
            var tag = _tagRepository.Get(x => x.Id == id);
            if (tag == null)
            {
                return null;
            }

            return new TagInfoDto
            {
                Id = tag.Id,
                Name = tag.Name
            };
        }
    }
}
