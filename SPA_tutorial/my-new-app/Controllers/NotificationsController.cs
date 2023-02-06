using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using my_new_app.Data;
using my_new_app.Models;

namespace my_new_app.Controllers
{
    [Route("api/[controller]/[action]")]
    public class NotificationsController : Controller
    {
        private WeatherDbContext _context;

        public NotificationsController(WeatherDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessage(DataSourceLoadOptions loadOptions) {
            var notifications = _context.notifications.Select(i => new {
                i.Id,
                i.Subject,
                i.Sender,
                i.Message,
                i.Date,
                i.Status
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(notifications, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> GetUnread(DataSourceLoadOptions loadOptions)
        {
            var notifications = _context.notifications.Select(i => new {
                i.Id,
                i.Subject,
                i.Sender,
                i.Message,
                i.Date,
                i.Status
            }).Where(i => i.Status == "U");

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(notifications, loadOptions));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNotifications(string values)
        {
            var keys = JsonConvert.DeserializeObject<IDictionary>(values);

            var model = await _context.notifications.FirstOrDefaultAsync(item => item.Id == Convert.ToInt32(keys["id"]));
            if (model == null)
                return StatusCode(409, "Object not found");

            model.Status = Convert.ToString(keys["status"]);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Notification();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.notifications.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.Id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.notifications.FirstOrDefaultAsync(item => item.Id == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.notifications.FirstOrDefaultAsync(item => item.Id == key);

            _context.notifications.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Notification model, IDictionary values) {
            string ID = nameof(Notification.Id);
            string SUBJECT = nameof(Notification.Subject);
            string ADDRESSED = nameof(Notification.Sender);
            string MESSAGE = nameof(Notification.Message);
            string DATE = nameof(Notification.Date);
            string STATUS = nameof(Notification.Status);

            if(values.Contains(ID)) {
                model.Id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(SUBJECT)) {
                model.Subject = Convert.ToString(values[SUBJECT]);
            }

            if(values.Contains(ADDRESSED)) {
                model.Sender = Convert.ToString(values[ADDRESSED]);
            }

            if(values.Contains(MESSAGE)) {
                model.Message = Convert.ToString(values[MESSAGE]);
            }

            if(values.Contains(DATE)) {
                model.Date = Convert.ToDateTime(values[DATE]);
            }

            if(values.Contains(STATUS)) {
                model.Status = Convert.ToString(values[STATUS]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}