﻿using StreamInSync.Models;
using System.Collections.Generic;

namespace StreamInSync.Services
{
    public interface IUserService
    {
        bool Create(RegisterVM newUser);

        User Get(string username, string password);

        User Get(int userId);
    }
}