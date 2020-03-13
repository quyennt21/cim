using AutoMapper;
using CIM.Common;
using CIM.Model.Models;
using CIM.Web.Infrastructure;
using CIM.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CIM.Web.Controllers
{
    [Authorize(Roles = "System Admin")]
    public class UserController : BaseController
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UserController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        // GET: User
        public ActionResult Index(int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));
            int totalRow = UserManager.Users.Count();
            var usersModel = UserManager.Users.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
            var userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(usersModel);
            var roles = new List<string>();
            foreach (var user in userViewModel)
            {
                string roleStr = "";
                foreach (var role in UserManager.GetRoles(user.Id))
                {
                    roleStr = (roleStr == "") ? role.ToString() : roleStr + " - " + role.ToString();
                }
                user.Role = new RoleViewModel();
                user.Role.Name = roleStr;
            }
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            //get all Role
            var rolesModel = RoleManager.Roles.ToList();
            ViewBag.roleViewModel = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleViewModel>>(rolesModel);
            var paginationSet = new PaginationSet<UserViewModel>()
            {
                Items = userViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };
            ViewBag.query = new
            {
                page = page
            };
            return View(paginationSet);
        }

        // GET: User
        public ActionResult Search(string userName, string roleName, int status, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetKey("PageSize"));
            var query = UserManager.Users;
            var predicate = PredicateBuilder.True<ApplicationUser>();
            if (status != 2)
            {
                var isStatus = PredicateBuilder.False<ApplicationUser>();
                isStatus = isStatus.Or(a => (a.Active ? 1 : 0) == status);
                predicate = predicate.And(isStatus);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                var IsUserName = PredicateBuilder.False<ApplicationUser>();
                IsUserName = IsUserName.Or(a => a.UserName.Contains(userName.Trim()));
                predicate = predicate.And(IsUserName);
            }
            if (roleName != "All")
            {
                var addUser = PredicateBuilder.False<ApplicationUser>();
                foreach (var user in query.ToList())
                {
                    if (UserManager.IsInRole(user.Id, roleName))
                    {
                        addUser = addUser.Or(a => a.Id == user.Id);
                    }
                }
                predicate = predicate.And(addUser);
            }
            query = query.Where(predicate);
            int totalRow = query.Count();
            query = query.OrderBy(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize);
            var userViewModel = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserViewModel>>(query);
            var roles = new List<string>();
            foreach (var user in userViewModel)
            {
                string roleStr = "";
                foreach (var role in UserManager.GetRoles(user.Id))
                {
                    roleStr = (roleStr == "") ? role.ToString() : roleStr + " - " + role.ToString();
                }
                user.Role = new RoleViewModel();
                user.Role.Name = roleStr;
            }
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);
            //get all Role
            var rolesModel = RoleManager.Roles.ToList();
            ViewBag.roleViewModel = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleViewModel>>(rolesModel);
            ViewBag.UserName = userName;
            var paginationSet = new PaginationSet<UserViewModel>()
            {
                Items = userViewModel,
                MaxPage = int.Parse(ConfigHelper.GetKey("MaxSize")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };
            ViewBag.query = new
            {
                userName = userName,
                roleName = roleName,
                status = status,
                page = page
            };

            return View("Index", paginationSet);
        }

        // GET: User/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var usersModel = await UserManager.FindByIdAsync(id);

            if (usersModel == null)
            {
                return HttpNotFound();
            }

            //get roles of user
            ViewBag.rolesOfUser = await UserManager.GetRolesAsync(id);

            var userViewModel = Mapper.Map<ApplicationUser, UserViewModel>(usersModel);

            return View(userViewModel);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            var viewModel = new UserViewModel();

            //get all Role
            var rolesModel = RoleManager.Roles.ToList();

            ViewBag.roleViewModel = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleViewModel>>(rolesModel);

            return View(viewModel);
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(UserViewModel viewModel)
        {
            try
            {
                var rolesModel = RoleManager.Roles.ToList();
                ViewBag.roleViewModel = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleViewModel>>(rolesModel);
                var validateMail = UserManager.Users.FirstOrDefault(x => x.Email == viewModel.Email && x.Id != viewModel.Id);
                if (validateMail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
                if (!ModelState.IsValid)
                {
                    // xy ly loi

                    return View("Create");
                }

                var user = new ApplicationUser()
                {
                    UserName = viewModel.Email.Split('@')[0].ToLower(),
                    Email = viewModel.Email.ToLower(),
                    Active = true
                };

                var addUser = await UserManager.CreateAsync(user);

                if (addUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, viewModel.Role.Name);
                }
                SetAlert("Add User success", "success");
            }
            catch(Exception e)
            {
                SetAlert("Add User error", "error");
            }
            return RedirectToAction("Index");
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            var userModel = UserManager.FindById(id);

            if (userModel == null)
            {
                return HttpNotFound();
            }

            var userRoles = UserManager.GetRoles(id);

            string roleOfUser = null;
            if (userRoles.Count > 0)
            {
                roleOfUser = userRoles[0];
            }

            //get all Role
            var rolesModel = RoleManager.Roles.ToList();

            ViewBag.roleViewModel = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleViewModel>>(rolesModel);

            var viewModel = Mapper.Map<ApplicationUser, UserViewModel>(userModel);
            viewModel.Role = new RoleViewModel();
            viewModel.Role.Name = roleOfUser;

            return View(viewModel);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel viewModel)
        {
            try
            {
                var validateMail = UserManager.Users.FirstOrDefault(x => x.Email == viewModel.Email && x.Id != viewModel.Id);
                if (validateMail != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
                if (!ModelState.IsValid)
                {
                    var rolesModel = RoleManager.Roles.ToList();
                    ViewBag.roleViewModel = Mapper.Map<IEnumerable<ApplicationRole>, IEnumerable<RoleViewModel>>(rolesModel);
                    // xy ly loi
                    return View(viewModel);
                }

                // TODO: Add update logic here
                ApplicationUser model = UserManager.FindById(viewModel.Id);
                model.UserName = viewModel.UserName;
                model.FullName = viewModel.FullName;
                model.Email = viewModel.Email;
                model.Active = viewModel.Active;
                IdentityResult result = UserManager.Update(model);

                if (viewModel.Role.Name != null)
                {
                    var userRoles = UserManager.GetRoles(model.Id);

                    if (userRoles.Count > 0)
                    {
                        //remove old role
                        UserManager.RemoveFromRoles(model.Id, userRoles.ToArray());
                    }

                    UserManager.AddToRole(model.Id, viewModel.Role.Name);
                }
                SetAlert("Update User success", "success");
            }
            catch(Exception e)
            {
                SetAlert("Udpate User error", "error");
            }
            return RedirectToAction("Index");
        }
    }
}