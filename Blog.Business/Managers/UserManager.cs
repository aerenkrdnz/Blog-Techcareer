using Blog.Business.Dtos.UserDtos;
using Blog.Business.Services;
using Blog.Business.Types;
using Blog.Data.Entities;
using Blog.Data.Enums;
using Blog.Data.Repositories;
using Microsoft.AspNetCore.DataProtection;
using System;

namespace Blog.Business.Managers
{
    public class UserManager : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IDataProtector _dataProtector;

        public UserManager(IRepository<User> userRepository, IDataProtectionProvider dataProtectionProvider)
        {
            _userRepository = userRepository;
            _dataProtector = dataProtectionProvider.CreateProtector("security");
        }

        public ServiceMessage AddUser(AddUserDto addUserDto)
        {
            var hasMail = _userRepository.GetAll(x => x.Email.ToLower() == addUserDto.Email.ToLower()).ToList();
            if (hasMail.Any())
            {
                return new ServiceMessage()
                {
                    IsSucceed = false,
                    Message = "Bu Eposta adresli bir kullanıcı zaten mevcut."
                };
            }

            var entity = new User()
            {
                Email = addUserDto.Email,
                FirstName = addUserDto.FirstName,
                LastName = addUserDto.LastName,
                Password = _dataProtector.Protect(addUserDto.Password),
                UserType = UserTypeEnum.User,
                ProfileImageUrl = addUserDto.ProfileImageUrl
            };

            _userRepository.Add(entity);

            return new ServiceMessage()
            {
                IsSucceed = true,
            };
        }

        public UserInfoDto LoginUser(LoginDto loginDto)
        {
            var userEntity = _userRepository.Get(x => x.Email == loginDto.Email);
            if (userEntity is null)
            {
                return null;
            }

            var rawPassword = _dataProtector.Unprotect(userEntity.Password);
            if (rawPassword == loginDto.Password)
            {
                return new UserInfoDto()
                {
                    Id = userEntity.Id,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    UserType = userEntity.UserType,
                    ProfileImageUrl = userEntity.ProfileImageUrl
                };
            }
            else
            {
                return null;
            }
        }

        public UserInfoDto GetUserById(int id)
        {
            var user = _userRepository.Get(x => x.Id == id);
            if (user == null)
            {
                return null;
            }

            return new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                ProfileImageUrl = user.ProfileImageUrl
            };
        }

        public ServiceMessage UpdateUser(UpdateUserDto updateUserDto)
        {
            var user = _userRepository.Get(x => x.Id == updateUserDto.Id);
            if (user == null)
            {
                return new ServiceMessage() { IsSucceed = false, Message = "Kullanıcı Bulunamadı." };
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.ProfileImageUrl = updateUserDto.ProfileImageUrl;

            _userRepository.Update(user);

            return new ServiceMessage() { IsSucceed = true };
        }
    }
}
