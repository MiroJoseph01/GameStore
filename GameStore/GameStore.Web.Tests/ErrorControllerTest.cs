using GameStore.Web.Controllers;
using Xunit;

namespace GameStore.Web.Tests
{
    public class ErrorControllerTest
    {
        private readonly ErrorController _errorController;

        public ErrorControllerTest()
        {
            _errorController = new ErrorController();
        }

        [Fact]
        public void HandleErrorHttpStatus_Pass404CodeError_ReturnNotFoundPage()
        {
            int statusCode = 404;
            string message = "404! This page is not found";

            _errorController.HandleErrorHttpStatus(statusCode);

            Assert.Equal(message, _errorController.ViewBag.ErrorMessage);
        }

        [Fact]
        public void HandleErrorHttpStatus_Pass500CodeError_ReturnInternalServerErrorPage()
        {
            int statusCode = 500;
            string message = "500! The server has encountered a situation it doesn't know how to handle";

            _errorController.HandleErrorHttpStatus(statusCode);

            Assert.Equal(message, _errorController.ViewBag.ErrorMessage);
        }

        [Fact]
        public void HandleErrorHttpStatus_PassCodeError_ReturnDefaultPage()
        {
            int statusCode = -1;
            string message = "Oooops! Something went wrong";

            _errorController.HandleErrorHttpStatus(statusCode);

            Assert.Equal(message, _errorController.ViewBag.ErrorMessage);
        }
    }
}
