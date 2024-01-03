using ConsumerListApp.IRepository;
using ConsumerListApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using X.PagedList;

namespace ConsumerListApp.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _user;

        public HomeController(ILogger<HomeController> logger, IUserRepository user)
        {
            _logger = logger;
            _user = user;
        }


        public IActionResult Index(string error)
        {
            if(error != null)
            {
                ViewBag.LoginError = error;
            }
            string checkIfLoggedin = HttpContext.Session.GetString("IsAuthenticated");
            if (checkIfLoggedin == "true")
            {
                return RedirectToAction("AdminDashboard");
            }
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.SetString("IsAuthenticated", "false");
            return RedirectToAction("Index");
        }

        public ActionResult AdminDashboard(ConsumerPagingInfoModel model)
        {
            string checkIfLoggedin = HttpContext.Session.GetString("IsAuthenticated");
            if (checkIfLoggedin == "true")
            {
                return View("AdminDashboard");
            }
            if(model != null)
            {
                return View(model);
            }
            return NoContent();
        }

        public IActionResult UserDashboard()
        {
            string checkIfLoggedin = HttpContext.Session.GetString("IsAuthenticated");
            if (checkIfLoggedin == "true")
            {
                return View();
            }
            return NoContent();
        }


        public ActionResult Register(UserModel user)
        {
            _user.CreateUser(user);
            return RedirectToAction("Index");
        } 


        public ActionResult Login(UserModel model)
        {
            UserModel data = _user.ValidateUser(model);
            if (data == null)
            {
                string errorMsg = "Invalid Username/Password";
                return RedirectToAction("Index", new { Error = errorMsg });
            }
            else if (data.IsAdmin == true)
            {
                if (data.UserName == model.UserName)
                {
                    HttpContext.Session.SetString("IsAuthenticated", "true");
                    return RedirectToAction("AdminDashboard");
                }
                else
                {
                    string errorMsg = "Invalid Username/Password";
                    return RedirectToAction("Index", new { Error = errorMsg });
                }
            }
            else if (data.UserName == model.UserName)
            {
                HttpContext.Session.SetString("IsAuthenticated", "true");
                return RedirectToAction("UserDashboard");
            }
            else
            {
                string errorMsg = "Invalid Username/Password";
                return RedirectToAction("Index", new { Error = errorMsg });
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetConsumerData(string Flag, string FeederInput, string SubstaionCodeInput, string SubDivisionCodeInput, int jtStartIndex = 0, int jtPageSize = 0)
        {
            ApiSubReqModel apiSubReqModel = new ApiSubReqModel();
            ApiFeederReqModel apiFeederReqModel = new ApiFeederReqModel();
            if (Flag == "F")
            {
                apiFeederReqModel.flag = Flag;
                if (FeederInput == null)
                {
                    return Json(new { Result = "OK", Records = "", TotalRecordCount = "".Count() });
                }
                apiFeederReqModel.feeder_code = FeederInput.Split(',').ToList();

                var data = await _user.GetConsumerDataApi(Flag, apiFeederReqModel, apiSubReqModel, jtStartIndex, jtPageSize);
                if(data == null)
                {
                    return Json(new { Result = "OK", Records = "", TotalRecordCount = "".Count() });
                }
                return Json(new { Result = "OK", Records = data, TotalRecordCount = data.Count() });
            }
            else if (Flag == "S")
            {                
                apiSubReqModel.flag = Flag;
                if (SubstaionCodeInput == null || SubDivisionCodeInput == null)
                {
                    return Json(new { Result = "OK", Records = "", TotalRecordCount = "".Count() });
                }
                apiSubReqModel.substationcode = SubstaionCodeInput;
                apiSubReqModel.sdocode = SubDivisionCodeInput;


                var data = await _user.GetConsumerDataApi(Flag, apiFeederReqModel, apiSubReqModel, jtStartIndex, jtPageSize);
                if (data == null)
                {
                    return Json(new { Result = "OK", Records = "", TotalRecordCount = "".Count() });
                }
                return Json(new { Result = "OK", Records = data, TotalRecordCount = data.Count() });
            }
            return Json(new { Result = "OK", Records = "", TotalRecordCount = "".Count() });
        }



        public async Task<ActionResult> SaveToDb(string Flag, string FeederInput, string SubstaionCodeInput, string SubDivisionCodeInput)
        {
            ApiFeederReqModel apiFeederReqModel = new ApiFeederReqModel();
            ApiSubReqModel apiSubReqModel = new ApiSubReqModel();
            if (Flag == "F")
            {
                apiFeederReqModel.flag = Flag;
                if (FeederInput == null)
                {
                    return NoContent();
                }
                apiFeederReqModel.feeder_code = FeederInput.Split(',').ToList();

                var data = await _user.GetConsumerDataApi(Flag, apiFeederReqModel, apiSubReqModel, 0, 0);

                foreach(ApiResModel item in data)
                {
                    _user.SaveConsumerData(item);
                }
            }
            else if (Flag == "S")
            {

                apiSubReqModel.flag = Flag;
                if (SubstaionCodeInput == null || SubDivisionCodeInput == null)
                {
                    return NoContent(); 
                }
                apiSubReqModel.substationcode = SubstaionCodeInput;
                apiSubReqModel.sdocode = SubDivisionCodeInput;

                var data = await _user.GetConsumerDataApi(Flag, apiFeederReqModel, apiSubReqModel, 0, 0);

                foreach (ApiResModel item in data)
                {
                    _user.SaveConsumerData(item);
                }
            }
            return NoContent();
        }


        public async Task<ActionResult> Grid(string Flag, string FeederInput, string SubstaionCodeInput, string SubDivisionCodeInput, int sortby, bool isAsc = true, int? page = 1)
        {

            if (page < 0)
            {
                page = 1;
            }

            ConsumerPagingInfoModel ConsumerPagingInfo = new ConsumerPagingInfoModel();
            var pageIndex = (page ?? 1) - 1;
            var pageSize = 5;

            string sortColumn;
            #region SortingColumn
            switch (sortby)
            {
                case 1:
                    if (isAsc)
                        sortColumn = "FeederCode";
                    else
                        sortColumn = "FeederCode Desc";
                    break;

                case 2:
                    if (isAsc)
                        sortColumn = "Name";
                    else
                        sortColumn = "Name Desc";
                    break;

                case 3:
                    if (isAsc)
                        sortColumn = "SubstationCode";
                    else
                        sortColumn = "SubstationCode Desc";
                    break;
                default:
                    sortColumn = "Name asc";
                    break;

            }
            #endregion
            ApiFeederReqModel apiFeederReqModel = new ApiFeederReqModel();
            ApiSubReqModel apiSubReqModel = new ApiSubReqModel();
            if (Flag == "F")
            {
                apiFeederReqModel.flag = Flag;
                if (FeederInput == null)
                {
                    return NoContent();
                }
                apiFeederReqModel.feeder_code = FeederInput.Split(',').ToList();

                var consumers = await _user.GetConsumerDataApi(Flag, apiFeederReqModel, apiSubReqModel, 0, 0);
                var consumersPagedList = new StaticPagedList<ApiResModel>(consumers, pageIndex + 1, pageSize, consumers.Count());
                ConsumerPagingInfo.Consumers = consumersPagedList;
                ConsumerPagingInfo.pageSize = page;
                ConsumerPagingInfo.sortBy = sortby;
                ConsumerPagingInfo.isAsc = isAsc;
                return Json(consumers);

            }
            else if (Flag == "S")
            {

                apiSubReqModel.flag = Flag;
                if (SubstaionCodeInput == null || SubDivisionCodeInput == null)
                {
                    return NoContent();
                }
                apiSubReqModel.substationcode = SubstaionCodeInput;
                apiSubReqModel.sdocode = SubDivisionCodeInput;

                var consumers = await _user.GetConsumerDataApi(Flag, apiFeederReqModel, apiSubReqModel, 0, 0);
                var consumersPagedList = new StaticPagedList<ApiResModel>(consumers, pageIndex + 1, pageSize, consumers.Count());
                ConsumerPagingInfo.Consumers = consumersPagedList;
                ConsumerPagingInfo.pageSize = page;
                ConsumerPagingInfo.sortBy = sortby;
                ConsumerPagingInfo.isAsc = isAsc;
                return Json(new { Result = "OK", Records = consumers, TotalRecordCount = consumers.Count() });
            }
            return NoContent();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}