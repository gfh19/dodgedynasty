using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using DodgeDynasty.Models;
using DodgeDynasty.Mappers;
using DodgeDynasty.Shared;
using DodgeDynasty.Filters;
using DodgeDynasty.Mappers.Account;
using DodgeDynasty.Models.Account;

namespace DodgeDynasty.Controllers
{
	[Authorize]
	public class AccountController : BaseController
	{
		//
		// GET: /Account/Login

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginModel model, string returnUrl)
		{
			if (model != null && model.UserName != null)
			{
				model.UserName = model.UserName.Trim().ToLower();
			}
			if (ModelState.IsValid && model.Login(Request))
			{
				FormsAuthentication.SetAuthCookie(model.UserName, true);
				GetDodgeDynastyCookie();
				return RedirectToLocal(returnUrl);
			}
			ModelState.AddModelError("", "The user name or password provided is incorrect.");
			return View(model);
		}

		//
		// POST: /Account/LogOff

		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			ExpireCookie(Constants.Cookies.DodgeDynasty);
			ExpireCookie(Constants.Cookies.PlayerRankOptions);
			return RedirectToAction("Index", "Home");
		}

		public ActionResult ChangePassword(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: "";
			//ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
			ViewBag.HasLocalPassword = Utilities.IsUserLoggedIn();
			ViewBag.ReturnUrl = Url.Action("ChangePassword");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ChangePassword(LocalPasswordModel model)
		{
			ViewBag.ReturnUrl = Url.Action("ChangePassword");
			if (ModelState.IsValid)
			{
				var mapper = new PasswordMapper<LocalPasswordModel>();
				bool changePasswordSucceeded;
				try
				{
					mapper.UserName = User.Identity.Name;
					mapper.UpdateEntity(model);
					changePasswordSucceeded = mapper.UpdateSucceeded;
				}
				catch (Exception)
				{
					changePasswordSucceeded = false;
				}

				if (changePasswordSucceeded)
				{
					return RedirectToAction("ChangePassword", new { Message = ManageMessageId.ChangePasswordSuccess });
				}
				else
				{
					if (mapper.PasswordStatus == ManageMessageId.ConfirmPasswordMismatch)
					{
						ModelState.AddModelError("", "New Password and Confirm Password do not match.");
					}
					else if (mapper.PasswordStatus == ManageMessageId.CurrentPasswordInvalid)
					{
						ModelState.AddModelError("CurrentPassword", "Current Password is incorrect.");
					}
					else
					{
						ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		public ActionResult MyInfo()
		{
			var mapper = new MyInfoMapper();
			var model = mapper.GetModel();
			return View(model);
		}

		[HttpPost]
		public ActionResult MyInfo(UserInfoModel model)
		{
			var mapper = new MyInfoMapper();
			mapper.ModelState = ModelState;
			if (!mapper.UpdateEntity(model))
			{
				return View(mapper.GetUpdatedModel(model));
			}
			return View(mapper.GetModel());
		}

		#region Helpers
		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}
		#endregion
	}
}
