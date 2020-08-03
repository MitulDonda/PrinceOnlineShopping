using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShoping.Models.ViewModel
{
    public class EditUserRoleViewModel
    {
        public EditUserRoleViewModel()
        {
            Users = new List<UserRoleViewModel>();
        }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public List<UserRoleViewModel> Users {get;set;}
    }
}
