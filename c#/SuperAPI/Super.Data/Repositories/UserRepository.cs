﻿using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Super.Core.DTOs;
using Super.Core.Models;
using Super.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super.Data.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public List<User> GetList()
        {
            var users = _context.Users.ToList();

            return users;
        }
        public User GetUserById(int Id)
        {
            var user = _context.Users.Find(Id);
            if (user != null)
            {
                return user;
            }
            return null;
        }
        public void SignUp(User user)
        {
            var existingUser = _context.Users
                .Include(u => u.UserRoles) 
                .FirstOrDefault(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                throw new Exception("User already exists!");
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
            user.Password = hashedPassword;
            user.UserRoles = new List<UserRole>();
            var userRole = _context.Roles.FirstOrDefault(r => r.Name == ERole.ROLE_USER.ToString());
            if (userRole == null)
            {
                userRole = new Role { Name = ERole.ROLE_USER.ToString() };
                _context.Roles.Add(userRole);
                _context.SaveChanges(); 
            }
            user.UserRoles.Add(new UserRole { User = user, Role = userRole });
            if (user.UserName.StartsWith("manager"))
            {
                var adminRole = _context.Roles.FirstOrDefault(r => r.Name == ERole.ROLE_ADMIN.ToString());
                if (adminRole == null)
                {
                    adminRole = new Role { Name = ERole.ROLE_ADMIN.ToString() };
                    _context.Roles.Add(adminRole);
                    _context.SaveChanges();
                }
                user.UserRoles.Add(new UserRole { User = user, Role = adminRole });
            }

            if (user.UserName == "manager1234")
            {
                var managerRole = _context.Roles.FirstOrDefault(r => r.Name == ERole.ROLE_MANAGER.ToString());
                if (managerRole == null)
                {
                    managerRole = new Role { Name = ERole.ROLE_MANAGER.ToString() };
                    _context.Roles.Add(managerRole);
                    _context.SaveChanges();
                }
                user.UserRoles.Add(new UserRole { User = user, Role = managerRole });
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            Console.WriteLine($"User {user.UserName} has the following roles:");
            foreach (var ur in user.UserRoles)
            {
                Console.WriteLine($"- {ur.Role.Name}");
            }
        }
        public void UpdateUser(int Id, User user)
        {
            var userToUpdate = _context.Users.Find(Id);
            if (userToUpdate != null)
            {
                userToUpdate.UserName = user.UserName;
                userToUpdate.Address = user.Address;
                userToUpdate.Email = user.Email;
                userToUpdate.Phone = user.Phone;

                if (user.Password != null )
                {
                    string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);
                    userToUpdate.Password = hashedPassword;
                }
                _context.SaveChanges();
            }
        }
        public void DeleteUser(int Id)
        {
            var user = _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefault(u => u.Id == Id);

            if (user != null)
            {
                _context.UserRoles.RemoveRange(user.UserRoles);
                _context.SaveChanges(); 
                _context.Users.Remove(user);
                _context.SaveChanges(); 
            }
            else
            {
                throw new Exception("User not found");
            }
             }

        public User GetUserByName(string userName)
        {
            return _context.Users
                .Include(u => u.UserRoles) 
                .ThenInclude(ur => ur.Role) 
                .FirstOrDefault(u => u.UserName == userName);
        }
    }
}
