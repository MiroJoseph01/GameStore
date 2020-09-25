using GameStore.BLL.Interfaces;
using GameStore.Web.Controllers;
using GameStore.Web.Util;
using GameStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GameStore.Web.Tests
{
    public class UserControllerTest
    {
        private readonly UserController _userController;
        private readonly Mock<IUserService> _userService;

        public UserControllerTest()
        {
            _userService = new Mock<IUserService>();

            _userController = new UserController(_userService.Object);
        }

        [Fact]
        public void BanUser_PassUserId_ReturnView()
        {
            var result = _userController.BanUser(Constants.UserId);

            var view = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<BanUserViewModel>(view.Model);
        }

        [Fact]
        public void BanUserConfirm_PassBanUserViewModel_ReturnsView()
        {
            var user = new BanUserConfirmationViewModel
            {
                UserId = Constants.UserId,
                Duration = BLL.Models.BanDuration.Day,
            };

            var result = _userController.BanUserConfirm(user);

            _userService.Verify(x => x.BanUser(user.UserId, user.Duration));

            var view = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<BanUserConfirmationViewModel>(view.Model);
        }
    }
}
