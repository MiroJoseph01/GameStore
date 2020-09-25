using System;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Models;
using GameStore.Web.Utill;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("/user/ban")]
        public IActionResult BanUser(string userId)
        {
            var model = new BanUserViewModel { UserId = userId };

            InitializeBanDuration(model);

            return View(model);
        }

        [HttpPost]
        [Route("/user/ban/confirm")]
        public IActionResult BanUserConfirm(BanUserConfirmationViewModel user)
        {
            _userService.BanUser(user.UserId, user.Duration);

            return View(user);
        }

        private void InitializeBanDuration(BanUserViewModel model)
        {
            var values = Enum.GetValues(typeof(BanDuration));

            foreach (BanDuration bd in values)
            {
                model.Durations.Add(new SelectListItem
                {
                    Value = bd.ToInt().ToString(),
                    Text = bd.ToString(),
                });
            }
        }
    }
}
