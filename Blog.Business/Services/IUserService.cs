using Blog.Business.Dtos.UserDtos;
using Blog.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Business.Services
{
    public interface IUserService
    {
        ServiceMessage AddUser(AddUserDto addUserDto);
        UserInfoDto LoginUser(LoginDto loginDto);
    }
}
